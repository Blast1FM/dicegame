using System.Net.Sockets;
using DiceGame.GameServer.GameRoom;
using DiceGame.GameServer.GameRoom.States;
using DiceGame.Networking;

namespace DiceGame.GameServer;

public class GameRoomController
{
    public StateManager _stateManager;
    public List<Player> _players;
    public List<HHTPClient> _connections;
    // TODO Actually create states here instead of having them passed into the constructor, this is temporary to make the warnings fuck off
    public GameRoomController(Dictionary<string, GameState> states, List<HHTPClient> connections)
    {
        _players = new(3);
        _connections = connections;
        _stateManager = new StateManager(states);
    }
    public async void Run()
    {
        while(true)
        {
            _stateManager.Update();
        }
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

    // TODO Start this task after the game is finished initialising
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
                        Console.WriteLine($"Client disconnected: {player.PlayerInfo.Guid}:{player.PlayerInfo.Name}");
                    }
                }
            }
            await Task.Delay(5000);
        }
    }


}
