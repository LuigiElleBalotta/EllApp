using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Network.Packets.Client
{
    public class ChatsRequestPacket
    {
        public int AccID { get; set; }
        public string ChatRequestID { get; set; }
    }
}
