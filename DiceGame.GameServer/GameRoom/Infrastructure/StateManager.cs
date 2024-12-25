using DiceGame.GameServer.GameRoom.States;

namespace DiceGame.GameServer.GameRoom.Infrastructure;

public class StateManager
{
    GameState? _currentState;
    Dictionary<string, GameState> _states;

    public StateManager(Dictionary<string, GameState> states)
    {
        _states = states;
        foreach(var state in _states)
        {
            state.Value.StateChanged += HandleChangeStateEvent;
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    void HandleChangeStateEvent(object? sender, ChangeStateEventArgs e)
    {
        // This could be fucky?
        ChangeGameState((GameState)sender! , e.NewGameState);
    }

    public void ChangeGameState(GameState currentState, string newStateName)
    {
        if (_currentState != currentState) throw new Exception($"State manager error: Manager's current state is not equal to current state recieved");

        bool gotNewGameState = _states.TryGetValue(newStateName.ToLower(), out GameState newGameState);

        if (!gotNewGameState) throw new Exception($"State manager error: failed to get new game state");

        if(currentState != null ) _currentState.Exit();
        
        newGameState.Enter();

        _currentState = newGameState;
    }
    
}
