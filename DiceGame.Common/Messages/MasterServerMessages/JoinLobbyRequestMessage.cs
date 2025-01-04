using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class JoinLobbyRequestMessage : BaseMessage
{
    public Guid LobbyID {get;set;}
    public JoinLobbyRequestMessage(Guid lobbyId)
    {
        LobbyID = lobbyId;
    }
}