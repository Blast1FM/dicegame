namespace DiceGame.Common.Models;

public class GameLobbyEntry
{
    public Guid LobbyID {get; set;}
    public string LobbyName {get;set;}
    public int PlayersConnected {get;set;}
}
