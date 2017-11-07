using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Нажмите любую клавишу для подкл к серверу...");
            Console.ReadLine();

            TcpClient client = new TcpClient("localhost", 2000);
            while (true)
            {
                string request = Console.ReadLine();

                request = request.Equals("exit") ? "DisconnectEvent" : request;

                if (string.IsNullOrEmpty(request)) continue;

                byte[] requestBytes = Encoding.Default.GetBytes(request);
                client.GetStream().Write(requestBytes, 0, requestBytes.Length);

                byte[] buffer = new byte[102400];
                int i = client.GetStream().Read(buffer, 0, buffer.Length);
                string response = Encoding.Default.GetString(buffer, 0, i);

                Console.WriteLine(response);
            }
        }
    }
}
