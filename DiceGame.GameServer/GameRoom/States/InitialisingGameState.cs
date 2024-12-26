using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.GameServer.GameRoom.Infrastructure;

namespace DiceGame.GameServer.GameRoom.States;

public class InitialisingGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    int MaxPlayers {get; set;} = 3;
    public InitialisingGameState(GameRoomController controller)
    {
        _controller = controller;
        _requestRouter = new();
        List<Action<Packet,HHTPClient>> postHandlers = [HandleInitialiseRequest];
        _requestRouter.SetPostHandlers(postHandlers);
    }
    public async override Task Enter()
    {
        _controller.PlayerDisconnected += HandlePlayerDisconnect;
        _controller._listener.ClientConnected += HandleClientConnected;
    }

    public async override Task Exit()
    {
        _controller.PlayerDisconnected -= HandlePlayerDisconnect;
        _controller._listener.ClientConnected -= HandleClientConnected;
        // TODO cancel listen task with a cancellation token
        throw new NotImplementedException();
    }

    public override async Task Update()
    {
        
        if (_controller._players.Count == MaxPlayers)
        {
            _controller._stateManager.ChangeGameState(this, "pregame");
        }
        else
        {
            // Handle initialisation packets
            foreach (var connection in _controller._unprocessedConnections)
            {
                Packet maybeInitPacket = await connection.RecievePacket();
                try
                {
                    _requestRouter.RouteRequest(maybeInitPacket, connection);
                }
                // TODO test for more specific exception
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
        
    }

    private void HandlePlayerDisconnect(object? sender, PlayerConnectionEventArgs e)
    {
        
    }

    private void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        
    }
    // TODO write a unit test for this
    public async void HandleInitialiseRequest(Packet packet, HHTPClient clientConnection)
    {
        PlayerInitMessage initMessage = JsonSerializer.Deserialize<PlayerInitMessage>(packet.Payload) ?? throw new ArgumentNullException($"Player init message is null");

        Player player = new(initMessage.PlayerInfo, clientConnection, initMessage.RequestedPayout);
        
        Packet okPacket = new(StatusCode.Ok,packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier,"");
        
        await clientConnection.SendPacket(okPacket);

        _controller._players.Add(player);
        _controller._unprocessedConnections.Remove(clientConnection);

    }
}
