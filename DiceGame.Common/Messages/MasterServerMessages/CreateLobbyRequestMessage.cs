namespace DiceGame.Common.Messages;

public class CreateLobbyRequestMessage : BaseMessage
{
    public string LobbyName {get;set;}
    public CreateLobbyRequestMessage(string lobbyName)
    {
        LobbyName = lobbyName;
    }
}