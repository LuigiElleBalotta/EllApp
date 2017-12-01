using System.Net.Sockets;

namespace Server.Classes
{
    public class ClientContext
    {
        public Socket Socket { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }
}
