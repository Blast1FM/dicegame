using DiceGame.Networking;
using DiceGame.Networking.Protocol;

namespace DiceGame.GameServer.GameRoom.States;

public class InitialisingGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    public InitialisingGameState(GameRoomController controller)
    {
        _controller = controller;

        Dictionary<int, Action<Packet>> postRequestHandlers = new()
        {
            [0] = HandleInitialiseRequest
        };

        _requestRouter = new RequestRouter(null, postRequestHandlers, null, null);

    }
    public async override void Enter()
    {
        // Handle initialisation packets
        foreach (var connection in _controller._connections)
        {
            Packet maybeInitPacket = await connection.RecievePacket();
            try
            {
                _requestRouter.RouteRequest(maybeInitPacket);
            }
            catch (Exception e)
            {

            }

        }
    }

    public override void Exit()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }

    public void HandleInitialiseRequest(Packet packet)
    {
        string message = packet.Payload;

        // TODO Deserialise json into an init player message
        // TODO create Player object for this player, associate a HHTP client with it
        
    }
}
