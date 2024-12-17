using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class MainGameRollMessage : BaseMessage
{
    public Player Player {get; private set;}
    public List<int> Rolls {get; set;}
}