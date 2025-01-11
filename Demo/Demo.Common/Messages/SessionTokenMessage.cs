using DiceGame.Common.Messages;

public class SessionTokenMessage : BaseMessage
{
    public Guid? SessionToken {get;set;}
    public SessionTokenMessage(Guid sessionToken)
    {
        SessionToken = sessionToken;
    }
    public SessionTokenMessage(){}
}