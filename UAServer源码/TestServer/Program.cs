// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ConverterSystems.Test;
using Opc.Ua;

namespace ConverterSystems
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var config = GetConfiguration();
                config.CertificateValidator.CertificateValidation += (sender, e) =>
                {
                    if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                    {
                        e.Accept = true;
                        return;
                    }
                    Console.WriteLine(e.Error.ToLongString());
                };
                EnsureApplicationCertificate(config);
                var server = new TestServer();
                server.Start(config);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
            catch (SocketException e)
            {
                Console.WriteLine("A Server at this address is already running.");
            }
        }

        private static ApplicationConfiguration GetConfiguration()
        {
            var applicationName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            if (Path.GetExtension(applicationName) == ".vshost")
            {
                applicationName = Path.GetFileNameWithoutExtension(applicationName);
            }
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new NullReferenceException("The applicationName was null.");
            }
            var configuration = new ApplicationConfiguration { ApplicationName = applicationName, ApplicationType = ApplicationType.Server, SecurityConfiguration = new SecurityConfiguration { ApplicationCertificate = new CertificateIdentifier { StoreType = @"Windows", StorePath = @"CurrentUser\My", SubjectName = Utils.Format(@"CN={0}, DC={1}", applicationName, Dns.GetHostName()) }, TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Windows", StorePath = @"CurrentUser\TrustedPeople", }, NonceLength = 32, }, TransportConfigurations = new TransportConfigurationCollection(), TransportQuotas = new TransportQuotas(), ServerConfiguration = new ServerConfiguration { BaseAddresses = { Utils.Format(@"opc.tcp://{0}:51212", Dns.GetHostName()) }, SecurityPolicies = { new ServerSecurityPolicy { SecurityLevel = 0, SecurityMode = MessageSecurityMode.None, SecurityPolicyUri = SecurityPolicies.None }, new ServerSecurityPolicy { SecurityLevel = 1, SecurityMode = MessageSecurityMode.SignAndEncrypt, SecurityPolicyUri = SecurityPolicies.Basic128Rsa15 }, new ServerSecurityPolicy { SecurityLevel = 2, SecurityMode = MessageSecurityMode.SignAndEncrypt, SecurityPolicyUri = SecurityPolicies.Basic256 } } }, TraceConfiguration = new TraceConfiguration(), };
            configuration.Validate(ApplicationType.Server);
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
                    if (string.Equals(serverDomainName, "localhost", StringComparison.OrdinalIgnoreCase))
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
    }
}