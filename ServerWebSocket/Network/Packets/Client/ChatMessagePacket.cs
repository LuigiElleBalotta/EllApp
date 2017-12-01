using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerWebSocket.definitions;

namespace ServerWebSocket.Network.Packets.Client
{
    public class ChatMessagePacket
    {
        public string Message { get; set; }
        public ChatType ToType { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
