using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Network.Packets.Client
{
    public class RegistrationPacket
    {
        public string Username { get; set; }
        public string Psw { get; set; }
        public string Email { get; set; }
    }
}
