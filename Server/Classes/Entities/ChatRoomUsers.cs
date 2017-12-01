using System;
using System.Collections.Generic;
using System.Text;
using Lappa.ORM;

namespace Server.Classes.Entities
{
    public class ChatRoomUsers : Entity
    {
        public int IDChatRoom { get; set; }
        public int IDAccount { get; set; }
    }
}
