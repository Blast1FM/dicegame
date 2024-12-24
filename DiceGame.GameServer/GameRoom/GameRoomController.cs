using System.Net.Sockets;
using System.Runtime.InteropServices.Marshalling;
using DiceGame.GameServer.GameRoom;
using DiceGame.GameServer.GameRoom.States;
using DiceGame.Networking;

namespace DiceGame.GameServer;

public class GameRoomController
{
    public StateManager _stateManager;
    public List<Player> _players;
    public List<HHTPClient> _unprocessedConnections;
    public List<Player> _disconnectedPlayers;
    public HHTPListener _listener;
    // TODO Actually create states here instead of having them passed into the constructor, this is temporary to make the warnings fuck off
    public GameRoomController(Dictionary<string, GameState> states, List<HHTPClient> connections)
    {
        _players = new(3);
        _disconnectedPlayers = new(3);
        _listener = new HHTPListener();
        _unprocessedConnections = connections;
        _stateManager = new StateManager(states);

    }
    public async void Run()
    {
        _listener.ClientConnected += HandleClientConnected;
        PlayerDisconnected += HandlePlayerDisconnect;
        _ = Task.Run(_listener.Listen);
        _ = Task.Run(MonitorPlayerConnections);

        while(true)
        {
            _stateManager.Update();
        }
    }

    public void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        _unprocessedConnections.Add(new HHTPClient(e.clientSocket));
    }

    #region Player disconnect reconnect stuff
    public event EventHandler<PlayerConnectionEventArgs>? PlayerDisconnected;
    public event EventHandler<PlayerConnectionEventArgs>? PlayerReconnected;

    public virtual void OnPlayerDisconnected(PlayerConnectionEventArgs e)
    {
        PlayerDisconnected?.Invoke(this, e);
    }
    public virtual void OnPlayerReconnected(PlayerConnectionEventArgs e)
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

    private void HandlePlayerDisconnect(object? sender, PlayerConnectionEventArgs e)
    {
        e.Player.IsConnected = false;
        e.Player.LastSeen = DateTime.UtcNow;
        Console.WriteLine($"Client disconnected: {e.Player.PlayerInfo.Guid}:{e.Player.PlayerInfo.Name}");
        _disconnectedPlayers.Add(e.Player);
    }

    // TODO this needs work
    public void HandlePlayerReconnect(object? sender, HHTPClient client)
    {

        var connection = _unprocessedConnections.Where(c => c.Socket.RemoteEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault();
        if (connection != null)
        {
            var player = _players.Where(c => c.HHTPClientConnection.Socket.RemoteEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault()!;
            player.IsConnected = true;
            player.LastSeen = DateTime.UtcNow;
            player.HHTPClientConnection = client;
            Console.WriteLine($"Client reconnected: {player.PlayerInfo.Guid}:{player.PlayerInfo.Name}");
            
            _unprocessedConnections.Remove(connection);
        }
        else
        {
            // We do not accept new clients here
        }
    }

    #endregion

}
