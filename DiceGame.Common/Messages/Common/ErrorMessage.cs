namespace DiceGame.Common.Messages;

public class ErrorMessage : BaseMessage
{
    public string Message {get;set;}
    public ErrorMessage(string message)
    {
        Message = message;
    }
}