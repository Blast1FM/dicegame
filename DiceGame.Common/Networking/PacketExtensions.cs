using System;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking.Protocol;

namespace DiceGame.Common.Networking;

/// <summary>
/// Convenience extension methods used to work with the Packet class
/// </summary>
public static class PacketExtensions
{
    /// <summary>
    /// Helper method used to deserialise a message of type T from a given packet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="packet"></param>
    /// <param name="message"></param>
    /// <returns></returns>
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
