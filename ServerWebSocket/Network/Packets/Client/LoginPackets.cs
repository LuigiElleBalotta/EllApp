using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerWebSocket.Network.Packets.Client
{
    public class LoginPacket
    {
        public string Username { get; set; }
        public string Psw { get; set; }
        public bool WantWelcomeMessage { get; set; }
    }
}
