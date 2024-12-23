using System.Buffers.Binary;

namespace DiceGame.Networking.Protocol;

public static class HeaderSerialiser
{
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

    public static PacketHeader BuildHeaderFromBytes(byte[] potentialHeader)
    {
        if (potentialHeader.Length != 4) throw new Exception($"Could not get enough bytes for header, expected 4, got {potentialHeader.Length}");

        if(!Enum.IsDefined(typeof(PacketHeader), potentialHeader[0])) throw new Exception("Unsupported protocol version");
        ProtocolVersion protocolVersion = (ProtocolVersion)potentialHeader[0];

        StatusCode statusCode = (StatusCode)((int)(potentialHeader[1] & 0b10000000)>>7);

        uint maybeMethod = (uint)((potentialHeader[1] & 0b01100000) >> 5);
        if(!Enum.IsDefined(typeof(ProtocolMethod), maybeMethod)) throw new Exception("Unsupported packet encoding type");
        ProtocolMethod packetMethod = (ProtocolMethod) maybeMethod;

        uint maybeResourceIdentifier = (uint)(potentialHeader[1] & 0b00011111);
        if(maybeResourceIdentifier > 31 || maybeResourceIdentifier < 0) throw new Exception("Unsupported resource identifier");

        // Run some check here too - TBD
        int payloadLength = BinaryPrimitives.ReadUInt16BigEndian(potentialHeader.AsSpan()[2..3]);
        
        PacketHeader header = new(protocolVersion, statusCode, packetMethod, (int)maybeResourceIdentifier, payloadLength);

        return header;
    }

    // TODO validation
    // TODO WRITE A FUCKING UNIT TEST FOR THIS, IT WILL BREAK EVERYTHING AND CAUSE HEADACHE
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

        // TODO double check this
        bytes[3] = (byte)((messageType & 0b00000001) << 7  | ((messageMethod & 0b00000011) << 4) | (resourceIdentifier &0b00011111));

        BinaryPrimitives.TryWriteInt16BigEndian(byteSpan,(short)header.PayloadLength);

        return bytes;
    }
}