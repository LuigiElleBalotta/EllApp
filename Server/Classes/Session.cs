using System;
using Newtonsoft.Json;
using Server.Classes.Entities;

namespace Server.Classes
{
    public class Session
    {
        public string ID { get; set; }
        public Account user { get; set; }
        public ClientContext context { get; set; }

        public Session(string _id, Account _user, ClientContext _context)
        {
            ID = _id;
            user = _user;
            context = _context;
        }

        public string CreateResponse(MessagePacket pkt)
        {
            var msg = JsonConvert.SerializeObject(pkt);
            Console.WriteLine($"Invio questo pacchetto: {msg}");
            return msg;
        }
    }
}
