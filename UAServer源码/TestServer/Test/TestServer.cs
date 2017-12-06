// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Com;
using Opc.Ua.Server;

namespace ConverterSystems.Test
{
    public class TestServer : StandardServer
    {
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            ComUtils.InitializeSecurity();
            Console.WriteLine("The server is starting at {0}", configuration.ServerConfiguration.BaseAddresses[0]);
            base.OnServerStarting(configuration);
        }

        protected override void OnServerStopping()
        {
            Console.WriteLine("The Server is stopping.");
            base.OnServerStopping();
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            return new MasterNodeManager(server, configuration, null, new List<INodeManager> { new TestNodeManager(server, configuration), }.ToArray());
        }

        protected override ServerProperties LoadServerProperties()
        {
            return new ServerProperties { ManufacturerName = "Converter Systems LLC", ProductName = "TestServer", ProductUri = @"http://opcuaservicesforwpf.codeplex.com/TestServer", SoftwareVersion = Utils.GetAssemblySoftwareVersion(), BuildNumber = Utils.GetAssemblyBuildNumber(), BuildDate = Utils.GetAssemblyTimestamp() };
        }

        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);
            server.SessionManager.SessionActivated += (session, reason) =>
            {
                var endpoint = SecureChannelContext.Current.EndpointDescription;
                Console.WriteLine("Activated Session Id; {0}, SessionName: {1}\n from Client: {2}\n via Endpoint: {3}\n Transport: {4}\n Security: {5}", session.SessionDiagnostics.SessionId, session.SessionDiagnostics.SessionName, session.SessionDiagnostics.ClientDescription.ApplicationName, endpoint.EndpointUrl, endpoint.TransportProfileUri, endpoint.SecurityPolicyUri);
            };
            server.SessionManager.SessionClosing += (session, reason) => Console.WriteLine("Closing Session Id: {0}", session.SessionDiagnostics.SessionId);
            server.SubscriptionManager.SubscriptionCreated += (subscription, deleted) => Console.WriteLine("Created Subscription Id: {0}", subscription.Diagnostics.SubscriptionId);
            server.SubscriptionManager.SubscriptionDeleted += (subscription, deleted) => Console.WriteLine("Deleted Subscription Id: {0}", subscription.Diagnostics.SubscriptionId);
        }
    }
}