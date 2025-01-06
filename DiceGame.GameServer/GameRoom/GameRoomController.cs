using System.Net.Sockets;
using DiceGame.GameServer.GameRoom.Infrastructure;
using DiceGame.GameServer.GameRoom.States;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace DiceGame.GameServer;

public class GameRoomController
{
    public StateManager _stateManager;
    public List<Player> _players;
    public List<HHTPClient> _unprocessedConnections;
    public List<Player> _disconnectedPlayers;
    public HHTPListener _listener;
    public AutoResetEvent _handledClientConnected = new(false);
    public AutoResetEvent _handledPlayerDisconnected = new(false);
    public AutoResetEvent _handledPlayedReconnected = new(false);
    public int MaxPlayerCount {get;} = 3;
    // TODO Actually create states here instead of having them passed into the constructor, this is temporary to make the warnings fuck off
    public GameRoomController(Dictionary<string, GameState> states)
    {
        _unprocessedConnections = new(3);
        _listener = new HHTPListener();
        _players = new(3);
        _disconnectedPlayers = new(3);
        _stateManager = new StateManager(states);

    }
    public void Run()
    {
        _listener.ClientConnected += HandleClientConnected;
        PlayerDisconnected += HandlePlayerDisconnect;
        PlayerReconnected += HandlePlayerReconnect;
        _listener.StartListening();
        _ = Task.Run(MonitorPlayerConnections);

        while(true)
        {
            _stateManager.Update();
        }
    }

    public async void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        HHTPClient client = new HHTPClient(new SocketWrapper(e.ClientSocket));
        _unprocessedConnections.Add(client);

        if(_players.Count>=3)
        {
            await RefuseClientConnection(client, "Server is full");
            return;
        }

        if(_disconnectedPlayers.Count>0)
        {
            OnPlayerReconnected(new HHTPClient(new SocketWrapper(e.ClientSocket)));
            return;
        }

        // TODO Stop the listener if we're up to max players.
        Console.WriteLine($"New client connected from: {e.ClientSocket.RemoteEndPoint}");

        _handledClientConnected.Set();
    }

    #region Player disconnect reconnect stuff
    public event EventHandler<PlayerConnectionEventArgs>? PlayerDisconnected;
    public event EventHandler<HHTPClient>? PlayerReconnected;

    public virtual void OnPlayerDisconnected(PlayerConnectionEventArgs e)
    {
        PlayerDisconnected?.Invoke(this, e);
    }
    public virtual void OnPlayerReconnected(HHTPClient e)
    {
        PlayerReconnected?.Invoke(this, e);
    }

    private bool IsClientConnected(HHTPClient client)
    {
        try
        {
            return !(client.Socket.Poll(1000, SelectMode.SelectRead) && client.Socket.Available == 0);
        }
        catch (Exception)
        {
            return false;
        }
    }
    private async Task MonitorPlayerConnections()
    {
        while (true)
        {
            foreach (var player in _players)
            {
                if (!IsClientConnected(player.HHTPClientConnection))
                {
                    if (player.IsConnected)
                    {
                        OnPlayerDisconnected(new PlayerConnectionEventArgs(player));          
                    }
                }
            }
            await Task.Delay(5000);
        }
    }

    // TODO Start up the listener again
    private void HandlePlayerDisconnect(object? sender, PlayerConnectionEventArgs e)
    {
        e.Player.IsConnected = false;
        e.Player.LastSeen = DateTime.UtcNow;
        Console.WriteLine($"Client disconnected: {e.Player.PlayerInfo.Guid}:{e.Player.PlayerInfo.Name}");
        _disconnectedPlayers.Add(e.Player);
        _handledPlayerDisconnected.Set();
    }
    // TODO Stop the listener if we're back to max players
    public async void HandlePlayerReconnect(object? sender, HHTPClient client)
    {
        // NOTE - This socket lookup might fail if it gets disposed of, which it probably will - Keep in mind
        var connection = _unprocessedConnections.Where(c => c.Socket.RemoteEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault();
        if (connection != null)
        {
            var player = _players.Where(c => c.IPEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault()!;
            player.IsConnected = true;
            player.LastSeen = DateTime.UtcNow;
            player.HHTPClientConnection = client;
            Console.WriteLine($"Client reconnected: {player.PlayerInfo.Guid}:{player.PlayerInfo.Name}");
            _disconnectedPlayers.Remove(player);
            _unprocessedConnections.Remove(connection);
        }
        else
        {
            // We do not accept new clients here
            await RefuseClientConnection(client, "Server is full");
        }

        // TODO Think it over if it should be here
        _handledPlayedReconnected.Set();
    }

    public async Task RefuseClientConnection(HHTPClient client, string message)
    {
        await client.SendPacket(new Packet(StatusCode.Error, ProtocolMethod.GET, 0, $"Connection refused: {message}"));
        _unprocessedConnections.Remove(client);
        client.CloseConnection();
    }

    #endregion

}
