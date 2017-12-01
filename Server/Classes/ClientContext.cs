using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.Classes
{
    public class ClientContext
    {
        public Socket Socket { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }
}
