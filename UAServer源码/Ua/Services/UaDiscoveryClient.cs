// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Threading.Tasks;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    public class UaDiscoveryClient : DiscoveryClient
    {
        public UaDiscoveryClient(ITransportChannel channel) : base(channel)
        {
        }

        public new static UaDiscoveryClient Create(Uri discoveryUrl)
        {
            var endpointConfiguration = EndpointConfiguration.Create();
            var channel = DiscoveryChannel.Create(discoveryUrl, endpointConfiguration, new ServiceMessageContext());
            return new UaDiscoveryClient(channel);
        }

        public new static UaDiscoveryClient Create(Uri discoveryUrl, EndpointConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = EndpointConfiguration.Create();
            }
            var channel = DiscoveryChannel.Create(discoveryUrl, configuration, new ServiceMessageContext());
            return new UaDiscoveryClient(channel);
        }

        public new static UaDiscoveryClient Create(Uri discoveryUrl, BindingFactory bindingFactory, EndpointConfiguration configuration)
        {
            if (discoveryUrl == null)
            {
                throw new ArgumentNullException("discoveryUrl");
            }
            if (bindingFactory == null)
            {
                bindingFactory = BindingFactory.Default;
            }
            if (configuration == null)
            {
                configuration = EndpointConfiguration.Create();
            }
            var channel = DiscoveryChannel.Create(discoveryUrl, bindingFactory, configuration, new ServiceMessageContext());
            return new UaDiscoveryClient(channel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            base.Dispose(disposing);
        }

        public async Task<FindServersResponse> FindServersAsync(FindServersRequest findServersRequest)
        {
            UpdateRequestHeader(findServersRequest, true, "FindServers");
            FindServersResponse findServersResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, findServersRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    findServersResponse = (FindServersResponse) serviceResponse;
                }
                else
                {
                    var findServersResponseMessage = await Task<FindServersResponseMessage>.Factory.FromAsync(InnerChannel.BeginFindServers, InnerChannel.EndFindServers, new FindServersMessage(findServersRequest), null).ConfigureAwait(false);
                    if (findServersResponseMessage == null || findServersResponseMessage.FindServersResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    findServersResponse = findServersResponseMessage.FindServersResponse;
                    ValidateResponse(findServersResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(findServersRequest, findServersResponse, "FindServers");
            }
            return findServersResponse;
        }

        public async Task<GetEndpointsResponse> GetEndpointsAsync(GetEndpointsRequest getEndpointsRequest)
        {
            UpdateRequestHeader(getEndpointsRequest, true, "GetEndpoints");
            GetEndpointsResponse getEndpointsResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, getEndpointsRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    getEndpointsResponse = (GetEndpointsResponse) serviceResponse;
                }
                else
                {
                    var getEndpointsResponseMessage = await Task<GetEndpointsResponseMessage>.Factory.FromAsync(InnerChannel.BeginGetEndpoints, InnerChannel.EndGetEndpoints, new GetEndpointsMessage(getEndpointsRequest), null).ConfigureAwait(false);
                    if (getEndpointsResponseMessage == null || getEndpointsResponseMessage.GetEndpointsResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    getEndpointsResponse = getEndpointsResponseMessage.GetEndpointsResponse;
                    ValidateResponse(getEndpointsResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(getEndpointsRequest, getEndpointsResponse, "GetEndpoints");
            }
            return getEndpointsResponse;
        }
    }
}