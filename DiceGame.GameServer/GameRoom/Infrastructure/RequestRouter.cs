using DiceGame.Networking;
using DiceGame.Networking.Protocol;

namespace DiceGame.GameServer.GameRoom.Infrastructure;

public class RequestRouter
{
    public Dictionary<int, Action<Packet, HHTPClient>>? GetRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet, HHTPClient>>? PostRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet, HHTPClient>>? UpdateRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet, HHTPClient>>? DeleteRequestHandlers {get;set;}

    public RequestRouter
    (
        Dictionary<int, Action<Packet, HHTPClient>>? getRequestHandlers = null, 
        Dictionary<int, Action<Packet, HHTPClient>>? postRequestHandlers = null, 
        Dictionary<int, Action<Packet, HHTPClient>>? updateRequestHandlers = null, 
        Dictionary<int, Action<Packet, HHTPClient>>? deleteRequestHandlers = null
    )
    {
        GetRequestHandlers = getRequestHandlers;
        PostRequestHandlers = postRequestHandlers;
        UpdateRequestHandlers = updateRequestHandlers;
        DeleteRequestHandlers = deleteRequestHandlers;
    }

    public void RouteRequest(Packet packet, HHTPClient clientConnection)
    {
        int resourceIdentifier = packet.Header.ResourceIdentifier;
        var method = packet.Header.ProtocolMethod;

        switch (method)
        {
            case ProtocolMethod.GET:
            {
                if(GetRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: GET");

                if(GetRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet, clientConnection);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
            case ProtocolMethod.POST:
            {
                if(PostRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: POST");

                if(PostRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet, clientConnection);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
            case ProtocolMethod.UPDATE:
            {
                if(UpdateRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: UPDATE");

                if(UpdateRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet, clientConnection);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }

            case ProtocolMethod.DELETE:
            {
                if(DeleteRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: DELETE");

                if(DeleteRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet, clientConnection);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
        }
    }

}