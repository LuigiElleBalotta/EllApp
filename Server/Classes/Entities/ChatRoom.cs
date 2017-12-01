using System;
using System.Collections.Generic;
using System.Text;
using Lappa.ORM;
using Server.definitions;

namespace Server.Classes.Entities
{
    public class ChatRoom : Entity
    {
        public int ID { get; set; }
        public ChatType Type { get; set; }
        public string Name { get; set; }
    }
}
