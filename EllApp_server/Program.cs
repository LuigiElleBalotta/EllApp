using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Threading;
using EllApp_server.Classes;

namespace EllApp_server
{
    class Program
    {
        public static Config_Manager config = new Config_Manager();
        //Thread-safe collection of Online Connections.
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();

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
                command = Console.ReadLine();
            }

            aServer.Stop();

        }

        public static void OnConnect(UserContext aContext)
        {

            Console.WriteLine("Client Connected From : " + aContext.ClientAddress.ToString());

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext };

            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
            conn.WelcomeMessage();
        }



        public static void OnReceive(UserContext aContext)
        {
            try
            {
                Console.WriteLine("Data Received From [" + aContext.ClientAddress.ToString() + "] - " + aContext.DataFrame.ToString());
                var log = new Log_Manager();
                log.content = aContext.DataFrame.ToString();  
                log.to_type = "globalchat";                       //this needs to be taken from user session
                log.from = 1;                                     //this too
                log.to = 0;                                       //this too
                log.SaveLog();

                if ("globalchat" == "globalchat") //At the moment there will be this stupid IF statement. When I'll know how to decide the chat type this will change
                {
                    var StCLog = new Log_Manager();
                    var o = 1;
                    foreach(var u in OnlineConnections.Values)
                    {
                        if (u.Context.ClientAddress.ToString() != aContext.ClientAddress.ToString())
                        {
                            u.Context.Send(aContext.ClientAddress.ToString()+"|" + aContext.DataFrame.ToString());
                            StCLog.content = aContext.DataFrame.ToString();
                            StCLog.to_type = "globalchat";                       //this needs to be taken from user session
                            StCLog.from = 0;                                     //this too
                            StCLog.to = o++;                                       //this too
                            StCLog.SaveLog();
                        }
                    }
                    Console.WriteLine("Message sent to {0} users", (o - 1));
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

            // Dispose timer to stop sending messages to the client.
            conn.timer.Dispose();
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

        public void WelcomeMessage()
        {
            config = new Config_Manager();
            Context.Send("Server Message|"+config.getValue("WelcomeMessage"));
        }

    }

}