using System;
using Newtonsoft.Json;
using Server.Classes.Entities;
using Server.Network.Alchemy.Classes;

namespace Server.Classes
{
    public class Session
    {
        int ID;
        public Account user { get; set; }
        public UserContext context { get; set; }

        public Session(int _id, Account _user, UserContext _context)
        {
            ID = _id;
            user = _user;
            context = _context;
        }

        public int GetID()
        {
            return ID;
        }

        public void SendMessage(MessagePacket pkt)
        {
            var msg = JsonConvert.SerializeObject(pkt);
            Console.WriteLine($"Invio questo pacchetto: {msg}");
            context.Send(msg);
        }
    }
}
