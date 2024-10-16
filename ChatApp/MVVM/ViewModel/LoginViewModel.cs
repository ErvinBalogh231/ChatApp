using ChatApp.Core;
using ChatApp.Net;
using ChatApp.Services;

namespace ChatApp.MVVM.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public RelayCommand ConnectToServerCommand { get; set; }

        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set 
            { 
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                OnPropertyChanged();
            }
        }
        public string Username
        {
            get { return _username; }
            set 
            { 
                _username = value;
                OnPropertyChanged();
            }
        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private Server _server;
        private readonly LoggedInUserService _loggedInUserService;


        public LoginViewModel(Server server, LoggedInUserService loggedInUserService)
        {
            _server = server;
            _loggedInUserService = loggedInUserService;
            IsVisible = true;
            ErrorMessage = "";

            ConnectToServerCommand = new RelayCommand(o => {
                _server.ConnectToServer(Username, Password);
            }, 
            o => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));

            _server.loginFailedEvent += LoginFailed;
            _server.loginSucceedEvent += LoginSucceed;
        }

        private void LoginSucceed()
        {
            _loggedInUserService.CurrentUser = Username;
            IsVisible = false;
        }

        private void LoginFailed()
        {
            ErrorMessage = "Invalid username or password!";
            Username = "";
            Password = "";
        }
    }
}
