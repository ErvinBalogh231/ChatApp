using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer.Net
{
    internal class Client
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid UId { get; set; }
        public bool Authorized { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UId = Guid.NewGuid();
            Authorized = false;
            _packetReader = new PacketReader(ClientSocket.GetStream());

            Task.Run(() => ClientProcess());

        }

        public void ClientProcess()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 0:
                            Username = _packetReader.ReadMessage();
                            Password = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Client tries to connected with the username: {Username} and password: {Password}");

                            if (Program.IsAuthorized(this))
                            {
                                Authorized = true;
                                Console.WriteLine($"[{Username}] login authorized.");
                                Program.Authorize(this);
                                Program._clients.Add(this);
                                Program.BroadcastConnection();
                            }
                            else
                            {
                                Console.WriteLine($"[{Username}] tried to connect with invalid username or password.");
                                Program.DenyAcces(this);
                            }
                            break;

                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received from [{Username}]: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            Program.SaveMessageToDb(Username, msg);
                            break;
                        default:
                            break;
                    }
                }

                catch (Exception)
                {
                    Console.WriteLine($"[{Username}]: Disconnected!");
                    Program.BroadcastDisconnect(UId.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
