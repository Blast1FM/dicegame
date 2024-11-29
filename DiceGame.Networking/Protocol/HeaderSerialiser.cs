using System.Buffers.Binary;
using System.Data.SqlTypes;

namespace DiceGame.Networking.Protocol;

public class HeaderSerialiser
{
    public PacketHeader DeserialiseHeader(ref MemoryStream dataStream)
    {
        byte[] potentialHeader = new byte[4];
        int bytesRead = dataStream.Read(potentialHeader,0,4);

        // TODO better exceptions lol
        if(bytesRead != 4) throw new Exception("Could not read enough bytes for header");

        if(!Enum.IsDefined(typeof(PacketHeader), potentialHeader[0])) throw new Exception("Unsupported protocol version");
        ProtocolVersion protocolVersion = (ProtocolVersion)potentialHeader[0];

        int maybeEncoding = (potentialHeader[1] & 0b11000000) >> 6;
        if(!Enum.IsDefined(typeof(PacketEncoding), maybeEncoding)) throw new Exception("Unsupported packet encoding type");
        PacketEncoding packetEncoding = (PacketEncoding) maybeEncoding;

        int maybePacketType = potentialHeader[1] & 0b00111111;
        if(!Enum.IsDefined(typeof(PacketType), maybePacketType)) throw new Exception("Unsupported packet type");
        PacketType packetType = (PacketType)maybePacketType;

        // Run some check here too - TBD
        int payloadLength = BinaryPrimitives.ReadUInt16BigEndian(potentialHeader[2..3]);
        
        PacketHeader header = new(protocolVersion, packetEncoding, packetType, payloadLength);

        return header;

    }

    // TODO validation
    // TODO WRITE A FUCKING UNIT TEST FOR THIS, IT WILL BREAK EVERYTHING AND CAUSE HEADACHE
    public byte[] SerialiseHeader(PacketHeader header)
    {
        byte[] bytes = new byte[4];
        Span<byte> byteSpan = new (bytes,2,2);

        bytes[1] = (byte)header.ProtocolVersion;

        byte[] encoding = new byte[4];
        byte[] packetType = new byte[4];

        if(BitConverter.IsLittleEndian)
        {
            encoding = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.PacketEncoding));
            packetType = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)header.PacketType));
        }

        encoding = BitConverter.GetBytes((int)header.PacketEncoding);
        packetType = BitConverter.GetBytes((int)header.PacketType);

        // TODO double check this
        bytes[3] = (byte) ((encoding[0] & 0b00111111)  | ((packetType[0] & 0b00000011) >> 2));

        BinaryPrimitives.TryWriteInt16BigEndian(byteSpan,(short)header.PayloadLength);

        return bytes;
    }
}