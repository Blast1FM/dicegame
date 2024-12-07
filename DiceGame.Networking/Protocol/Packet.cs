namespace DiceGame.Networking.Protocol;

public class Packet
{
    public PacketHeader Header { get; set; }
    public string PayloadData {get; set;}
}
