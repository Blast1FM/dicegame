using System.Buffers.Binary;

namespace DiceGame.Networking.Protocol;

public class HeaderSerialiser
{
    public PacketHeader DeserialiseHeader(ref Stream dataStream)
    {
        byte[] potentialHeader = new byte[4];
        int bytesRead = dataStream.Read(potentialHeader,0,4);

        // TODO better exceptions lol
        if(bytesRead != 4) throw new Exception("Could not read enough bytes for header");

        if(!Enum.IsDefined(typeof(PacketHeader), potentialHeader[0])) throw new Exception("Unsupported protocol version");
        ProtocolVersion protocolVersion = (ProtocolVersion)potentialHeader[0];

        MessageType messageType = (MessageType)((int)(potentialHeader[1] & 0b10000000)>>7);

        uint maybeMethod = (uint)((potentialHeader[1] & 0b01100000) >> 5);
        if(!Enum.IsDefined(typeof(ProtocolMethod), maybeMethod)) throw new Exception("Unsupported packet encoding type");
        ProtocolMethod packetMethod = (ProtocolMethod) maybeMethod;

        uint maybeResourceIdentifier = (uint)(potentialHeader[1] & 0b00011111);
        if(maybeResourceIdentifier > 31 || maybeResourceIdentifier < 0) throw new Exception("Unsupported resource identifier");

        // Run some check here too - TBD
        int payloadLength = BinaryPrimitives.ReadUInt16BigEndian(potentialHeader[2..3]);
        
        PacketHeader header = new(protocolVersion, messageType, packetMethod, (int)maybeResourceIdentifier, payloadLength);

        return header;

    }

    // TODO validation
    // TODO WRITE A FUCKING UNIT TEST FOR THIS, IT WILL BREAK EVERYTHING AND CAUSE HEADACHE
    public byte[] SerialiseHeader(PacketHeader header)
    {
        byte[] bytes = new byte[4];
        Span<byte> byteSpan = new (bytes,2,2);

        bytes[1] = (byte)header.ProtocolVersion;

        byte messageType, messageMethod, resourceIdentifier;

        if(BitConverter.IsLittleEndian)
        {
            messageType = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.MessageType))[0];
            messageMethod = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.ProtocolMethod))[0];
            resourceIdentifier = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.ResourceIdentifier))[0];
        }

        messageType = BitConverter.GetBytes((int)header.MessageType)[0];
        messageMethod = BitConverter.GetBytes((int)header.MessageType)[0];
        resourceIdentifier = BitConverter.GetBytes((int)header.ResourceIdentifier)[0];

        // TODO double check this
        bytes[3] = (byte)((messageType & 0b00000001) << 7  | ((messageMethod & 0b00000011) << 4) | (resourceIdentifier &0b00011111));

        BinaryPrimitives.TryWriteInt16BigEndian(byteSpan,(short)header.PayloadLength);

        return bytes;
    }
}