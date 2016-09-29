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

            var aServer = new WebSocketServer(Convert.ToInt16(config.getValue("serverport")), System.Net.IPAddress.Parse("127.0.0.1"))
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
            Console.WriteLine("Host: " + aServer.ListenAddress);
            Console.WriteLine("Port: " + aServer.Port);
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
                        string username = (string)obj.Username;
                        string psw = (string)obj.Psw;
                        User u = new User(username.ToUpper(), psw.ToUpper());
                        if(!u.Validate())
                        {
                            Connection conn;
                            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
                            conn.timer.Dispose();
                        }
                        else
                        {
                            Session s = new Session(u.GetID(), u, aContext);
                            Sessions.Add(s);
                            var welcomeMessage = new MessagePacket(0, s.GetUser().GetID(), "Benvenuto " + s.GetUser().GetUsername());
                            s.GetUser().SetOnline();
                            s.SendMessage(welcomeMessage);
                        }
                        break;
                    case (int)CommandType.Message:
                        string messagecontent = obj.Message;
                        string to_type = obj.ToType;
                        int from = obj.From;
                        int to = obj.To;
                        var log = new Log_Manager();
                        log.content = messagecontent;
                        log.to_type = to_type;
                        log.from = from;
                        log.to = to;
                        log.SaveLog();
                        if (to_type == "globalchat")
                        {
                            var StCLog = new Log_Manager();
                            var o = 1;
                            foreach (var session in Sessions)
                            {
                                if (session.GetUser().GetID() != from)
                                {

                                    session.SendMessage(new MessagePacket(from, 0, messagecontent));
                                    StCLog.content = messagecontent;
                                    StCLog.to_type = to_type;
                                    StCLog.from = from;
                                    StCLog.to = session.GetUser().GetID();
                                    StCLog.SaveLog();
                                    o++;
                                }
                            }
                            Console.WriteLine("Message sent to {0} users", (o - 1));
                        }
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
        Config_Manager config = null;
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