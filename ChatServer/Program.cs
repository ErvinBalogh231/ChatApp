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
        static List<Client> _clients;
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
                _clients.Add(client);
                
                SaveUserToDb(client);
                BroadcastConnection();
            }
        }

        

        static void BroadcastConnection()
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

        static void SaveUserToDb(Client client)
        {
            _db.Users.Add(new User
            {
                Id = client.UId,
                UserName = client.Username,
                ConnectionTime = DateTime.Now
            });

            _db.SaveChanges();
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

        public static void SaveMessageToDb(Guid UId, string message)
        {
            _db.Messages.Add(new Message
            {
                SentTime = DateTime.Now,
                Content = message,
                UId = UId
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