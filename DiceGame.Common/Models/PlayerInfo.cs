namespace DiceGame.Common.Models;

public class PlayerInfo
{
    public Guid Guid {get;set;}
    public string Name {get;set;}
    public decimal AmountRequested {get;set;}
    public PlayerInfo(Guid guid, string name, decimal amountRequested)
    {
        Guid = guid;
        Name = name;
        AmountRequested = amountRequested;
    }
}