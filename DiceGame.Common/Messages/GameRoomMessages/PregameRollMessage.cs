using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class PregameRollMessage : BaseMessage
{
    public Player Player {get;set;}
    public int Roll {get;set;}
}