using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StartServer();
        }

        private static void StartServer()
        {
            // Get IPHostEntry from Dns ("localhost")
            // Get ip address and setup local end point
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12345);

            foreach (var ip in host.AddressList)
            {
                Console.WriteLine(ip);
            }

            try
            {
                // Create a listener socket, that will use ProtocolType.Tcp
                // Bind local end point to that listener
                // Specify how many requests a Socket can listen before it gives "Server Busy" -> value 10
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for a connection...");
                // Create a handler socket (listener.Accept())
                Socket handler = listener.Accept();

                // Store incoming data from the client -> data: string, bytes: byte[]
                string data = null;
                byte[] bytes = null;
                int bufferSize = 1024;

                // While true, Receive bytes data using the handler and add these encoded bytes to data string
                // Check whether indexof <EOF> is greater than -1 (-1 is returned when <EOF> is not found), if found then break out of while loop
                while(true)
                {
                    bytes = new byte[bufferSize];
                    int bytesReceived = handler.Receive(bytes);

                    StringBuilder stringBuilder = new StringBuilder(data);
                    stringBuilder.Append(Encoding.ASCII.GetString(bytes, 0, bytesReceived));
                    data = stringBuilder.ToString();

                    if (data.IndexOf("<EOF>") > -1) break;
                }

                Console.WriteLine($"Text received : {data}");

                // Encoding.ASCII.GetBytes(data) to msg: byte[] variable
                // Then Send, Shutdown(SocketShutdown.Both) and Close connection from the handler
                byte[] msg = Encoding.ASCII.GetBytes(data);
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}