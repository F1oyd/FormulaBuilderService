using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                    tcpServer.ConnectedClients.ForEach(client => Console.WriteLine(client.RemoteEndPoint));
                }
                if (input.StartsWith("disconnect"))
                {
                    tcpServer.ConnectedClients[int.Parse(input.Split(' ')[1])].Disconnect();
                }
            } while (!exitOrder);
        }

        private static void tcpServer_ClientConnected(TcpClient client)
        {
            Console.WriteLine("Client connected: {0}", client.RemoteEndPoint);
            client.Received += (c, r) => Console.WriteLine("← Request received: {0} from client {1}", r, client.RemoteEndPoint);
            client.Sended += (c, r) => Console.WriteLine("→ Response sent: {0} to client {1}", r, client.RemoteEndPoint);

            while (client.IsConnected)
            {
                var receivedBytes = client.ReceiveBytes();
                var receivedString = Encoding.UTF8.GetString(receivedBytes);
                if (receivedString.Equals("DisconnectEvent") || string.IsNullOrEmpty(receivedString))
                {
                    client.Disconnect();
                    break;
                }

                var builder = new FormulaBuilder(receivedString);
                var sendingString = builder.Build();

                var match = Regex.Match(receivedString, "Sec-WebSocket-Key: (?<key>[^\\s]+)");
                if (match.Success)
                {
                    var key = match.Groups["key"].Value;
                    var ha = SHA1.Create();
                    ha.ComputeHash(Encoding.UTF8.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
                    var acceptString = Convert.ToBase64String(ha.Hash);
                    var sb = new StringBuilder();
                    sb.AppendLine("HTTP/1.1 101 Switching Protocols");
                    sb.AppendLine("Upgrade: websocket");
                    sb.AppendLine("Connection: Upgrade");
                    sb.AppendFormat("Sec-WebSocket-Accept: {0}", acceptString);
                    sb.AppendLine();
                    sb.AppendLine();
                    sendingString = sb.ToString();
                    client.SendString(sendingString, Encoding.Default);
                }
                else
                {
                    receivedString = DecodeMessage(receivedBytes);
                    client.SendBytes(EncodeMessageToSend("test"));
                }
            }
        }

        private static String DecodeMessage(Byte[] bytes)
        {
            String incomingData = String.Empty;
            Byte secondByte = bytes[1];
            Int32 dataLength = secondByte & 127;
            Int32 indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;

            IEnumerable<Byte> keys = bytes.Skip(indexFirstMask).Take(4);
            Int32 indexFirstDataByte = indexFirstMask + 4;

            Byte[] decoded = new Byte[bytes.Length - indexFirstDataByte];
            for (Int32 i = indexFirstDataByte, j = 0; i < bytes.Length; i++, j++)
            {
                decoded[j] = (Byte)(bytes[i] ^ keys.ElementAt(j % 4));
            }

            return incomingData = Encoding.UTF8.GetString(decoded, 0, decoded.Length);
        }

        private static Byte[] EncodeMessageToSend(String message)
        {
            Byte[] response;
            Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            Byte[] frame = new Byte[10];

            Int32 indexStartRawData = -1;
            Int32 length = bytesRaw.Length;

            frame[0] = (Byte)129;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (Byte)126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (Byte)127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }
    }
}
