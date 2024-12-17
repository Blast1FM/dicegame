namespace DiceGame.Common.Messages;

public class ErrorMessage : BaseMessage
{
    public int ErrorCode {get;set;}
    public string ErrorMessageText {get;set;}
}