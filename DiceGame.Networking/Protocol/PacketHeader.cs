namespace DiceGame.Networking.Protocol;

public class PacketHeader
{
    public ProtocolVersion ProtocolVersion {get;set;}
    public MessageType MessageType {get;set;}
    public ProtocolMethod ProtocolMethod {get;set;}
    public int ResourceIdentifier {get;set;}
    public int PayloadLength {get;set;}

    public PacketHeader(ProtocolVersion protocolVersion, MessageType messageType, ProtocolMethod protocolMethod, int resourceIdentifier, int payloadLength)
    {
        ProtocolVersion = protocolVersion;
        ProtocolMethod = protocolMethod;
        MessageType = messageType;
        ResourceIdentifier = resourceIdentifier;
        PayloadLength = payloadLength;
    }
}