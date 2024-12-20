using DiceGame.Networking.Protocol;

namespace DiceGame.GameServer.GameRoom;

public class RequestRouter
{
    public Dictionary<int, Action<Packet>>? GetRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet>>? PostRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet>>? UpdateRequestHandlers {get;set;}
    public Dictionary<int, Action<Packet>>? DeleteRequestHandlers {get;set;}

    public RequestRouter
    (
        Dictionary<int, Action<Packet>>? getRequestHandlers, 
        Dictionary<int, Action<Packet>>? postRequestHandlers, 
        Dictionary<int, Action<Packet>>? updateRequestHandlers, 
        Dictionary<int, Action<Packet>>? deleteRequestHandlers
    )
    {
        GetRequestHandlers = getRequestHandlers;
        PostRequestHandlers = postRequestHandlers;
        UpdateRequestHandlers = updateRequestHandlers;
        DeleteRequestHandlers = deleteRequestHandlers;
    }

    public void RouteRequest(Packet packet)
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
                    handler(packet);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
            case ProtocolMethod.POST:
            {
                if(PostRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: POST");

                if(PostRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
            case ProtocolMethod.UPDATE:
            {
                if(UpdateRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: UPDATE");

                if(UpdateRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }

            case ProtocolMethod.DELETE:
            {
                if(DeleteRequestHandlers == null) throw new ArgumentException($"Unsupported request method for current service: DELETE");

                if(DeleteRequestHandlers.TryGetValue(resourceIdentifier, out var handler))
                {
                    handler(packet);
                }
                else throw new ArgumentException($"Resource not found: {resourceIdentifier}");
                break;
            }
        }
    }

}