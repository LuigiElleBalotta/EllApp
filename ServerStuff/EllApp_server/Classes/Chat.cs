using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    public class Chat
    {
        public ChatType chattype { get; set; }
        public string ChatRoom { get; set; }
        public string text { get; set; }
        public string ChatFrom { get; set; }
        public string ChatTo { get; set; }
        public long timestamp { get; set; }

        public Chat() { }
        public Chat(ChatType _type, string _chatroom, string _text, string _from, string _to, long _timestamp = 0)
        {
            ChatRoom = _chatroom;
            chattype = _type;
            text = _text;
            ChatFrom = _from;
            ChatTo = _to;
            timestamp = _timestamp;
            if (timestamp == 0)
                timestamp = Misc.UnixTimeNow();
        }

        
    }
}
