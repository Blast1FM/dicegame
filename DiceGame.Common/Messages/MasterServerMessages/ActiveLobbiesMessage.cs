using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class ActiveLobbiesMessage : BaseMessage
{
    public List<GameLobbyEntry> ActiveLobbies {get;set;}
}