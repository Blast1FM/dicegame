using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class JoinLobbyRequestMessage : BaseMessage
{
    public GameLobbyEntry Lobby {get;set;}
    public JoinLobbyRequestMessage(GameLobbyEntry lobby)
    {
        Lobby = lobby;
    }
}