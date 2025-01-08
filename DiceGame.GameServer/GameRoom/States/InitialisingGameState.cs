using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.GameServer.GameRoom.Infrastructure;
using DiceGame.Common.Messages.GameRoomMessages;
using DiceGame.Common.Networking;

namespace DiceGame.GameServer.GameRoom.States;

public class InitialisingGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    private Dictionary<HHTPClient, int> _initialisationTriesPerConnection;
    private readonly int _maxInitialisationTries;
    private readonly AutoResetEvent allStateChangeMessagesSent = new(false);
    private int _stateChangeMessagesSent = 0;
    public InitialisingGameState(GameRoomController controller, int maxInitTries)
    {
        _maxInitialisationTries = maxInitTries;
        _controller = controller;
        _requestRouter = new();
        List<Action<Packet,HHTPClient>> postHandlers = [HandleInitialiseRequest];
        List<Action<Packet,HHTPClient>> getHandlers = [HandleGetConnectedPlayersRequest, HandleGetStateStatusRequest];
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
        // This will break if someone disconnects lol
        if (_controller._players.Count == _controller.MaxPlayerCount)
        {
            allStateChangeMessagesSent.WaitOne();
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

                Packet maybeInitPacket = await connection.ReceivePacket();
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
            // Rollbacks shouldn't be neccessary
            await clientConnection.SendErrorPacket(packet, $"Server error, could not initialise: {e.Message}");
        }
    }

    public async void HandleGetConnectedPlayersRequest(Packet packet, HHTPClient clientConnection)
    {
        try
        {
            ConnectedPlayerListMessage connectedPlayersDTO = new(_controller._players.Select(p => p.PlayerInfo).ToList());
            string message = JsonSerializer.Serialize(connectedPlayersDTO); 
            Packet response = new(StatusCode.Ok, packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier, message);
            await clientConnection.SendPacket(response);
        }
        catch (JsonException e)
        {
            Packet errorPacket = new(StatusCode.Error, packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier, $"Serialisation error: {e.Message}");
        }
        catch (Exception e)
        {
            Packet errorPacket = new(StatusCode.Error, packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier, $"Unhandled server error:{e.Message}");
        }
    }

    public async void HandleGetStateStatusRequest(Packet packet, HHTPClient clientConnection)
    {
        bool changing = _controller._players.Count == _controller.MaxPlayerCount;
        if(changing)
        {
            GameStateStatusMessage message = new("initialising", "pregame");
            bool success = await clientConnection.SendMessage<GameStateStatusMessage>(message, packet);
            // Ideally retry logic but cba
            if (success) _stateChangeMessagesSent++;
            if(_stateChangeMessagesSent == _controller.MaxPlayerCount)
            {
                allStateChangeMessagesSent.Set();
            }
        } else
        {
            GameStateStatusMessage message = new("initialising", "initialising");
            bool success = await clientConnection.SendMessage<GameStateStatusMessage>(message, packet);
        }
    }
}
