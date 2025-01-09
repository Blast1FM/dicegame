using System;
using System.Text.Json;
using DiceGame.Common.Messages.GameRoomMessages;
using DiceGame.Common.Networking;
using DiceGame.GameServer.GameRoom.Infrastructure;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;

namespace DiceGame.GameServer.GameRoom.States;

public class PreGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    private int _currentRollNumber = 0;
    private readonly Random _rng;
    private Dictionary<int, Player> _playerRollOrder;
    public PreGameState(GameRoomController controller)
    {
        _controller = controller;
        _requestRouter = new();
        _playerRollOrder = new(_controller.MaxPlayerCount);
        _rng = new();
        List<Action<Packet,HHTPClient>> postHandlers = [HandlePostSendReadyRequest];
        List<Action<Packet,HHTPClient>> getHandlers = [HandleGetConnectedPlayersRequest, HandleGetStateStatusRequest];
        _requestRouter.SetPostHandlers(postHandlers);
        _requestRouter.SetGetHandlers(getHandlers);
    }
    public override async Task Enter()
    {
        _controller.PlayerDisconnected += HandlePlayerDisconnect;
        _controller._listener.ClientConnected += HandleClientConnected;
    }

    public override async Task Exit()
    {
        _controller.PlayerDisconnected -= HandlePlayerDisconnect;
        _controller._listener.ClientConnected -= HandleClientConnected;
    }

    public override async Task Update()
    {
        throw new NotImplementedException();
    }

    public async void HandleGetConnectedPlayersRequest(Packet packet, HHTPClient clientConnection)
    {
        try
        {
            ConnectedPlayerListMessage connectedPlayersDTO = new(_controller._players.Select(p => p.PlayerInfo).ToList());
            bool success = await clientConnection.SendMessage<ConnectedPlayerListMessage>(connectedPlayersDTO, packet);
        }
        catch (JsonException e)
        {
            Packet errorPacket = new(StatusCode.Error, packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier, $"Serialisation error: {e.Message}");
        }
        catch (Exception e)
        {
            Packet errorPacket = new(StatusCode.Error, packet.Header.ProtocolMethod, packet.Header.ResourceIdentifier, $"Unhandled server error:{e.Message}");
        }
    }

    public async void HandleGetStateStatusRequest(Packet packet, HHTPClient clientConnection)
    {
        throw new NotImplementedException();   
    }

    public async void HandlePostSendReadyRequest(Packet packet, HHTPClient clientConnection)
    {
        throw new NotImplementedException();
    }

    public async void HandleGetCurrentRollRequest(Packet packet, HHTPClient clienConnection)
    {
        throw new NotImplementedException();
    }

    private void HandlePlayerDisconnect(object? sender, PlayerConnectionEventArgs e)
    {
        // TODO finish implementing
        _controller._listener.StartListening();
        throw new NotImplementedException();
    }

    private void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public int RollOneDie()
    {
        _currentRollNumber++;
        return _rng.Next(0,7);
    }
}
