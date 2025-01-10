namespace Demo.Server;

class Program
{
    static void Main(string[] args)
    {
        Server server = new();
        var serverTask = Task.Run(server.Run);
        serverTask.Wait();
    }
}
