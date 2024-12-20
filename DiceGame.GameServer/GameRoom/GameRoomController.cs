using DiceGame.GameServer.GameRoom;
using DiceGame.GameServer.GameRoom.States;
using DiceGame.Networking;

namespace DiceGame.GameServer;

public class GameRoomController
{
    public StateManager _stateManager;
    public List<Player>? _players;
    public List<HHTPClient> _connections;
    // TODO Actually create states here instead of having them passed into the constructor, this is temporary to make the warnings fuck off
    public GameRoomController(Dictionary<string, GameState> states, List<HHTPClient> connections)
    {
        _players = null;
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
}
