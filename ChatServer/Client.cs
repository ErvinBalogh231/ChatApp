using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Client
    {
        public string Username {  get; set; }
        public Guid UId { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;
        public Client(TcpClient client) 
        {
            ClientSocket = client;
            UId = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
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
