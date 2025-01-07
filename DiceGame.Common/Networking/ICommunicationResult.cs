using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

public interface ICommunicationResult<T>
    where T : BaseMessage
{
    public DataExchangeResult Result {get; set;}
    public string? ErrorMessage {get;set;}
    public T? Message {get; set;}
    public Packet? Packet {get; set;}
}
