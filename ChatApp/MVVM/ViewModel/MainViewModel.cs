using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.Net;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand {  get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        private Server _server;
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();

            _server.userConnectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;
            _server.loginFailedEvent += LoginFailed;
            _server.loginSucceedEvent += LoginSucceedEvent;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username, Password), o => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !IsLoggedIn);
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message) && IsLoggedIn);
        }

        private void LoginSucceedEvent()
        {
            IsLoggedIn = true;
        }

        private void LoginFailed()
        {
            MessageBox.Show("Invalid username or password!");
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UId == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.PacketReader.ReadMessage(),
                UId = _server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UId == user.UId))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }
    }
}
