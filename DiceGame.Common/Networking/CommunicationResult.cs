using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

public class CommunicationResult<T> : ICommunicationResult<T> where T : BaseMessage
{
    public DataExchangeResult Result { get;set; }
    public T? Message { get;set; }
    public Packet? Packet { get;set; }
    public string? ErrorMessage { get;set; }

    public CommunicationResult(T message, Packet packet)
    {
        Result = DataExchangeResult.Ok;
        Message = message;
        Packet = packet;
    }
    public CommunicationResult(DataExchangeResult error, string errorMessage, Packet packet)
    {
        Result = error;
        ErrorMessage = errorMessage;
        Packet = packet;
    }
}
