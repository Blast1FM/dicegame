using System.Buffers.Binary;

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
}