using Alchemy.Classes;
using EllApp_server;
using EllApp_server.Classes;
using EllApp_server.definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EllApp_server.Network.Handlers
{
    public class LoginHandler
    {

        public static void DoLogin(UserContext aContext, List<Session> Sessions, ConcurrentDictionary<string, Connection> OnlineConnections, dynamic obj, string json)
        {
	        Console.WriteLine("LOGIN REQUEST FROM " + aContext.ClientAddress);
	        Console.WriteLine(json);
	        string username = (string)obj.Username;
	        string psw = (string)obj.Psw;
	        bool WantWelcomeMessage = Convert.ToBoolean((int)obj.WantWelcomeMessage);
	        User u = new User(username.ToUpper(), psw.ToUpper());
	        if (!u.Validate())
	        {
		        Connection conn;
		        OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
		        conn.timer.Dispose();
	        }
	        else
	        {
		        Session s = new Session(u.GetID(), u, aContext);
		        Sessions.Add(s);

		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.GetUser().GetID(), s.GetUser().GetID().ToString());
                            
		        s.SendMessage(loginInfo);
		        if(WantWelcomeMessage)
		        {
			        //Create the welcome message object
			        Chat c = new Chat();
			        c.chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT;
			        c.text = "Benvenuto " + s.GetUser().GetUsername();
			        string output = JsonConvert.SerializeObject(c);
			        s.GetUser().SetOnline();
			        var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.GetUser().GetID(), output);
			        s.SendMessage(welcomeMessage);
		        }
	        }
        }
    }
}
