using System;

namespace GameServer
{
    class Program
    {
        static ServerNetworking server = new ServerNetworking();
        static GameInfo gameInfo = new GameInfo();

        static void Main(string[] args)
        {
            PortSelection();
            gameInfo.LoadGame();
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
                else if (command == "get ore map")
                {
                    for (int x = 0; x < 100; x++)
                    {
                        for (int y = 0; y < 100; y++)
                        {
                            Console.Write(gameInfo.oreMap[x, y] + " ");
                        }
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
