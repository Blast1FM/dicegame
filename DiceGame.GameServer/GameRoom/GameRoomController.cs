using System.Net.Sockets;
using DiceGame.GameServer.GameRoom;
using DiceGame.GameServer.GameRoom.States;
using DiceGame.Networking;

namespace DiceGame.GameServer;

public class GameRoomController
{
    public StateManager _stateManager;
    public List<Player> _players;
    public List<HHTPClient> _unprocessedConnections;
    public HHTPListener _listener;
    // TODO Actually create states here instead of having them passed into the constructor, this is temporary to make the warnings fuck off
    public GameRoomController(Dictionary<string, GameState> states, List<HHTPClient> connections)
    {
        _players = new(3);
        _listener = new HHTPListener();
        _unprocessedConnections = connections;
        _stateManager = new StateManager(states);

    }
    public async void Run()
    {
        _ = Task.Run(MonitorConnections);

        while(true)
        {
            _stateManager.Update();
        }
    }

    #region Player disconnect reconnect stuff
    public event EventHandler<Player>? PlayerDisconnected;
    public event EventHandler<Player>? PlayerReconnected;

    private void OnPlayerDisconnected(Player player)
    {
        PlayerDisconnected?.Invoke(this, player);
    }
    private void OnPlayerReconnected(Player player)
    {
        PlayerReconnected?.Invoke(this, player);
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
    private async Task MonitorConnections()
    {
        while (true)
        {
            foreach (var player in _players)
            {
                if (!IsClientConnected(player.HHTPClientConnection))
                {
                    if (player.IsConnected)
                    {
                        // Mark as disconnected
                        player.IsConnected = false;
                        player.LastSeen = DateTime.UtcNow;
                        OnPlayerDisconnected(player);
                        Console.WriteLine($"Client disconnected: {player.PlayerInfo.Guid}:{player.PlayerInfo.Name}");
                        
                        _unprocessedConnections.Add(player.HHTPClientConnection);
                    }
                }
            }
            await Task.Delay(5000);
        }
    }
    public void HandleReconnection(HHTPClient client)
    {
        var connection = _unprocessedConnections.Where(c => c.Socket.RemoteEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault();
        if (connection != null)
        {
            var player = _players.Where(c => c.HHTPClientConnection.Socket.RemoteEndPoint == client.Socket.RemoteEndPoint).FirstOrDefault()!;
            player.IsConnected = true;
            player.LastSeen = DateTime.UtcNow;
            player.HHTPClientConnection = client;
            Console.WriteLine($"Client reconnected: {player.PlayerInfo.Guid}:{player.PlayerInfo.Name}");
        }
        else
        {
            // We do not accept new clients here
        }
    }

    #endregion

}
