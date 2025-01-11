using DiceGame.Common.Messages;

namespace Demo.Common.Messages;

public class RandomNumberMessage : BaseMessage
{
    public Guid? SessionToken {get;set;}
    public int Number {get;set;}
    public RandomNumberMessage(Guid sessionToken, int number)
    {
        SessionToken = sessionToken;
        Number = number;
    }
}
