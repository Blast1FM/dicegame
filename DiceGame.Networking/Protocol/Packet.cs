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
}
