namespace DiceGame.Common.Models;

public class LeaderboardEntry
{
    public Player Player {get;set;}
    public int GamesPlayed {get;set;}
    public decimal AmountOwedTotal {get;set;}
}