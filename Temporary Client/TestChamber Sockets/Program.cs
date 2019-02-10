using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketClient
{
    private static IPAddress ipAddress;
    private static IPEndPoint remoteEP;
    private static Socket socket;
    private static string username;

    public static void StartClient()
    {
        remoteEP = new IPEndPoint(ipAddress, 8008);

        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(remoteEP);

            Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint);
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception: {0}", e.Message);
        }
    }

    public static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                Console.WriteLine("What server do you want to connect to?");
                ipAddress = IPAddress.Parse(Console.ReadLine());
                break;
            }
            catch (Exception) { Console.WriteLine("Wrong ip address"); }
        }

        Console.WriteLine("Write your username");
        username = Console.ReadLine();

        while (true)
        {
            string r = Console.ReadLine();

            if (r.StartsWith("send"))
            {
                SendMessage(r.Remove(0, 5));
            }
            else if (r.Contains("shutdown"))
            {
                ShutdownClient();
            }
            else if (r.Contains("start"))
            {
                StartClient();
                SendMessage("userInfo " + username);
            }
            else if (r.StartsWith("getUnreadChat"))
            {
                SendMessage(r);
                Console.WriteLine(ReceiveMessage());
            }
            else if (r.StartsWith("getConnected"))
            {
                SendMessage(r);
                Console.WriteLine(ReceiveMessage());
            }
        }
    }

    private static void ShutdownClient()
    {
        try
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void SendMessage(string message)
    {
        try
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            socket.Send(msg);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static string ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        int rec = socket.Receive(buffer, SocketFlags.None);
        return Encoding.ASCII.GetString(buffer, 0, rec);
    }
}