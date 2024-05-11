using ChatServerConsole.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerConsole
{
    internal class Program
    {
        static List<Client> _user;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _user = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("192.168.1.196"),7891);
            _listener.Start();
            while (true) 
            { 
                var client = new Client(_listener.AcceptTcpClient()); 
                _user.Add(client);

                BroadcastConnection();

            }
        }
        static void BroadcastConnection()
        {
            foreach (var user in _user) 
            {
                foreach(var usr in _user) 
                {
                    var broadcastPackets = new PacketBilder();
                    broadcastPackets.WriteOpCode(1);
                    broadcastPackets.WriteMessage(usr.UserName);
                    broadcastPackets.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPackets.GetPacketBytes());
                }
            }
        }
        public static void BroadcastMessage(string message)
        {
            foreach(var user  in _user)
            {
                var msgPacket = new PacketBilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _user.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _user.Remove(disconnectedUser);

            foreach (var user in _user)
            {
                var broadcastPacket = new PacketBilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            BroadcastMessage($"{disconnectedUser.UserName} отключился");
        }
    }
}
