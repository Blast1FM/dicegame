namespace DiceGame.Networking.Protocol;

public interface ICustomPacketSerialiser
{
    public HeaderSerialiser HeaderSerialiser {get;set;}
}