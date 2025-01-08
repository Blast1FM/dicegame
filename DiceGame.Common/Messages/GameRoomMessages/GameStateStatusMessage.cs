namespace DiceGame.Common.Messages.GameRoomMessages;

public class GameStateStatusMessage : BaseMessage
{
    public bool StateChanging {get; set;}
    public string CurrentGameState {get;set;}
    public string NextGameState {get;set;}
    public GameStateStatusMessage(bool stateChanging, string currentGameState, string nextGameState)
    {
        StateChanging = stateChanging;
        CurrentGameState = currentGameState;
        NextGameState = nextGameState;
    }
    public GameStateStatusMessage(string currentGameState, string nextGameState)
    {
        StateChanging = currentGameState != nextGameState;
        CurrentGameState = currentGameState;
        NextGameState = nextGameState;
    }
}
