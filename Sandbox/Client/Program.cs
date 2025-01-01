using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using DiceGame.Networking;
using Microsoft.VisualBasic;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        Client client = new();
        client.Run();
    }
}
