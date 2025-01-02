using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using DiceGame.Networking;
using Microsoft.VisualBasic;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        Client client = new();
        _ = client.Run();
    }
}