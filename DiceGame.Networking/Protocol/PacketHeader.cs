namespace DiceGame.Networking.Protocol;

public class PacketHeader
{
    public ProtocolVersion ProtocolVersion {get;set;}
    public StatusCode StatusCode {get;set;}
    public ProtocolMethod ProtocolMethod {get;set;}
    public int ResourceIdentifier {get;set;}
    public int PayloadLength {get;set;}

    public PacketHeader(ProtocolVersion protocolVersion, StatusCode statusCode, ProtocolMethod protocolMethod, int resourceIdentifier, int payloadLength)
    {
        ProtocolVersion = protocolVersion;
        ProtocolMethod = protocolMethod;
        StatusCode = statusCode;
        ResourceIdentifier = resourceIdentifier;
        PayloadLength = payloadLength;
    }
}