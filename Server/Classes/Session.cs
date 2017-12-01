using System;
using Newtonsoft.Json;
using Server.Classes.Entities;
using Server.Network.Alchemy.Classes;

namespace Server.Classes
{
    public class Session
    {
        string ID;
        public Account user { get; set; }
        public ClientContext context { get; set; }

        public Session(string _id, Account _user, ClientContext _context)
        {
            ID = _id;
            user = _user;
            context = _context;
        }

        public string GetID()
        {
            return ID;
        }

        public void SendMessage(MessagePacket pkt)
        {
            var msg = JsonConvert.SerializeObject(pkt);
            Console.WriteLine($"Invio questo pacchetto: {msg}");
            //context.Send(msg);
        }
    }
}
