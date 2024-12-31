using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class GameResultMessage : BaseMessage
{
    public PlayerInfo Loser {get;set;}
    public decimal AmountOwed {get; set;}
    public GameResultMessage (PlayerInfo loser, decimal amountOwed)
    {
        Loser = loser;
        AmountOwed = amountOwed;
    }
}