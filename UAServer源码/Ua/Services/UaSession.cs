// Copyright (c) 2014 Converter Systems LLC

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Opc.Ua;
using Opc.Ua.Client;

namespace ConverterSystems.Workstation.Services
{
    [TypeConverter(typeof (ExpandableObjectConverter)), RuntimeNameProperty("SessionName")]
    public partial class UaSession : Session, INotifyPropertyChanged, ISupportInitialize
    {
        private static readonly ApplicationConfiguration Configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly SemaphoreSlim _mutex;
        private CancellationToken _cancellationToken;
        private int _deferlevel;
        private string _endpointUrl;
        private string _sessionName;
        private Task _stateMachineTask;

        static UaSession()
        {
            Configuration = GetConfiguration();
            Configuration.CertificateValidator.CertificateValidation += OnCertificateValidation;
            Configuration.TraceConfiguration.ApplySettings();
            Utils.SetTraceOutput(Configuration.TraceConfiguration.TraceMasks != 0 ? Utils.TraceOutput.StdOutAndFile : Utils.TraceOutput.Off);
            EnsureApplicationCertificate(Configuration);
        }

        public UaSession() : base(new NullTransportChannel(), Configuration, new ConfiguredEndpoint(null, new EndpointDescription(), EndpointConfiguration.Create(Configuration)), null)
        {
            _mutex = new SemaphoreSlim(1);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            WeakEventManager<Application, ExitEventArgs>.AddHandler(Application.Current, "Exit", OnApplicationExit);
        }

        [Category("Common")]
        public string EndpointUrl
        {
            get { return _endpointUrl; }
            set
            {
                _endpointUrl = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common")]
        public new string SessionName
        {
            get { return _sessionName; }
            set
            {
                _sessionName = value;
                NotifyPropertyChanged();
            }
        }

        [Category("Common"), DefaultValue(5000)]
        public new int KeepAliveInterval
        {
            get { return base.KeepAliveInterval; }
            set
            {
                base.KeepAliveInterval = value;
                NotifyPropertyChanged();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Subscription DefaultSubscription
        {
            get { return base.DefaultSubscription; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new object Handle
        {
            get { return base.Handle; }
            set
            {
                base.Handle = value;
                NotifyPropertyChanged();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(0)]
        public new int OperationTimeout
        {
            get { return base.OperationTimeout; }
            set
            {
                base.OperationTimeout = value;
                NotifyPropertyChanged();
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new StringCollection PreferredLocales
        {
            get { return base.PreferredLocales; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new DiagnosticsMasks ReturnDiagnostics
        {
            get { return base.ReturnDiagnostics; }
            set
            {
                base.ReturnDiagnostics = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void BeginInit()
        {
            _deferlevel++;
        }

        public void EndInit()
        {
            _deferlevel--;
            if (_deferlevel == 0 && !IsInDesignMode)
            {
                _stateMachineTask = Task.Run(() => StateMachine());
            }
        }

        public async Task ConnectAsync()
        {
            await TryConnectAsync();
            while (!Connected)
            {
                await Task.Delay(KeepAliveInterval*2, _cancellationToken);
                await TryConnectAsync();
            }
        }

        public async Task TryConnectAsync()
        {
            await _mutex.WaitAsync(_cancellationToken).ConfigureAwait(false);
            try
            {
                if (Connected)
                {
                    return;
                }
                try
                {
                    var endpointDescription = await GetEndpointAsync(EndpointUrl, true).ConfigureAwait(false);
                    ConfiguredEndpoint.Update(endpointDescription);
                    ConfiguredEndpoint.Configuration.UseBinaryEncoding = endpointDescription.EncodingSupport == BinaryEncodingSupport.Optional || endpointDescription.EncodingSupport == BinaryEncodingSupport.Required;
                    TransportChannel = SessionChannel.Create(Configuration, ConfiguredEndpoint.Description, ConfiguredEndpoint.Configuration, Configuration.SecurityConfiguration.ApplicationCertificate.Certificate, Configuration.CreateMessageContext());
                    Open(_sessionName, null);
                    NotifyPropertyChanged("Connected");
                    Trace.TraceInformation("Success connecting to endpoint '{0}'. ", ConfiguredEndpoint);
                }
                catch (ServiceResultException ex)
                {
                    Trace.TraceError("Error connecting to endpoint '{0}'. {1}", ConfiguredEndpoint, ex.Message);
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            await _mutex.WaitAsync(_cancellationToken).ConfigureAwait(false);
            try
            {
                if (!Connected)
                {
                    return;
                }
                foreach (var subscription in Subscriptions)
                {
                    subscription.Delete(true);
                }
                try
                {
                    await CloseSessionAsync(new CloseSessionRequest { DeleteSubscriptions = true }).ConfigureAwait(false);
                    Trace.TraceInformation("Success closing session with endpoint '{0}'.", ConfiguredEndpoint);
                }
                catch (ServiceResultException ex)
                {
                    Trace.TraceError("Error closing session with endpoint '{0}'. {1}", ConfiguredEndpoint, ex.Message);
                }
                SessionCreated(null, null);
                NotifyPropertyChanged("Connected");
                try
                {
                    CloseChannel();
                    Trace.TraceInformation("Success closing secure channel with endpoint '{0}'.", ConfiguredEndpoint);
                }
                catch (ServiceResultException ex)
                {
                    Trace.TraceError("Error closing secure channel with endpoint '{0}'. {1}", ConfiguredEndpoint, ex.Message);
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        public override StatusCode Close(int timeout)
        {
            if (Disposed)
            {
                return StatusCodes.Good;
            }
            _cancellationTokenSource.Cancel();
            var t = _stateMachineTask;
            if (t != null)
            {
                t.Wait(timeout);
            }
            var code = base.Close(timeout);
            return code;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Dispose();
                _mutex.Dispose();
            }
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return String.Format("UaSession SessionName: {0}, Endpoint: {1}", SessionName, ConfiguredEndpoint);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task StateMachine()
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    Trace.TraceInformation("State Disconnected.");
                    await WhenSubscriptionsNotEmpty(_cancellationToken);

                    Trace.TraceInformation("State Connecting.");
                    await ConnectAsync();

                    Trace.TraceInformation("State Connected.");
                    using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(new[] { _cancellationToken }))
                    {
                        await Task.WhenAny(WhenSubscriptionsEmpty(linkedCts.Token), WhenKeepAliveStopped(linkedCts.Token), AutoConnectSubscriptions(linkedCts.Token));
                        linkedCts.Cancel();
                    }

                    Trace.TraceInformation("State Disconnecting.");
                    await DisconnectAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Trace.TraceInformation("State canceled.");
            }
        }

        private async Task WhenSubscriptionsEmpty(CancellationToken token)
        {
            if (SubscriptionCount == 0)
            {
                return;
            }
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = (o, e) =>
            {
                var s = (UaSession) o;
                if (s.SubscriptionCount == 0)
                {
                    tcs.TrySetResult(true);
                }
            };
            using (token.Register(state => ((TaskCompletionSource<bool>) state).TrySetCanceled(), tcs, false))
            {
                try
                {
                    SubscriptionsChanged += handler;
                    await tcs.Task;
                }
                finally
                {
                    SubscriptionsChanged -= handler;
                }
            }
        }

        private async Task WhenSubscriptionsNotEmpty(CancellationToken token)
        {
            if (SubscriptionCount > 0)
            {
                return;
            }
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = (o, e) =>
            {
                var s = (UaSession) o;
                if (s.SubscriptionCount > 0)
                {
                    tcs.TrySetResult(true);
                }
            };
            using (token.Register(state => ((TaskCompletionSource<bool>) state).TrySetCanceled(), tcs, false))
            {
                try
                {
                    SubscriptionsChanged += handler;
                    await tcs.Task;
                }
                finally
                {
                    SubscriptionsChanged -= handler;
                }
            }
        }

        private async Task AutoConnectSubscriptions(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = (o, e) =>
            {
                foreach (var subscription in Subscriptions)
                {
                    if (!subscription.Created)
                    {
                        try
                        {
                            subscription.Create();
                            Trace.TraceInformation("Success creating subscription '{0}' on endpoint '{1}'. ", subscription.DisplayName, ConfiguredEndpoint);
                        }
                        catch (ServiceResultException ex)
                        {
                            Trace.TraceError("Error creating subscription '{0}' on endpoint '{1}'. {2}", subscription.DisplayName, ConfiguredEndpoint, ex.Message);
                        }
                    }
                }
            };
            using (token.Register(state => ((TaskCompletionSource<bool>) state).TrySetCanceled(), tcs, false))
            {
                try
                {
                    handler.Invoke(null, null);
                    SubscriptionsChanged += handler;
                    await tcs.Task;
                }
                finally
                {
                    SubscriptionsChanged -= handler;
                }
            }
        }

        private async Task WhenKeepAliveStopped(CancellationToken token)
        {
            if (KeepAliveStopped)
            {
                return;
            }
            var tcs = new TaskCompletionSource<bool>();
            KeepAliveEventHandler handler = (s, e) =>
            {
                if (ServiceResult.IsBad(e.Status))
                {
                    tcs.TrySetResult(true);
                }
            };
            using (token.Register(state => ((TaskCompletionSource<bool>) state).TrySetCanceled(), tcs, false))
            {
                try
                {
                    KeepAlive += handler;
                    await tcs.Task;
                }
                finally
                {
                    KeepAlive -= handler;
                }
            }
        }

        private void OnApplicationExit(object sender, ExitEventArgs exitEventArgs)
        {
            Close();
        }

        private static ApplicationConfiguration GetConfiguration()
        {
            var applicationName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            if (Path.GetExtension(applicationName) == ".vshost")
            {
                applicationName = Path.GetFileNameWithoutExtension(applicationName);
            }
            if (String.IsNullOrEmpty(applicationName))
            {
                throw new NullReferenceException("The applicationName was null.");
            }
            var configuration = new ApplicationConfiguration { ApplicationName = applicationName, ApplicationType = ApplicationType.Client, SecurityConfiguration = new SecurityConfiguration { ApplicationCertificate = new CertificateIdentifier { StoreType = @"Windows", StorePath = @"CurrentUser\My", SubjectName = Utils.Format(@"CN={0}, DC={1}", applicationName, Dns.GetHostName()) }, TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Windows", StorePath = @"CurrentUser\TrustedPeople", }, NonceLength = 32, }, TransportConfigurations = new TransportConfigurationCollection(), TransportQuotas = new TransportQuotas(), ClientConfiguration = new ClientConfiguration(), TraceConfiguration = new TraceConfiguration { TraceMasks = Utils.TraceMasks.Error | Utils.TraceMasks.Information | Utils.TraceMasks.Operation } };
            configuration.Validate(ApplicationType.Client);
            Trace.TraceInformation("Success building new ApplicationConfiguration for application {0}", configuration.ApplicationName);
            return configuration;
        }

        private static void EnsureApplicationCertificate(ApplicationConfiguration configuration)
        {
            const ushort keySize = 1024;
            const ushort lifetimeInMonths = 300;

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            bool errorFlag = false;
            string hostName = Dns.GetHostName();
            var serverDomainNames = configuration.GetServerDomainNames();
            var applicationCertificate = configuration.SecurityConfiguration.ApplicationCertificate;
            var certificate = applicationCertificate.Find(true);
            if (certificate != null)
            {
                // if cert found then check domains
                var domainsFromCertficate = Utils.GetDomainsFromCertficate(certificate);
                foreach (string serverDomainName in serverDomainNames)
                {
                    if (Utils.FindStringIgnoreCase(domainsFromCertficate, serverDomainName))
                    {
                        continue;
                    }
                    if (String.Equals(serverDomainName, "localhost", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Utils.FindStringIgnoreCase(domainsFromCertficate, hostName))
                        {
                            continue;
                        }
                        var hostEntry = Dns.GetHostEntry(hostName);
                        if (hostEntry.Aliases.Any(alias => Utils.FindStringIgnoreCase(domainsFromCertficate, alias)))
                        {
                            continue;
                        }
                        if (hostEntry.AddressList.Any(ipAddress => Utils.FindStringIgnoreCase(domainsFromCertficate, ipAddress.ToString())))
                        {
                            continue;
                        }
                    }
                    Trace.TraceInformation("The application is configured to use domain '{0}' which does not appear in the certificate.", serverDomainName);
                    errorFlag = true;
                } // end for
                // if no errors and keySizes match
                if (!errorFlag && (keySize == certificate.PublicKey.Key.KeySize))
                {
                    return; // cert okay
                }
            }
            // if we get here then we'll create a new cert
            if (certificate == null)
            {
                certificate = applicationCertificate.Find(false);
                if (certificate != null)
                {
                    Trace.TraceInformation("Matching certificate with SubjectName '{0}' found but without a private key.", applicationCertificate.SubjectName);
                }
            }
            // lets check if there is any to delete
            if (certificate != null)
            {
                using (var store2 = applicationCertificate.OpenStore())
                {
                    store2.Delete(certificate.Thumbprint);
                }
            }
            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(hostName);
            }
            CertificateFactory.CreateCertificate(applicationCertificate.StoreType, applicationCertificate.StorePath, configuration.ApplicationUri, configuration.ApplicationName, null, serverDomainNames, keySize, lifetimeInMonths);
            Trace.TraceInformation("Created new certificate with SubjectName '{0}', in certificate store '{1}'.", applicationCertificate.SubjectName, applicationCertificate.StorePath);
            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);
        }

        private static void OnCertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
                return;
            }
            Trace.TraceInformation(e.Error.ToLongString());
        }

        private static async Task<EndpointDescription> GetEndpointAsync(string endpointUrl, bool preferPerformance = false)
        {
            EndpointDescription endpointDescription;
            var endpointUri = new Uri(endpointUrl.Split(new[] { ' ' }, 1)[0]);
            var discoveryUri = new UriBuilder(endpointUri);
            var profileUris = new string[0];
            switch (endpointUri.Scheme)
            {
                case "opc.tcp":
                    profileUris = new[] { Profiles.UaTcpTransport };
                    break;
                case "https":
                    discoveryUri.Path += "/discovery";
                    profileUris = new[] { Profiles.HttpsBinaryTransport, Profiles.HttpsXmlOrBinaryTransport, Profiles.HttpsXmlTransport };
                    break;
                case "http":
                    discoveryUri.Path += "/discovery";
                    profileUris = new[] { Profiles.WsHttpXmlOrBinaryTransport, Profiles.WsHttpXmlTransport };
                    break;
            }
            using (var discoveryClient = UaDiscoveryClient.Create(discoveryUri.Uri))
            {
                discoveryClient.OperationTimeout = 5000;
                var request = new GetEndpointsRequest { EndpointUrl = discoveryClient.Endpoint.EndpointUrl, ProfileUris = profileUris };
                var response = await discoveryClient.GetEndpointsAsync(request).ConfigureAwait(false);
                endpointDescription = preferPerformance ? response.Endpoints.OrderBy(ed => profileUris.ToList().IndexOf(ed.TransportProfileUri)).ThenBy(ed => ed.SecurityLevel).First() : response.Endpoints.OrderBy(ed => profileUris.ToList().IndexOf(ed.TransportProfileUri)).ThenByDescending(ed => ed.SecurityLevel).First();
            }
            return endpointDescription;
        }
    }
}