using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FBS.WpfClient
{
    public sealed class ConnectionStatusViewModel : INotifyPropertyChanged
    {
        public ConnectionStatusViewModel()
        {
            _connectionButtonTitle = "Connect";
            _requestString = @"<request>
    <expression>
    <operation>plus</operation>
    <operand>
        <const>20</const>
    </operand>
    <operand>
        <expression>
            <operation>minus</operation>
            <operand>
                <const>10</const>
            </operand>
            <operand>
                <const>5</const>
            </operand>
        </expression>
    </operand>
    </expression>
</request>";
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
                ConnectionButtonTitle = _isConnected ? "Disconnect" : "Connect";
            }
        }

        private string _connectionButtonTitle;
        public string ConnectionButtonTitle
        {
            get
            {
                return _connectionButtonTitle;
            }
            set
            {
                _connectionButtonTitle = value;
                OnPropertyChanged();
            }
        }

        private string _requestString;
        public string RequestString
        {
            get
            {
                return _requestString;
            }
            set
            {
                _requestString = value;
                OnPropertyChanged();
            }
        }

        private string _responseString;
        public string ResponseString
        {
            get
            {
                return _responseString;
            }
            set
            {
                _responseString = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
