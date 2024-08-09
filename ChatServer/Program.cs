using ChatServer.Database.Data;
using ChatServer.Database.Models;
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

            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _clients.Add(client);

                User user = new User();
                user.Id = client.UId;
                user.UserName = client.Username;
                user.ConnectionTime = DateTime.Now;

                _db.Users.Add(user);
                _db.SaveChanges();

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