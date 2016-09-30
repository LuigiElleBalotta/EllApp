using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;

namespace EllApp_server.Classes
{
    class Session
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

        public void SendMessage(MessagePacket pkt)
        {
            var msg = JsonConvert.SerializeObject(pkt);
            Console.WriteLine(msg);
            context.Send(msg);
        }
    }
}
