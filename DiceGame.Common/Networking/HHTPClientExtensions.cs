using System.Net.NetworkInformation;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

public static class HHTPClientExtensions
{
    public static async Task<Packet?> ReceivePacket(this HHTPClient client, TimeSpan timeout)
    {
        var receiveTask = client.ReceivePacket();
        if (await Task.WhenAny(receiveTask, Task.Delay(timeout)) == receiveTask)
        {
            return await receiveTask;
        }
        else
        {
            // Timeout 
            return null; 
        }
    }
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

    public static async Task<bool> SendMessage<T>(this HHTPClient client, T message, StatusCode statusCode, ProtocolMethod method, int resourceIdentifier)
        where T : BaseMessage
    {
        try
        {
            var payload = JsonSerializer.Serialize(message);
            var packet = new Packet(statusCode,method,resourceIdentifier,payload);
            bool success = await client.SendPacket(packet);
            return success;
        }
        catch(JsonException e)
        {
            System.Console.WriteLine($"Failed to serialize payload: {e.Message}");
            return false;
        }
        catch(Exception e)
        {
            System.Console.WriteLine($"Unhandled exception: {e.Message}");
            return false;
        }
    }
    /// <summary>
    /// Send a message of type T to the HHTPClient client, using the headerDonorPacket for its header info
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="message"></param>
    /// <param name="headerDonorPacket"></param>
    /// <returns></returns>
    public static async Task<bool> SendMessage<T>(this HHTPClient client, T message, Packet headerDonorPacket)
        where T : BaseMessage
    {
        try
        {
            var payload = JsonSerializer.Serialize(message);
            var outboundPacket = new Packet(headerDonorPacket.Header.StatusCode, headerDonorPacket.Header.ProtocolMethod,headerDonorPacket.Header.ResourceIdentifier,payload);
            bool success = await client.SendPacket(outboundPacket);
            return success;
        }
        catch(JsonException e)
        {
            System.Console.WriteLine($"Failed to serialize payload: {e.Message}");
            return false;
        }
        catch(Exception e)
        {
            System.Console.WriteLine($"Unhandled exception: {e.Message}");
            return false;
        }
    }
}