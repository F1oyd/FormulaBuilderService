using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FBS.Server
{
    internal delegate void StartedCallback(TcpServer server);

    internal delegate void StoppedCallback(TcpServer server);

    internal class TcpServer
    {
        private readonly TcpListener server;

        public bool IsWorking { get; set; }
        public List<TcpClient> ConnectedClients;

        public TcpServer(IPAddress ipAddress, int port = 2200)
        {
            this.server = new TcpListener(ipAddress, port);
        }
        public TcpServer() : this(IPAddress.Any) { }

        public void Start(StartedCallback started)
        {
            this.server.Start();
            this.IsWorking = true;
            this.ConnectedClients = new List<TcpClient>();
            started?.Invoke(this);

            BeginAccept();
        }

        public void Stop(StoppedCallback stopped)
        {
            this.server.Stop();
            this.IsWorking = false;
            this.ConnectedClients.ForEach(client => client.Disconnect());
            this.ConnectedClients.Clear();
            stopped?.Invoke(this);
        }

        public event ConnectedEventHandler ClientConnected;
        private void BeginAccept()
        {
            this.server.BeginAcceptTcpClient(EndAccept, null);
        }
        private void EndAccept(IAsyncResult ar)
        {
            this.BeginAccept();
            System.Net.Sockets.TcpClient client = this.server.EndAcceptTcpClient(ar);

            TcpClient tcpClient = new TcpClient(client);
            this.ConnectedClients.Add(tcpClient);
            tcpClient.ClientDisconnected += extClient_ClientDisconnected;
            ClientConnected?.Invoke(tcpClient);
        }

        public event DisconnectedEventHandler ClientDisconnected;

        private void extClient_ClientDisconnected(TcpClient tcpClient)
        {
            this.ConnectedClients.Remove(tcpClient);
            ClientDisconnected?.Invoke(tcpClient);
        }
    }
}
