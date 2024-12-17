using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class GameResultMessage : BaseMessage
{
    public Player Loser {get;set;}
    public decimal AmountOwed {get; set;}
}
