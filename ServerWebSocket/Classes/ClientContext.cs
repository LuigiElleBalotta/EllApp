using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ServerWebSocket.Classes
{
    public class ClientContext
    {
        public WebSocket WebSocket { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }
}
