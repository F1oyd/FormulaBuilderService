using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace FBS.WpfClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this._viewModel = new ConnectionStatusViewModel();
            this.DataContext = this._viewModel;
        }

        private readonly ConnectionStatusViewModel _viewModel;

        private string Host => tbHost.Text;

        private int Port => int.TryParse(tbPort.Text, out int port) ? port : 0;

        private TcpClient _client;

        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsConnected)
            {
                try
                {
                    _client = new TcpClient(Host, Port);
                    _viewModel.IsConnected = true;
                }
                catch (SocketException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else
            {
                try
                {
                    SendMessage("DisconnectEvent");
                    _client.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                _viewModel.IsConnected = false;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMessage(_viewModel.RequestString);
                _viewModel.ResponseString = ReceiveMessage();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                _viewModel.IsConnected = false;
            }
        }

        private void SendMessage(string message)
        {
            byte[] requestBytes = Encoding.UTF8.GetBytes(message);
            _client.GetStream().Write(requestBytes, 0, requestBytes.Length);
        }

        private string ReceiveMessage()
        {
            byte[] buffer = new byte[102400];
            int i = _client.GetStream().Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, i);
        }
    }
}
