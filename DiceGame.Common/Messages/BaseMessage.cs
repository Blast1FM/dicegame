namespace DiceGame.Common.Messages;

/// <summary>
/// Base message class every message inherits from
/// </summary>
public class BaseMessage
{
    public DateTime CreatedAt {get; set;} = DateTime.Now;
}
