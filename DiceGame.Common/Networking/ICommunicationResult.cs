using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

/// <summary>
/// Interface used for communication result
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommunicationResult<T>
    where T : BaseMessage
{
    public DataExchangeResult Result {get; set;}
    public string? ErrorMessage {get;set;}
    public T? Message {get; set;}
    public Packet Packet {get; set;}
}
