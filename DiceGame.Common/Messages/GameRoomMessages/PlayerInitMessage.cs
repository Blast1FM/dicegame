using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class PlayerInitMessage : BaseMessage
{
    public Player Player {get; private set;}
    public decimal RequestedPayout {get;private set;}
}