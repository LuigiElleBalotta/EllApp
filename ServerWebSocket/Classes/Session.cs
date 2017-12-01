using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using ServerWebSocket.Classes.Entities;

namespace ServerWebSocket.Classes
{
    public class Session
    {
        int ID;
        public Account user { get; set; }
        public ClientContext context { get; set; }

        public Session(int _id, Account _user, ClientContext _context)
        {
            ID = _id;
            user = _user;
            context = _context;
        }

        public int GetID()
        {
            return ID;
        }

        public /*async*/ string CreateMessage(MessagePacket pkt)
        {
            var msg = JsonConvert.SerializeObject( pkt );
            Console.WriteLine($"Invio questo pacchetto: {msg}");
            return msg;
            /*var data = Encoding.UTF8.GetBytes( msg );
            var buffer = new ArraySegment<Byte>(new Byte[4096]);
            buffer = new ArraySegment<Byte>( data );
            await context.WebSocket.SendAsync( buffer, context.Type, true, context.Token );*/
        }
    }
}
