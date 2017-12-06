// Copyright (c) 2014 Converter Systems LLC

using System;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    internal sealed class NullTransportChannel : ITransportChannel
    {
        public void Initialize(Uri url, TransportChannelSettings settings)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object callbackData)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Reconnect()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReconnect(AsyncCallback callback, object callbackData)
        {
            throw new NotImplementedException();
        }

        public void EndReconnect(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object callbackData)
        {
            throw new NotImplementedException();
        }

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IServiceResponse SendRequest(IServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginSendRequest(IServiceRequest request, AsyncCallback callback, object callbackData)
        {
            throw new NotImplementedException();
        }

        public IServiceResponse EndSendRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public TransportChannelFeatures SupportedFeatures { get; private set; }
        public EndpointDescription EndpointDescription { get; private set; }
        public EndpointConfiguration EndpointConfiguration { get; private set; }
        public ServiceMessageContext MessageContext { get; private set; }
        public int OperationTimeout { get; set; }
    }
}