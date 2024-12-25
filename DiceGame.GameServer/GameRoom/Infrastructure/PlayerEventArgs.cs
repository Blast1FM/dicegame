namespace DiceGame.GameServer.GameRoom.Infrastructure;
public class PlayerConnectionEventArgs : EventArgs
{
    public Player Player { get; set; }
    
    public PlayerConnectionEventArgs(Player player)
    {
        Player = player;
    }
}
