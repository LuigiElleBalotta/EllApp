using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Server.Network;

namespace Server.Classes
{
    public class ServerContext
    {
        public Socket Socket { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public List<Session> Sessions = new List<Session>();
    }
}
