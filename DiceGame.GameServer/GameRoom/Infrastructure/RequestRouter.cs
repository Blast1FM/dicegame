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
    public RequestRouter(){}
    
    public void SetGetHandlers(List<Action<Packet, HHTPClient>> getHandlers)
    {
        GetRequestHandlers ??= [];
        foreach( var (handler, index) in getHandlers.Select((handler, index) => (handler, index)))
        {
            GetRequestHandlers.Add(index,handler);
        }
    }

    public void SetPostHandlers(List<Action<Packet,HHTPClient>> postHandlers)
    {
        PostRequestHandlers ??= [];
        foreach( var (handler, index) in postHandlers.Select((handler, index) => (handler, index)))
        {
            PostRequestHandlers.Add(index,handler);
        }
    }

    public void SetUpdateHandlers(List<Action<Packet,HHTPClient>> updateHandlers)
    {
        UpdateRequestHandlers ??= [];
        foreach(var (handler,index) in updateHandlers.Select((handler,index) => (handler,index)))
        {
            UpdateRequestHandlers.Add(index,handler);
        }
    }

    public void SetDeleteHandlers(List<Action<Packet,HHTPClient>> deleteHandlers)
    {
        DeleteRequestHandlers ??= [];
        foreach (var (handler,index) in deleteHandlers.Select((handler,index)=>(handler,index)))
        {
            DeleteRequestHandlers.Add(index,handler);
        }
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