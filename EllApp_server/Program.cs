using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using EllApp_server.Classes;
using EllApp_server.definitions;
using System.Diagnostics;

namespace EllApp_server
{
    class Program
    {
        public static Config_Manager config = new Config_Manager();
        //Thread-safe collection of Online Connections.
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public static List<Session> Sessions = new List<Session>();

        static void Main(string[] args)
        {
            // instantiate a new server - acceptable port and IP range,
            // and set up your methods.

            var aServer = new WebSocketServer(Convert.ToInt16(config.getValue("serverport")), System.Net.IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            aServer.Start();


            Console.ForegroundColor = ConsoleColor.Red;
            Console.Title = "EllApp WebSocket Server";
            Console.WriteLine("Running EllApp WebSocket Server ...");
            Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");


            // Accept commands on the console and keep it alive
            var command = string.Empty;
            while (command != "exit")
            {
                if (command != "")
                {
                    switch (command)
                    {
                        case "online":
                            Console.WriteLine("Online Users: " + GetOnlineUsers());
                            foreach(var session in Sessions)
                            {
                                Console.WriteLine("User ID: " + session.GetUser().GetID());
                                Console.WriteLine("User Name: " + session.GetUser().GetUsername());
                                Console.WriteLine("Connected from: " + session.GetContext().ClientAddress);
                                Console.WriteLine("-------------------------------------");
                            }
                            break;
                        case "serverinfo":
                            Console.WriteLine("Listening on: " + aServer.ListenAddress);
                            Console.WriteLine("Port: " + aServer.Port);
                            break;
                        case "gsm": //Global Server Message
                            foreach (var session in Sessions)
                            {
                                Console.WriteLine("Message to send: ");
                                var msg = Console.ReadLine();
                                Chat c = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, "", msg, "Server Message", session.GetUser().GetUsername());
                                var message = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, session.GetUser().GetID(), JsonConvert.SerializeObject(c));
                                session.SendMessage(message);
                            }
                            break;
                        case "fakemessage":
                            Console.WriteLine("Insert the username: ");
                            string uname = Console.ReadLine().ToUpper();
                            bool validChoice = false;
                            do
                            {
                                Console.WriteLine("Single chat or Group Chat? (1 or 2)");
                                int choice = Convert.ToInt16(Console.ReadLine());
                                switch (choice)
                                {
                                    case 1:
                                        validChoice = true;
                                        Console.WriteLine("Insert the destinatary username:");
                                        string duname = Console.ReadLine().ToUpper();
                                        Console.WriteLine("Insert your message: ");
                                        string tmpmessage = Console.ReadLine();

                                        int from = Misc.GetUserIDByUsername(uname);
                                        int to = Misc.GetUserIDByUsername(duname);
                                        string chatroomid = Misc.CreateChatRoomID(to, from);

                                        Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, chatroomid, tmpmessage, uname, duname);
                                        var msg = new MessagePacket(MessageType.MSG_TYPE_CHAT, from, to, JsonConvert.SerializeObject(c));

                                        Console.WriteLine($"Sending message to {to} - {duname}");
                                        if (Sessions.Any(s => s.GetUser().GetID() == to))
                                        {
                                            Console.WriteLine("L'utente è nella lista delle sessioni");
                                            if (Sessions.First(s => s.GetUser().GetID() == to).GetUser().IsOnline())
                                            {
                                                Console.WriteLine("L'utente è online");
                                                Session session = Sessions.SingleOrDefault(s => s.GetUser().GetID() == to);
                                                Console.WriteLine("Sending message to user");
                                                session.SendMessage(msg);
                                            }
                                        }

                                        var log = new Log_Manager();
                                        log.ChatID = chatroomid;
                                        log.content = tmpmessage;
                                        log.to_type = ChatType.CHAT_TYPE_USER_TO_USER;
                                        log.from = from;
                                        log.to = to;
                                        log.SaveLog();
                                        Console.WriteLine(JsonConvert.SerializeObject(msg));
                                        Console.WriteLine("Done.");
                                        break;
                                    case 2:
                                        validChoice = true;
                                        Console.WriteLine("Not yet implemented.");
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice");
                                        break;
                                }
                            } while (!validChoice);
                            break;
                        case "clearconsole":
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            break;
                    }
                }
                command = Console.ReadLine();
            }

            aServer.Stop();
            Environment.Exit(0);
        }

        public static void OnConnect(UserContext aContext)
        {

            Console.WriteLine("Client Connected From : " + aContext.ClientAddress.ToString());

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext };

            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
        }



        public static void OnReceive(UserContext aContext)
        {
            try
            {
                //Console.WriteLine("Data Received From [" + aContext.ClientAddress.ToString() + "] - " + aContext.DataFrame.ToString());

                var json = aContext.DataFrame.ToString();
                dynamic obj = JsonConvert.DeserializeObject(json);

                switch ((int)obj.Type)
                {
                    case (int)CommandType.Login:
                        Console.WriteLine("LOGIN REQUEST FROM " + aContext.ClientAddress);
                        Console.WriteLine(json);
                        string username = (string)obj.Username;
                        string psw = (string)obj.Psw;
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

                            var loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.GetUser().GetID(), s.GetUser().GetID().ToString());

                            //Create the welcome message object
                            Chat c = new Chat();
                            c.chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT;
                            c.text = "Benvenuto " + s.GetUser().GetUsername();
                            string output = JsonConvert.SerializeObject(c);
                            var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.GetUser().GetID(), output);
                            s.GetUser().SetOnline();
                            s.SendMessage(loginInfo);
                            s.SendMessage(welcomeMessage);
                        }
                        break;
                    case (int)CommandType.Message:
                        Console.WriteLine("MESSAGE PACKET FROM " + aContext.ClientAddress);
                        string messagecontent = obj.Message;
                        ChatType to_type = obj.ToType;
                        int from = obj.From;
                        int to = obj.To;
                        var log = new Log_Manager();
                        log.ChatID = Misc.CreateChatRoomID(from, to);
                        log.content = messagecontent;
                        log.to_type = to_type;
                        log.from = from;
                        log.to = to;
                        log.SaveLog();
                        switch (to_type)
                        {
                            case ChatType.CHAT_TYPE_GLOBAL_CHAT: //Send message to all connected clients (that we have stored in sessions)
                                var StCLog = new Log_Manager();
                                var o = 1;
                                foreach (var session in Sessions)
                                {
                                    if (session.GetUser().GetID() != from) //Do not send message to ourselves
                                    {
                                        Chat c = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, Misc.CreateChatRoomID(from, session.GetUser().GetID()), messagecontent, Misc.GetUsernameByID(from), Misc.GetUsernameByID(session.GetUser().GetID()));
                                        session.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, from, session.GetUser().GetID(), JsonConvert.SerializeObject(c)));
                                        StCLog.content = messagecontent;
                                        StCLog.to_type = to_type;
                                        StCLog.from = from;
                                        StCLog.to = session.GetUser().GetID();
                                        StCLog.SaveLog();
                                        o++;
                                    }
                                }
                                Console.WriteLine("Message sent to {0} users", (o - 1));
                                break;
                            case ChatType.CHAT_TYPE_USER_TO_USER:
                                Console.WriteLine("Received MSG_TYPE_CHAT_WITH_USER packet.");
                                //If the receiving User is online, we can send the message to him, otherwise he will load everything at next login
                                if (Sessions.Any(s => s.GetUser().GetID() == (int)obj.To))
                                {
                                    if (Sessions.SingleOrDefault(s => s.GetUser().GetID() == (int)obj.To).GetUser().IsOnline())
                                    {
                                        Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, Misc.CreateChatRoomID(obj.To, obj.From), obj.Message, obj.From, obj.To);
                                        Session session = Sessions.SingleOrDefault(s => s.GetUser().GetID() == (int)obj.To);
                                        session.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, obj.From, obj.To, JsonConvert.SerializeObject(c)));
                                    }
                                }
                                else
                                    Console.WriteLine("DEBUG: The receiver was not online, message will be read at next login");
                                break;
                            case ChatType.CHAT_TYPE_GROUP_CHAT:
                                Console.WriteLine("MSG_TYPE_CHAT_WITH_GROUP NOT YET IMPLEMENTED");
                                break;
                            case ChatType.CHAT_TYPE_NULL:
                                Console.WriteLine("Message Type is NULL.");
                                break;
                        }
                        break;
                    case (int)CommandType.ChatsRequest:
                        Console.WriteLine("Received CHAT REQUEST from AccountID = " + obj.accid + ".");
                        int accountID = obj.accid;
                        string ChatRequestID = "";
                        if (obj.ChatRequestID != null)
                            ChatRequestID = obj.ChatRequestID;
                        string chats = JsonConvert.SerializeObject(User.GetChats(accountID, ChatRequestID));
                        Sessions.First(s => s.GetUser().GetID() == accountID).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE, 0, accountID, chats));
                        break;
                    case (int)CommandType.ChatListRequest:
                        Console.WriteLine("Received CHAT LIST REQUEST from AccountID = " + obj.accid + ".");
                        int accountid = obj.accid;
                        string chatlist = JsonConvert.SerializeObject(User.GetChats(accountid, ""));
                        Sessions.First(s => s.GetUser().GetID() == accountid).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE, 0, accountid, chatlist));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }
        public static void OnSend(UserContext aContext)
        {
            Console.WriteLine("Data Sent To : " + aContext.ClientAddress.ToString());
        }

        public static void OnDisconnect(UserContext aContext)
        {
            Console.WriteLine("Client Disconnected : " + aContext.ClientAddress.ToString());

            // Remove the connection Object from the thread-safe collection
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
            var sessionlist = Sessions.Where(s => s.GetContext().ClientAddress.ToString() == aContext.ClientAddress.ToString());
            foreach (var s in sessionlist)
                s.GetUser().SetOffline();
            Sessions.Remove(Sessions.First(s => s.GetContext().ClientAddress.ToString() == aContext.ClientAddress.ToString()));
            // Dispose timer to stop sending messages to the client.
            conn.timer.Dispose();
        }

        public static string GetOnlineUsers()
        {
            return Sessions.Count.ToString();
        }

    }

    public class Connection
    {
        public System.Threading.Timer timer;
        public UserContext Context { get; set; }
        public Connection()
        {
            this.timer = new System.Threading.Timer(this.TimerCallback, null, 0, 1000);
        }

        private void TimerCallback(object state)
        {
            try
            {
                // Sending Data to the Client
                //Context.Send("[" + Context.ClientAddress.ToString() + "] " + System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

}