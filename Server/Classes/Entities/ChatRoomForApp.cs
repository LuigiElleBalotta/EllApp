using System;
using System.Collections.Generic;
using System.Text;
using Server.definitions;

namespace Server.Classes.Entities
{
    public class ChatRoomForApp
    {
        public int ChatRoomID { get; set; }
        public string ChatRoomName { get; set; }
        public string Destinatario { get; set; }
        public int DestinatarioID { get; set; }
        public ChatType Type { get; set; }
    }
}
