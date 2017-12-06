// Copyright (c) 2014 Converter Systems LLC

using System.Threading.Tasks;
using Opc.Ua;

namespace ConverterSystems.Workstation.Services
{
    public partial class UaSession
    {
        public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest createSessionRequest)
        {
            UpdateRequestHeader(createSessionRequest, true, "CreateSession");
            CreateSessionResponse createSessionResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, createSessionRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    createSessionResponse = (CreateSessionResponse) serviceResponse;
                }
                else
                {
                    var createSessionResponseMessage = await Task<CreateSessionResponseMessage>.Factory.FromAsync(InnerChannel.BeginCreateSession, InnerChannel.EndCreateSession, new CreateSessionMessage(createSessionRequest), null).ConfigureAwait(false);
                    if (createSessionResponseMessage == null || createSessionResponseMessage.CreateSessionResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    createSessionResponse = createSessionResponseMessage.CreateSessionResponse;
                    ValidateResponse(createSessionResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(createSessionRequest, createSessionResponse, "CreateSession");
            }
            return createSessionResponse;
        }

        public async Task<ActivateSessionResponse> ActivateSessionAsync(ActivateSessionRequest activateSessionRequest)
        {
            UpdateRequestHeader(activateSessionRequest, true, "ActivateSession");
            ActivateSessionResponse activateSessionResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, activateSessionRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    activateSessionResponse = (ActivateSessionResponse) serviceResponse;
                }
                else
                {
                    var activateSessionResponseMessage = await Task<ActivateSessionResponseMessage>.Factory.FromAsync(InnerChannel.BeginActivateSession, InnerChannel.EndActivateSession, new ActivateSessionMessage(activateSessionRequest), null).ConfigureAwait(false);
                    if (activateSessionResponseMessage == null || activateSessionResponseMessage.ActivateSessionResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    activateSessionResponse = activateSessionResponseMessage.ActivateSessionResponse;
                    ValidateResponse(activateSessionResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(activateSessionRequest, activateSessionResponse, "ActivateSession");
            }
            return activateSessionResponse;
        }

        public async Task<ReadResponse> ReadAsync(ReadRequest readRequest)
        {
            UpdateRequestHeader(readRequest, true, "Read");
            ReadResponse readResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, readRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    readResponse = (ReadResponse) serviceResponse;
                }
                else
                {
                    var readResponseMessage = await Task<ReadResponseMessage>.Factory.FromAsync(InnerChannel.BeginRead, InnerChannel.EndRead, new ReadMessage(readRequest), null).ConfigureAwait(false);
                    if (readResponseMessage == null || readResponseMessage.ReadResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    readResponse = readResponseMessage.ReadResponse;
                    ValidateResponse(readResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(readRequest, readResponse, "Read");
            }
            return readResponse;
        }

        public async Task<WriteResponse> WriteAsync(WriteRequest writeRequest)
        {
            UpdateRequestHeader(writeRequest, true, "Write");
            WriteResponse writeResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, writeRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    writeResponse = (WriteResponse) serviceResponse;
                }
                else
                {
                    var writeResponseMessage = await Task<WriteResponseMessage>.Factory.FromAsync(InnerChannel.BeginWrite, InnerChannel.EndWrite, new WriteMessage(writeRequest), null).ConfigureAwait(false);
                    if (writeResponseMessage == null || writeResponseMessage.WriteResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    writeResponse = writeResponseMessage.WriteResponse;
                    ValidateResponse(writeResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(writeRequest, writeResponse, "Write");
            }
            return writeResponse;
        }

        public async Task<CallResponse> CallAsync(CallRequest callRequest)
        {
            UpdateRequestHeader(callRequest, true, "Call");
            CallResponse callResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, callRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    callResponse = (CallResponse) serviceResponse;
                }
                else
                {
                    var callResponseMessage = await Task<CallResponseMessage>.Factory.FromAsync(InnerChannel.BeginCall, InnerChannel.EndCall, new CallMessage(callRequest), null).ConfigureAwait(false);
                    if (callResponseMessage == null || callResponseMessage.CallResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    callResponse = callResponseMessage.CallResponse;
                    ValidateResponse(callResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(callRequest, callResponse, "Call");
            }
            return callResponse;
        }

        public async Task<BrowseResponse> BrowseAsync(BrowseRequest browseRequest)
        {
            UpdateRequestHeader(browseRequest, true, "Browse");
            BrowseResponse browseResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, browseRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    browseResponse = (BrowseResponse) serviceResponse;
                }
                else
                {
                    var browseResponseMessage = await Task<BrowseResponseMessage>.Factory.FromAsync(InnerChannel.BeginBrowse, InnerChannel.EndBrowse, new BrowseMessage(browseRequest), null).ConfigureAwait(false);
                    if (browseResponseMessage == null || browseResponseMessage.BrowseResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    browseResponse = browseResponseMessage.BrowseResponse;
                    ValidateResponse(browseResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(browseRequest, browseResponse, "Browse");
            }
            return browseResponse;
        }

        public async Task<BrowseNextResponse> BrowseNextAsync(BrowseNextRequest browseNextRequest)
        {
            UpdateRequestHeader(browseNextRequest, true, "BrowseNext");
            BrowseNextResponse browseNextResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, browseNextRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    browseNextResponse = (BrowseNextResponse) serviceResponse;
                }
                else
                {
                    var browseNextResponseMessage = await Task<BrowseNextResponseMessage>.Factory.FromAsync(InnerChannel.BeginBrowseNext, InnerChannel.EndBrowseNext, new BrowseNextMessage(browseNextRequest), null).ConfigureAwait(false);
                    if (browseNextResponseMessage == null || browseNextResponseMessage.BrowseNextResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    browseNextResponse = browseNextResponseMessage.BrowseNextResponse;
                    ValidateResponse(browseNextResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(browseNextRequest, browseNextResponse, "BrowseNext");
            }
            return browseNextResponse;
        }

        public async Task<CloseSessionResponse> CloseSessionAsync(CloseSessionRequest closeSessionRequest)
        {
            UpdateRequestHeader(closeSessionRequest, true, "CloseSession");
            CloseSessionResponse closeSessionResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, closeSessionRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    closeSessionResponse = (CloseSessionResponse) serviceResponse;
                }
                else
                {
                    var closeSessionResponseMessage = await Task<CloseSessionResponseMessage>.Factory.FromAsync(InnerChannel.BeginCloseSession, InnerChannel.EndCloseSession, new CloseSessionMessage(closeSessionRequest), null).ConfigureAwait(false);
                    if (closeSessionResponseMessage == null || closeSessionResponseMessage.CloseSessionResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    closeSessionResponse = closeSessionResponseMessage.CloseSessionResponse;
                    ValidateResponse(closeSessionResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(closeSessionRequest, closeSessionResponse, "CloseSession");
            }
            return closeSessionResponse;
        }

        public async Task<TranslateBrowsePathsToNodeIdsResponse> TranslateBrowsePathsToNodeIdsAsync(TranslateBrowsePathsToNodeIdsRequest translateRequest)
        {
            UpdateRequestHeader(translateRequest, true, "TranslateBrowsePathsToNodeIds");
            TranslateBrowsePathsToNodeIdsResponse translateResponse = null;
            try
            {
                if (UseTransportChannel)
                {
                    var serviceResponse = await Task<IServiceResponse>.Factory.FromAsync(TransportChannel.BeginSendRequest, TransportChannel.EndSendRequest, translateRequest, null).ConfigureAwait(false);
                    if (serviceResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    ValidateResponse(serviceResponse.ResponseHeader);
                    translateResponse = (TranslateBrowsePathsToNodeIdsResponse) serviceResponse;
                }
                else
                {
                    var browseResponseMessage = await Task<TranslateBrowsePathsToNodeIdsResponseMessage>.Factory.FromAsync(InnerChannel.BeginTranslateBrowsePathsToNodeIds, InnerChannel.EndTranslateBrowsePathsToNodeIds, new TranslateBrowsePathsToNodeIdsMessage(translateRequest), null).ConfigureAwait(false);
                    if (browseResponseMessage == null || browseResponseMessage.TranslateBrowsePathsToNodeIdsResponse == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadUnknownResponse);
                    }
                    translateResponse = browseResponseMessage.TranslateBrowsePathsToNodeIdsResponse;
                    ValidateResponse(translateResponse.ResponseHeader);
                }
            }
            finally
            {
                RequestCompleted(translateRequest, translateResponse, "TranslateBrowsePathsToNodeIds");
            }
            return translateResponse;
        }
    }
}