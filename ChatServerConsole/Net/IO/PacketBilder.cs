using System;
using System.IO;
using System.Text;

namespace ChatServerConsole.Net.IO
{
    class PacketBilder
    {
        MemoryStream _ms;
        public PacketBilder()
        {
            _ms = new MemoryStream();
        }
        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }
        public void WriteMessage(string msg)
        {
            var msgLenght = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLenght), 0, BitConverter.GetBytes(msgLenght).Length);
            _ms.Write(Encoding.ASCII.GetBytes(msg), 0, Encoding.ASCII.GetBytes(msg).Length);
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}
