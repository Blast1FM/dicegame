using System.Security.Cryptography.X509Certificates;
using DiceGame.Networking;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        Server server = new();
        var serverTask = Task.Run(server.Run);
        serverTask.Wait();
    }
}
