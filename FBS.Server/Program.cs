using System;
using System.Net;
using System.Text;
using FBS.Builder;

namespace FBS.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new AppConfig();
            var port = config.Port ?? 2200;

            var tcpServer = new TcpServer(IPAddress.Any, port);
            tcpServer.ClientConnected += tcpServer_ClientConnected;
            tcpServer.ClientDisconnected += client =>
            {
                Console.WriteLine("Client disconected: {0}", client.RemoteEndPoint);
            };
            tcpServer.Start(server => Console.WriteLine("SERVER STARTED ON PORT: {0}!", port));

            var exitOrder = false;
            do
            {
                var input = Console.ReadLine() ?? "";
                if (input.Equals("exit"))
                {
                    tcpServer.Stop(server => Console.WriteLine("SERVER TERMINATED!"));
                    exitOrder = true;
                }
                if (input.Equals("clients"))
                {
                    for (int i = 0; i < tcpServer.ConnectedClients.Count; i++)
                    {
                        Console.WriteLine("Client {0}: {1}", i, tcpServer.ConnectedClients[i].RemoteEndPoint);
                    }
                }
                if (input.StartsWith("disconnect"))
                {
                    var prms = input.Split();
                    if (prms.Length > 1)
                    {
                        if (int.TryParse(prms[1], out int clinetIndex))
                        {
                            if (tcpServer.ConnectedClients.Count > clinetIndex)
                            {
                                tcpServer.ConnectedClients[clinetIndex].Disconnect();
                            }
                            else
                            {
                                Console.WriteLine("There is no client with index {0}.", clinetIndex);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong client index value {0}.", prms[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Client index was not provided.");
                    }
                }
            } while (!exitOrder);
        }

        private static void tcpServer_ClientConnected(TcpClient client)
        {
            Console.WriteLine("Client connected: {0}", client.RemoteEndPoint);
            client.Received += (c, r) => Console.WriteLine("← Request received: {0} from client {1}", r, client.RemoteEndPoint);
            client.Sent += (c, r) => Console.WriteLine("→ Response sent: {0} to client {1}", r, client.RemoteEndPoint);

            while (client.IsConnected)
            {
                var receivedString = client.ReceiveString(Encoding.UTF8);
                if (receivedString.Equals("DisconnectEvent") || string.IsNullOrEmpty(receivedString))
                {
                    client.Disconnect();
                    break;
                }

                var builder = new FormulaBuilder(receivedString);
                var sendingString = builder.Build();

                client.SendString(sendingString, Encoding.UTF8);
            }
        }
    }
}
