using System.Dynamic;
using System.Net.Sockets;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;

namespace DiceGame.GameServer.GameRoom.States;

public class InitialisingGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    private HHTPListener _listener;
    int MaxPlayers {get; set;} = 3;
    public InitialisingGameState(GameRoomController controller)
    {
        _controller = controller;

        Dictionary<int, Action<Packet, HHTPClient>> postRequestHandlers = new()
        {
            [0] = HandleInitialiseRequest
        };

        _listener = new HHTPListener();

        _requestRouter = new RequestRouter(null, postRequestHandlers);

    }
    public async override void Enter()
    {
        _controller.PlayerDisconnected += HandlePlayerDisconnect;
        _listener.ClientConnected += HandleClientConnected;

        _ = Task.Run(_listener.Listen);

    }

    public async override void Exit()
    {
        _controller.PlayerDisconnected -= HandlePlayerDisconnect;
        _listener.ClientConnected -= HandleClientConnected;
        // TODO cancel listen task with a cancellation token
        throw new NotImplementedException();
    }

    public override async void Update()
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

    private void HandlePlayerDisconnect(object? sender, Player player)
    {
        throw new NotImplementedException();
    }

    private void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        // Wrap the accepted socket in an HHTPClient object
        var client = new HHTPClient(e.clientSocket);

        // Add the new client connection to the controller's list
        _controller._unprocessedConnections.Add(client);
        Console.WriteLine($"New client connected from: {client.Socket.RemoteEndPoint}");
    }
    // TODO write a unit test for this
    public async void HandleInitialiseRequest(Packet packet, HHTPClient clientConnection)
    {
        // TODO Deserialise json into an init player message
        PlayerInitMessage initMessage = JsonSerializer.Deserialize<PlayerInitMessage>(packet.Payload) ?? throw new ArgumentNullException($"Player init message is null");

        // TODO create Player object for this player, associate a HHTP client with it
        Player player = new(initMessage.PlayerInfo, clientConnection, initMessage.RequestedPayout);
        
        Packet okPacket = new(StatusCode.Ok,packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier,"");
        
        await clientConnection.SendPacket(okPacket);

        _controller._players.Add(player);
        _controller._unprocessedConnections.Remove(clientConnection);

    }
}
