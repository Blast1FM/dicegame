using System;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

public static class PacketExtensions
{
    public static bool TryExtractMessageFromPacket<T>(this Packet packet, out T message)
        where T : BaseMessage
    {
        bool result = false;
        try
        {
            message = JsonSerializer.Deserialize<T>(packet.Payload);
            result = true;
        }
        catch (JsonException e)
        {
            System.Console.WriteLine($"Json exception:{e.Message}");
            throw;
        }

        return result;
    }
}
