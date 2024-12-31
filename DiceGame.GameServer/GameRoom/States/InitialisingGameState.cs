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
    private Dictionary<HHTPClient, int> _initialisationTriesPerConnection;
    private readonly int _maxInitialisationTries;
    public InitialisingGameState(GameRoomController controller, int maxInitTries)
    {
        _maxInitialisationTries = maxInitTries;
        _controller = controller;
        _requestRouter = new();
        List<Action<Packet,HHTPClient>> postHandlers = [HandleInitialiseRequest];
        List<Action<Packet,HHTPClient>> getHandlers = [HandleGetPlayerStatsRequest];
        _requestRouter.SetPostHandlers(postHandlers);
        _requestRouter.SetGetHandlers(getHandlers);
        _initialisationTriesPerConnection = [];
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

        await _controller._listener.StopListeningAsync();
    }

    public override async Task Update()
    {
        if (_controller._players.Count == _controller.MaxPlayerCount)
        {
            _controller._stateManager.ChangeGameState(this, "pregame");
        }
        else
        {
            // Handle initialisation packets
            foreach (var connection in _controller._unprocessedConnections)
            {
                if(!_initialisationTriesPerConnection.ContainsKey(connection))
                {
                    _initialisationTriesPerConnection[connection] = 0;
                }
                _initialisationTriesPerConnection[connection]++;

                if(_initialisationTriesPerConnection[connection] > _maxInitialisationTries)
                {
                    await _controller.RefuseClientConnection(connection, $"Exceeded retry limit: {_maxInitialisationTries}");
                    continue;
                }

                Packet maybeInitPacket = await connection.RecievePacket();
                try
                {
                    _requestRouter.RouteRequest(maybeInitPacket, connection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server error when routing request from {connection.Socket.RemoteEndPoint}:{e.Message}");
                    await connection.SendErrorPacket(maybeInitPacket, $"Failed to route request: {e.Message}");
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
        try
        {
            PlayerInfoMessage initMessage = JsonSerializer.Deserialize<PlayerInfoMessage>(packet.Payload) ?? throw new ArgumentNullException($"Player init message is null");

            Player player = new(initMessage.PlayerInfo, clientConnection);
            
            Packet okPacket = new(StatusCode.Ok,packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier,"");
            
            await clientConnection.SendPacket(okPacket);

            _controller._players.Add(player);
            _controller._unprocessedConnections.Remove(clientConnection);
        }
        catch (Exception e)
        {
           await clientConnection.SendErrorPacket(packet, $"Server error, could not initialise: {e.Message}");
        }
    }

    public async void HandleGetPlayerStatsRequest(Packet packet, HHTPClient clientConnection)
    {
        
    }
}
