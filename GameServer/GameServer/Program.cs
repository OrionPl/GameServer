using System;

namespace GameServer
{
    class Program
    {
        static ServerNetworking server = new ServerNetworking();

        static void Main(string[] args)
        {
            PortSelection();
            server.StartServer();
            Console.WriteLine("Server starting...");

            while (true)
            {
                string command = Console.ReadLine();

                if (command == "exit")
                {
                    Environment.Exit(0);
                }
                else if (command == "players")
                {
                    foreach (var name in server.GetConnectedUsernames())
                    {
                        Console.WriteLine(name);
                    }
                }

            }
        }

        private static void PortSelection()
        {
            Console.WriteLine("What port do you want to use? (Press Enter to use default 8008)");

            while (true)
            {
                try
                {
                    string t = Console.ReadLine();

                    if (t.Length != 0)
                    {
                        server.settings.Port = int.Parse(t);
                        Console.WriteLine("Port set to " + t);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Port set to 8008");
                        return;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Wrong input");
                }
            }
        }
    }
}
