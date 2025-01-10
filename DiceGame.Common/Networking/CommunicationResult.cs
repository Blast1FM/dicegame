using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

/// <summary>
/// Implementation of a communication result, used in ReceiveMessage
/// Represents 2 outcomes - a succesful message exchange or an error
/// If successful - Result is set to DataExchangeResult.Ok and the message is set, while the ErrorMessage remains null
/// If an error occurs - Result is set to DataExchangeResult.Error and the ErrorMessage is set, while the Message property is null.
/// The packet property is set regardless, to allow inspection of the request packet.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CommunicationResult<T> : ICommunicationResult<T> where T : BaseMessage
{
    public DataExchangeResult Result { get;set; }
    public T? Message { get;set; }
    public Packet Packet { get;set; }
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
