using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class ReadyMessage : BaseMessage
{
    public PlayerInfo Player {get;set;}
    public bool Ready { get; set; }
}