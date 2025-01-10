using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class RandomNumberMessage(int number) : BaseMessage
{
    public int Number {get;set;} = number;
}
