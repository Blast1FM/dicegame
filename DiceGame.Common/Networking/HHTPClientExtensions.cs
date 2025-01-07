using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;

namespace DiceGame.Common.Networking;

public static class HHTPClientExtensions
{
    public static async Task<CommunicationResult<T>> ReceiveMessage<T>(this HHTPClient client)
    where T : BaseMessage
    {
        var packet = await client.ReceivePacket();
        try
        {
            // TODO check when deserialize can return null
            var message = JsonSerializer.Deserialize<T>(packet.Payload);
            CommunicationResult<T> result = new(message, packet);
            return result;
        }
        catch (JsonException e)
        {
            var errorMessage = $"Json exception deserializing packet message: {e.Message}";
            CommunicationResult<T> result = new(DataExchangeResult.Error,errorMessage,packet);
            return result;
        }
        catch (Exception e)
        {
            // use an actul logger you monkey
            System.Console.WriteLine($"Unknown exception: {e.Message}");
            throw;
        }


    }
}