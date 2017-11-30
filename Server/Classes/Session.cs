using System;
using Newtonsoft.Json;
using Server.Network.Alchemy.Classes;

namespace Server.Classes
{
    public class Session
    {
        int ID;
        User user;
        UserContext context;

        public Session(int _id, User _user, UserContext _context)
        {
            ID = _id;
            user = _user;
            context = _context;
        }

        public UserContext GetContext()
        {
            return context;
        }

        public User GetUser()
        {
            return user;
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
