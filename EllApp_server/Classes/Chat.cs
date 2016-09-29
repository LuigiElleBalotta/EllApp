using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    class Chat
    {
        public ChatType chattype;
        public string text;
        public string ChatFrom;
        public string ChatTo;
        public long timestamp;

        public Chat() { }
        public Chat(ChatType _type, string _text, string _from, string _to, long _timestamp = 0)
        {
            chattype = _type;
            text = _text;
            ChatFrom = _from;
            ChatTo = _to;
            timestamp = _timestamp;
            /*if (timestamp == 0)
                timestamp = Misc.UnixTimeNow();*/
        }

        
    }
}
