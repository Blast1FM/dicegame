using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class RandomNumberMessage : BaseMessage
{
    public int Number {get;set;}
    public RandomNumberMessage(int number)
    {
        Number = number;
    }
    public RandomNumberMessage(){}
}
