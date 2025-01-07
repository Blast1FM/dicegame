using System.Text;

namespace DiceGame.Networking.Protocol;

public class Packet
{
    public PacketHeader Header { get; set; }
    public string Payload {get; set;}

    public Packet(PacketHeader header, string payload)
    {
        Header = header;
        Payload = payload;
    }

    public Packet(StatusCode statusCode, ProtocolMethod method, int resourceIdentifier, string payload)
    {
        PacketHeader header = new(ProtocolVersion.V1, statusCode, method, resourceIdentifier, Encoding.BigEndianUnicode.GetByteCount(payload));
        Header = header;
        Payload = payload;
    }
    public static Packet CreateErrorResponsePacket(Packet request, string message)
    {
        PacketHeader header = new(ProtocolVersion.V1, StatusCode.Error, request.Header.ProtocolMethod,request.Header.ResourceIdentifier, Encoding.BigEndianUnicode.GetByteCount(message));
        return new Packet(header, message);
    }
}
