namespace DiceGame.Networking.Protocol;

public class PacketHeader
{
    public ProtocolVersion ProtocolVersion {get;set;}
    public PacketEncoding PacketEncoding {get;set;}
    public PacketType PacketType {get;set;}
    public int PayloadLength {get;set;}

    public PacketHeader(ProtocolVersion protocolVersion, PacketEncoding packetEncoding, PacketType packetType, int payloadLength)
    {
        ProtocolVersion = protocolVersion;
        PacketEncoding = packetEncoding;
        PacketType = packetType;
        PayloadLength = payloadLength;
    }
}