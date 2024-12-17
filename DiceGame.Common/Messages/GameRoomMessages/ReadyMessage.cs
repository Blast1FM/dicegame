using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class ReadyMessage : BaseMessage
{
    public Player Player {get;set;}
    public bool Ready { get; set; }
}
