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

        public Chat() { }
        public Chat(ChatType _type, string _text)
        {
            chattype = _type;
            text = _text;
        }
    }
}
