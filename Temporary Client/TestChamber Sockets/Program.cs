using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketClient
{
    private static IPAddress ipAddress;
    private static IPEndPoint remoteEP;
    private static Socket socket;

    public static void StartClient()
    {
        ipAddress = IPAddress.Parse("192.168.0.250");
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
            }
            else if (r.StartsWith("getUnreadChat"))
            {
                SendMessage(r.Remove(0, 14));
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