using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.definitions;

namespace Server.Classes
{
    public class Response
    {
        public MessageType MessageType { get; set; }
        public ResponseType ResponseType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
