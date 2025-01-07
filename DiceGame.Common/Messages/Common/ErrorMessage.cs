namespace DiceGame.Common.Messages;

public class ErrorMessage : BaseMessage
{
    public string ErrorMessageText {get;set;}
    public ErrorMessage(string errorMessageText)
    {
        ErrorMessageText = errorMessageText;
    }

}