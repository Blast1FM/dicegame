using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class MainGameRollMessage : BaseMessage
{
    public PlayerInfo Player {get; set;}
    public int RollNumber {get;set;}
    public List<int> Rolls {get; set;}
    public MainGameRollMessage(PlayerInfo player, int rollNumber, List<int> rolls)
    {
        Player = player;
        RollNumber = rollNumber;
        Rolls = rolls;
    }
}