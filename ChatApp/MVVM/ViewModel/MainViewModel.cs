using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand {  get; set; }
        public RelayCommand SendMessageCommand {  get; set; }

        public string UserName {  get; set; }
        public string Message { get; set; }


        private Server _server;

        public MainViewModel() 
        {
            Users = new ObservableCollection<UserModel>(); 
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgRecivedEvent += MessageRecived;
            _server.userDisconnectEvent += RemoveUser;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(UserName), o => !string.IsNullOrEmpty(UserName));

            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));

        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageRecived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
