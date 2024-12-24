using System;

namespace DiceGame.GameServer.GameRoom;

public class PlayerConnectionEventArgs : EventArgs
{
    public Player Player { get; set; }
    
    public PlayerConnectionEventArgs(Player player)
    {
        Player = player;
    }
}
