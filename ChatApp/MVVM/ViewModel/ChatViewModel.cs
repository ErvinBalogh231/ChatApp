using ChatApp.Core;
using ChatApp.MVVM.Model;
using ChatApp.Net;
using ChatApp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        
        public RelayCommand SendMessageCommand { get; set; }

        private string _loggedInUser = "Invalid";
        public string LoggedInUser 
        { 
            get { return _loggedInUser; }
            set 
            {
                _loggedInUser = value;
                OnPropertyChanged();
            }
        }
        private string _message;
        public string Message 
        {
            get {  return _message; }
            set 
            { 
                _message = value;
                OnPropertyChanged();
            }
        }
        
        private Server _server;
        private LoggedInUserService _loggedInUserService;


        public ChatViewModel(Server server, LoggedInUserService loggedInUserService)
        {
            _server = server;
            _loggedInUserService = loggedInUserService;

            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<MessageModel>();

            _server.userConnectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;

            SendMessageCommand = new RelayCommand(o => 
            { 
                _server.SendMessageToServer(Message);
                Message = "";
            }, 
            o => !string.IsNullOrEmpty(Message));
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
            var username = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new MessageModel
            {
                Username = username,
                Message = msg,
                SentTime = DateTime.Now
            }));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UId = _server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UId == user.UId) && user.Username != _loggedInUserService.CurrentUser)
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }
    }
}
