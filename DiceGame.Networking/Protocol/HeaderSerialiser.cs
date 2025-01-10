using System.Buffers.Binary;

namespace DiceGame.Networking.Protocol;

/// <summary>
/// Class used to manage header serialisation/deserialisation to and from binary
/// </summary>
public class HeaderSerialiser
{
    public static int HeaderSize {get;} = 4;
    /// <summary>
    /// Will try to deserialise the header from the next 4 bytes in the stream. Will advance the seeker head
    /// </summary>
    /// <param name="dataStream"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static PacketHeader DeserialiseHeader(ref Stream dataStream)
    {
        byte[] potentialHeader = new byte[4];
        int bytesRead = dataStream.Read(potentialHeader,0,4);
        if(bytesRead != 4) throw new Exception($"Could not read 4 bytes for header, got {bytesRead}");

        return BuildHeaderFromBytes(potentialHeader);
    }

    /// <summary>
    /// Will try to deserialise a message header from first 4 bytes of the passed data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static PacketHeader DeserialiseHeader(byte[] data)
    {
        byte[] potentialHeader = data.Take(4).ToArray();

        return BuildHeaderFromBytes(potentialHeader);
    }

    /// <summary>
    /// Method that interprets the passed 4 bytes as the HHTP Packet header
    /// </summary>
    /// <param name="potentialHeader"></param>
    /// <returns>A header deserialised from given bytes</returns>
    /// <exception cref="Exception"></exception>
    public static PacketHeader BuildHeaderFromBytes(byte[] potentialHeader)
    {
        if (potentialHeader.Length != 4)
        {
            throw new Exception($"Could not get enough bytes for header, expected 4, got {potentialHeader.Length}");
        }

        if (!Enum.IsDefined(typeof(ProtocolVersion), (int)potentialHeader[0]))
        {
            throw new Exception("Unsupported protocol version");
        }

        ProtocolVersion protocolVersion = (ProtocolVersion)potentialHeader[0];

        StatusCode statusCode = (StatusCode)((potentialHeader[1] & 0b10000000) >> 7);

        uint maybeMethod = (uint)((potentialHeader[1] & 0b01100000) >> 5);
        if (!Enum.IsDefined(typeof(ProtocolMethod), (int)maybeMethod))
        {
            throw new Exception("Unsupported packet encoding type");
        }
        ProtocolMethod packetMethod = (ProtocolMethod)maybeMethod;

        uint maybeResourceIdentifier = (uint)(potentialHeader[1] & 0b00011111);

        if (maybeResourceIdentifier > 31 || maybeResourceIdentifier < 0)
        {
            throw new Exception("Unsupported resource identifier");
        }

        int payloadLength = BinaryPrimitives.ReadUInt16BigEndian(potentialHeader.AsSpan()[2..4]);

        PacketHeader header = new(protocolVersion, statusCode, packetMethod, (int)maybeResourceIdentifier, payloadLength);

        return header;
    }

    /// <summary>
    /// Serialises a given packet header to binary format, while taking care of the network order
    /// </summary>
    /// <param name="header"></param>
    /// <returns>A 4 byte array containing the packet header in binary format</returns>
    public static byte[] SerialiseHeader(PacketHeader header)
    {
        byte[] bytes = new byte[4];
        Span<byte> byteSpan = new (bytes,2,2);

        bytes[1] = (byte)header.ProtocolVersion;

        byte messageType, messageMethod, resourceIdentifier;

        if(BitConverter.IsLittleEndian)
        {
            messageType = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.StatusCode))[0];
            messageMethod = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.ProtocolMethod))[0];
            resourceIdentifier = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.ResourceIdentifier))[0];
        }

        messageType = BitConverter.GetBytes((int)header.StatusCode)[0];
        messageMethod = BitConverter.GetBytes((int)header.StatusCode)[0];
        resourceIdentifier = BitConverter.GetBytes((int)header.ResourceIdentifier)[0];

        bytes[3] = (byte)((messageType & 0b00000001) << 7  | ((messageMethod & 0b00000011) << 4) | (resourceIdentifier &0b00011111));

        BinaryPrimitives.TryWriteInt16BigEndian(byteSpan,(short)header.PayloadLength);

        return bytes;
    }
}