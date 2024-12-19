using System;

namespace DiceGame.GameServer.GameRoom.States;

public class ChangeStateEventArgs : EventArgs
{
    public string NewGameState {get;set;}
    public GameState OldGameState {get;set;}
    public ChangeStateEventArgs(string newGameState, GameState oldGameState)
    {
        NewGameState = newGameState;
        OldGameState = oldGameState;
    }
}