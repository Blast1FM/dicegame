namespace DiceGame.Common.Models;

public class LeaderboardEntry
{
    public PlayerInfo Player {get;set;}
    public int GamesPlayed {get;set;}
    public decimal AmountOwedTotal {get;set;}
}