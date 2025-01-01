using System.Security.Cryptography.X509Certificates;
using DiceGame.Networking;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        Server server = new();
        server.Run();
    }

    
}
