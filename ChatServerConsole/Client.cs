using ChatServerConsole.Net.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerConsole
{
    internal class Client
    {
        public string UserName { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;
        public Client(TcpClient client) 
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            UserName = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]: Произошло подлючение под именем: {UserName}");

            Task.Run(() => Process());
        }

        void Process() 
        { 
            while(true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch(opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now.ToShortDateString()}: Message: {msg}]");
                            Program.BroadcastMessage( $"[{DateTime.Now.ToShortDateString()}]:{UserName}: {msg}" );
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception) 
                {
                    Console.WriteLine($"[{UID.ToString()}: Disconnected]");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
 