using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class PregameRollMessage : BaseMessage
{
    public PlayerInfo Player {get;set;}
    public int Roll {get;set;}

    public PregameRollMessage(PlayerInfo player, int roll)
    {
        Player = player;
        Roll = roll;
    }
}