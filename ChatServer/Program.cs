using ChatServer.Database.Data;
using ChatServer.Database.Models;
using ChatServer.Net;
using ChatServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        public static List<Client> _clients;
        static TcpListener _listener;
        static ChatServerContext _db;

        static void Main(string[] args) 
        {
            _clients = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _db = new ChatServerContext();

            ServerConsole.Run(_db);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
            }
        }

        public static bool IsAuthorized(Client client)
        {
            bool isAuthorized = false;
            _db.Users.ToList().ForEach(user =>
            {
                if (user.UserName == client.Username && user.Password == client.Password) { isAuthorized = true; }
            });
            return isAuthorized;
        }

        public static void Authorize(Client client)
        {
            var loginSucceedPacket = new PacketBuilder();
            loginSucceedPacket.WriteOpCode(2);
            client.ClientSocket.Client.Send(loginSucceedPacket.GetPacketBytes());
        }
        public static void DenyAcces(Client client)
        {
            var authFailedPacket = new PacketBuilder();
            authFailedPacket.WriteOpCode(3);
            client.ClientSocket.Client.Send(authFailedPacket.GetPacketBytes());
        }
        public static void BroadcastConnection()
        {
            foreach (var client in _clients)
            {
                foreach (var cli in _clients)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(cli.Username);
                    broadcastPacket.WriteMessage(cli.UId.ToString());
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach(var client in _clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void SaveMessageToDb(string username, string message)
        {
            _db.Messages.Add(new Message
            {
                SentTime = DateTime.Now,
                Content = message,
                Username = username
            });
            _db.SaveChanges();
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _clients.Where(x => x.UId.ToString() == uid).FirstOrDefault();
            _clients.Remove(disconnectedUser);
            foreach (var client in _clients)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                client.ClientSocket.Client?.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
        }
    }
}