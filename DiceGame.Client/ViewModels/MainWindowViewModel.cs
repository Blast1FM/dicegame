using ReactiveUI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Threading.Tasks;
using DiceGame.Networking;

namespace BakDiceClient.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _serverIp = string.Empty;
        private string _connectionStatus = "Не подключено.";
        private string _lobbyName = string.Empty;
        private HHTPClient _client;
        private int _currentState = 0; // 0 - подключение, 1 - главное меню, 2 - лобби

        public string ServerIp
        {
            get => _serverIp;
            set => this.RaiseAndSetIfChanged(ref _serverIp, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
        }

        public string LobbyName
        {
            get => _lobbyName;
            set => this.RaiseAndSetIfChanged(ref _lobbyName, value);
        }

        public int CurrentState
        {
            get => _currentState;
            set => this.RaiseAndSetIfChanged(ref _currentState, value);
        }

        // Команды
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, int> ShowLobbyCommand { get; }
        public ReactiveCommand<Unit, int> BackToMenuCommand { get; }
        public ReactiveCommand<Unit, string> ShowLeaderboardCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public ReactiveCommand<Unit, string> CreateLobbyCommand { get; }
        public ReactiveCommand<Unit, string> JoinLobbyCommand { get; }

        public MainWindowViewModel()
        {
            // Инициализация команд
            ConnectCommand = ReactiveCommand.CreateFromTask(ConnectToServerAsync);
            ShowLobbyCommand = ReactiveCommand.Create(() => 2); // Переход в стейт лобби
            BackToMenuCommand = ReactiveCommand.Create(() => 1); // Возврат в главное меню
            ShowLeaderboardCommand = ReactiveCommand.Create(() => "Лидерборд пока не реализован.");
            ExitCommand = ReactiveCommand.Create(() => Environment.Exit(0));
            CreateLobbyCommand = ReactiveCommand.Create(() => $"Лобби '{LobbyName}' создано!");
            JoinLobbyCommand = ReactiveCommand.Create(() => "Подключение к лобби...");

            // Подписка на команды, изменяющие состояние
            ShowLobbyCommand.Subscribe(state => CurrentState = state);
            BackToMenuCommand.Subscribe(state => CurrentState = state);

            // Создаем пустой клиент
            _client = new HHTPClient(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        }

        private async Task ConnectToServerAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ServerIp))
                {
                    ConnectionStatus = "Ошибка: Введите IP сервера.";
                    return;
                }

                if (!IPAddress.TryParse(ServerIp, out var ipAddress))
                {
                    ConnectionStatus = "Ошибка: Неверный формат IP-адреса.";
                    return;
                }

                var endPoint = new IPEndPoint(ipAddress, 12345);

                ConnectionStatus = "Подключение...";
                await Task.Run(() => _client.Connect(endPoint));

                ConnectionStatus = "Подключено к серверу!";
                CurrentState = 1; // Переход в главное меню
            }
            catch (SocketException ex)
            {
                ConnectionStatus = $"Ошибка подключения: {ex.Message}";
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Неизвестная ошибка: {ex.Message}";
            }
        }
    }
}