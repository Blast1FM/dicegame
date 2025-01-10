namespace DiceGame.Common.Messages;

/// <summary>
/// Basic error message class used to pass arbitrary error messages
/// </summary>
public class ErrorMessage : BaseMessage
{
    public string ErrorMessageText {get;set;}
    public ErrorMessage(string errorMessageText)
    {
        ErrorMessageText = errorMessageText;
    }

}