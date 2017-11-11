using System.IO;
using System.Net;
using System.Text;

namespace FBS.Server
{
    internal delegate void ReceivedEventHandler(TcpClient client, object request);

    internal delegate void SentEventHandler(TcpClient client, object response);

    internal delegate void ConnectedEventHandler(TcpClient client);

    internal delegate void DisconnectedEventHandler(TcpClient client);

    internal class TcpClient
    {
        public System.Net.Sockets.TcpClient Client { get; }
        public bool IsConnected { get; private set; }
        public EndPoint RemoteEndPoint { get; }

        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            this.Client = client;
            this.IsConnected = true;
            this.RemoteEndPoint = client.Client.RemoteEndPoint;
        }

        public event ReceivedEventHandler Received;
        public string ReceiveString(Encoding encoding)
        {
            if (!this.Client.Connected) return string.Empty;

            string str;
            try
            {
                byte[] buffer = new byte[this.Client.ReceiveBufferSize];
                int i = this.Client.GetStream().Read(buffer, 0, buffer.Length);
                str = encoding.GetString(buffer, 0, i);
            }
            catch (IOException)
            {
                this.Disconnect();
                return string.Empty;
            }
            Received?.Invoke(this, str);
            return str;
        }
        public byte[] ReceiveBytes()
        {
            if (!this.Client.Connected) return new byte[0];

            byte[] bytes;
            try
            {
                byte[] buffer = new byte[this.Client.ReceiveBufferSize];
                this.Client.GetStream().Read(buffer, 0, buffer.Length);
                bytes = buffer;
            }
            catch (IOException)
            {
                this.Disconnect();
                return new byte[0];
            }
            Received?.Invoke(this, bytes);
            return bytes;
        }

        public event SentEventHandler Sent;
        public void SendString(string str, Encoding encoding)
        {
            if (!this.Client.Connected) return;

            try
            {
                byte[] sendingBytes = encoding.GetBytes(str);
                this.Client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);
            }
            catch (IOException)
            {
                this.Disconnect();
                return;
            }
            Sent?.Invoke(this, str);
        }
        public void SendBytes(byte[] bytes)
        {
            if (!this.Client.Connected) return;

            try
            {
                this.Client.GetStream().Write(bytes, 0, bytes.Length);
            }
            catch (IOException)
            {
                this.Disconnect();
                return;
            }
            Sent?.Invoke(this, bytes);
        }

        public event DisconnectedEventHandler ClientDisconnected;
        public void Disconnect()
        {
            ClientDisconnected?.Invoke(this);
            this.Client.Close();
            this.IsConnected = false;
        }
    }
}