using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;
using GameServer;

public class ServerNetworking
{
    public Client[] clients;

    public ServerSettings settings;
    
    private IPAddress acceptIp;
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
        
        acceptIp = IPAddress.Any;
        Console.WriteLine("Server bound to " + acceptIp + " with port " + settings.Port);
        localEndPoint = new IPEndPoint(acceptIp, settings.Port);
        listenSocket = new Socket(acceptIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
        catch (Exception e)
        {
            Console.WriteLine("Error while receiving data from " + ip + ": " + e.Message);
            CloseClient(index);
        }
    }

    public void CloseClient(int Index)
    {
        if (socket != null)
        {
            Console.WriteLine(player.username + " has disconnected from the server");
            player = null;
            isClosed = true;
            socket.Close();
            socket = null;
        }
    }

    public void CloseClient(int Index, string reason)
    {
        Console.WriteLine(player.username + " has disconnected from the server. Reason: " + reason);
        player = null;
        isClosed = true;
        socket.Close();
        socket = null;
    }

    private void HandleQuery(string query)
    {
        if (player == null && !query.StartsWith("userInfo"))
        {
            CloseClient(index, "no user info");
        }

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

            Console.WriteLine("Client with ip " + ip + " assigned username " + username);
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
        else if (query.StartsWith("getConnected"))
        {
            string msg = "awsUsers=";
            Client[] ccs = GetConnectedClients();

            if (ccs != null)
            {
                foreach (var cc in clients)
                {
                    if (cc.player != null)
                    {
                        msg += cc.player.username + "<eou>";
                    }
                }

                AnwserToQuery(msg);
            }
            else
            {
                AnwserToQuery("noUsers");
            }
        }
        else if (query.StartsWith("position="))
        {
            string temp = query.Remove(0, 9);
            string x = "";
            string y = "";

            //X
            foreach (var a in temp)
            {
                if (a != ';')
                {
                    x += a;
                }
                else
                {
                    break;
                }
            }

            temp = temp.Remove(0, x.Length + 2);
            //Y
            foreach (var b in temp)
            {
                if (b != ';')
                {
                    y += b;
                }
                else
                {
                    break;
                }
            }

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ",";
            player.WritePosition(float.Parse(x), float.Parse(y));
            //Console.WriteLine(player.username + " moved to x=" + x + ", y=" + y);
        }
        else if (query == "getPositions")
        {
            string anwser = "awsPos=";
            Client[] ccs = GetConnectedClients();

            if (ccs != null)
            {
                foreach (var cc in ccs)
                {
                    anwser += '"' + cc.player.username + '"' + " " + cc.player.position[0] + "; " + cc.player.position[1] + "<eou>";
                }

                AnwserToQuery(anwser + " ");
            }
            else
            {
                AnwserToQuery("noPos");
            }
        }
        else
        {
            Console.WriteLine("Unintelligible query from " + ip + ": " + query);
        }
    }

    private void AnwserToQuery(string anwser)
    {
        int size = Encoding.ASCII.GetByteCount(anwser);
        
        socket.Send(Encoding.ASCII.GetBytes(anwser), 0, size, SocketFlags.None);
    }

    public void UpdateClients(Client[] updatedClients)
    {
        clients = updatedClients;
    }

    private Client[] GetConnectedClients()
    {
        List<Client> tempClients = new List<Client>();
        int i = 0;

        foreach (var c in clients)
        {
            if (!c.isClosed && c.player != null)
            {
                if (c.player.username != player.username)
                {
                    tempClients.Add(c);
                    i++;
                }
            }
        }

        if (i > 0)
        {
            return tempClients.ToArray();
        }
        else
        {
            return null;
        }
    }
}