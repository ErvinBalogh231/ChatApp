using ChatApp.Net.IO;
using System.Net.Sockets;

namespace ChatApp.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action userConnectedEvent;
        public event Action loginSucceedEvent;
        public event Action loginFailedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;

        public Server() {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username, string password)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());
                ReadPackets();
            }

            var connectPacket = new PacketBuilder();
            connectPacket.WriteOpCode(0);
            connectPacket.WriteMessage(username);
            connectPacket.WriteMessage(password);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            userConnectedEvent?.Invoke();
                            break;
                        case 2:
                            loginSucceedEvent?.Invoke();
                            break;
                        case 3:
                            loginFailedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Invalid operation code!");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
