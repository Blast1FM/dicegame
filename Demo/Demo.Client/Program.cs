namespace Demo.Client;

class Program
{
    static void Main(string[] args)
    {
        Client client = new();
        var clientTask = client.Run();
        clientTask.Wait();
    }
}
