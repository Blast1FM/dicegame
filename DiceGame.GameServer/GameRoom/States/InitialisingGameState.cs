using System.Text.Json;
using DiceGame.Common.Messages;
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

        Dictionary<int, Action<Packet, HHTPClient>> postRequestHandlers = new()
        {
            [0] = HandleInitialiseRequest
        };

        _requestRouter = new RequestRouter(null, postRequestHandlers);

    }
    public async override void Enter()
    {
        // TODO this should actually just have the listener stuff?
    }

    public async override void Exit()
    {
        throw new NotImplementedException();
    }

    // TODO Place the listener stuff here, or maybe not? Have it in a separate method? I'm losing my mind here
    public override async void Update()
    {
        // Handle initialisation packets
        // This will cause long connections for clients, while one client is lagging
        foreach (var connection in _controller._connections)
        {
            Packet maybeInitPacket = await connection.RecievePacket();
            try
            {
                _requestRouter.RouteRequest(maybeInitPacket, connection);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error:{e.Message}");
                string messagePayload = e.Message;

                Packet errorPacket = new
                (
                    StatusCode.Error, 
                    maybeInitPacket.Header.ProtocolMethod, 
                    maybeInitPacket.Header.ResourceIdentifier, 
                    messagePayload
                );

                await connection.SendPacket(errorPacket);

                throw;
            }

        }
    }

    // TODO write a unit test for this
    public async void HandleInitialiseRequest(Packet packet, HHTPClient clientConnection)
    {
        // TODO Deserialise json into an init player message
        PlayerInitMessage initMessage = JsonSerializer.Deserialize<PlayerInitMessage>(packet.Payload) ?? throw new ArgumentNullException($"Player init message is null");

        // TODO create Player object for this player, associate a HHTP client with it
        Player player = new(initMessage.PlayerInfo, clientConnection, initMessage.RequestedPayout);

        _controller._players.Add(player);

        PacketHeader okPacketHeader = new
                (
                    ProtocolVersion.V1, 
                    StatusCode.Ok,
                    packet.Header.ProtocolMethod,
                    packet.Header.ResourceIdentifier, 
                    0
                );
        
        await clientConnection.SendPacket(new Packet(okPacketHeader, ""));

    }
}
