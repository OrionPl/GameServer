using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer;

public class ServerNetworking
{
    public Client[] clients;

    public ServerSettings settings;

    private IPHostEntry localHostEntry;
    private IPAddress localIp;
    private IPEndPoint localEndPoint;
    private Socket listenSocket;

    public void StartServer()
    {
        settings = new ServerSettings();
        clients = new Client[settings.MaxPlayers];

        for (int i = 0; i < clients.Length; i++)
        {
            clients[i] = new Client();
        }

        localHostEntry = Dns.GetHostEntry("localhost");
        localIp = localHostEntry.AddressList[0];
        localEndPoint = new IPEndPoint(localIp, settings.Port);
        listenSocket = new Socket(localIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(100);

            listenSocket.BeginAccept(new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void OnConnect(IAsyncResult ar)
    {
        Socket socket = listenSocket.EndAccept(ar);
        listenSocket.BeginAccept(new AsyncCallback(OnConnect), null);

        for (int i = 0; i < clients.Length; i++)
        {
            if (clients[i].socket == null)
            {
                Socket newClientSocket = socket;
                clients[i].socket = newClientSocket;
                clients[i].index = i;
                clients[i].ip = socket.RemoteEndPoint.ToString();
                clients[i].StartClient(clients);
                UpdateClientsClients();

                Console.WriteLine("Connection from " + clients[i].ip);
                return;
            }
        }
    }

    private void UpdateClientsClients()
    {
        foreach (var client in clients)
        {
            client.UpdateClients(clients);
        }
    }

    public string[] GetConnectedUsernames()
    {
        List<string> ret = new List<string>();

        foreach (var client in clients)
        {
            if (client.socket != null && !client.isClosed && client.player != null)
            {
                ret.Add(client.player.username);
            }
        }

        return ret.ToArray();
    }
}

public class Client
{
    public bool isClosed = true;
    public Socket socket;
    public string ip;
    public int index;
    private byte[] buffer = new byte[1024];
    private Client[] clients;

    public Player player;

    public void StartClient(Client[] Clients)
    {
        isClosed = false;
        clients = Clients;
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), socket);
    }

    private void OnReceive(IAsyncResult ar)
    {
        Console.WriteLine("receiving");
        Socket socket = (Socket) ar.AsyncState;

        try
        {
            int rec = socket.EndReceive(ar);

            if (rec <= 0)
            {
                CloseClient(index);
            }
            else
            {
                string received = Encoding.ASCII.GetString(buffer, 0, rec);
                HandleQuery(received);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), socket);
            }
        }
        catch (Exception)
        {
            //Console.WriteLine("Error while receiving data from " + ip);
            CloseClient(index);
        }
    }

    public void CloseClient(int Index)
    {
        Console.WriteLine(ip + " has disconnected from the server");
        isClosed = true;
        socket.Close();
        socket = null;
    }

    private void HandleQuery(string query)
    {
        if (query.StartsWith("userInfo"))
        {
            bool startAdding = false;
            string username = "";

            foreach (var c in query)
            {
                if (c >= 33 && c <= 126 && startAdding)
                {
                    username += c;
                }
                else if (c == 32)
                {
                    startAdding = true;
                }
                else if (startAdding)
                {
                    break;
                }
            }

            player = new Player(username);
        }
        else if (query.StartsWith("say"))
        {
            foreach (var cc in GetConnectedClients())
            {
                string chatMessage = player.username + "> " + query.Remove(0, 4);
                cc.player.AddStringToChat(chatMessage);
            }
        }
        else if (query.StartsWith("getUnreadChat"))
        {
            string unreadChatToSend = "chat: ";
            foreach (var msg in player.ReadChat())
            {
                unreadChatToSend += msg + "<eom>";
            }
            AnwserToQuery(unreadChatToSend);
        }
        else
        {
            Console.WriteLine("Unintelligible query from " + ip + ": " + query);
        }
    }

    private void AnwserToQuery(string anwser)
    {
        socket.Send(buffer, 0, Encoding.ASCII.GetByteCount(anwser), SocketFlags.None);
    }

    public void UpdateClients(Client[] updatedClients)
    {
        clients = updatedClients;
    }

    private Client[] GetConnectedClients()
    {
        List<Client> tempClients = new List<Client>();

        foreach (var c in clients)
        {
            if (!c.isClosed)
            {
                tempClients.Add(c);
            }
        }

        return tempClients.ToArray();
    }
}