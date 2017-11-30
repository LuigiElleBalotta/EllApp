using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;
using EllApp_server.Classes;
using System.Configuration;
using System.Reflection;
using EllApp_server.Commands;
using EllApp_server.Network;
using NLog;

namespace EllApp_server
{
    class Program
    {
        //Thread-safe collection of Online Connections.
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public static List<Session> Sessions = new List<Session>();
	    private static Logger logger = LogManager.GetCurrentClassLogger();
	    private static WebSocketServer aServer;

        static void Main(string[] args)
        {
            // instantiate a new server - acceptable port and IP range,
            // and set up your methods.

            aServer = new WebSocketServer(Convert.ToInt16(ConfigurationSettings.AppSettings["serverport"]), System.Net.IPAddress.Any)
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

            //Open DB Instance
            DB.Instantiate();

            // Accept commands on the console and keep it alive
            var command = string.Empty;
            while (command != "exit")
            {
                if (command != "")
                {
					CommandHandlers cmd = new CommandHandlers();
	                Type type = cmd.GetType();
	                MethodInfo metodo = type.GetMethod(Utility.ToTitleCase(command));
                    switch (command)
                    {
	                    case "fakemessage":
						case "gsm":
                        case "online":
	                        metodo.Invoke(cmd, new object[]{ Sessions });
                            break;
                        case "serverinfo":
	                        metodo.Invoke(cmd, new object[]{ aServer });
                            break;
						case "commands":
							metodo.Invoke(cmd, null);
							break;
                        case "createaccount":
                        case "clearconsole":
                            break;
                        default:
                            logger.Warn($"Unknown command \"{command}\".");
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

            logger.Info("Client Connected From : " + aContext.ClientAddress);

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext, IP = aContext.ClientAddress };

            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
			Sessions.Add(new Session(0, null, aContext));
        }
        public static void OnReceive(UserContext aContext)
        {
            try
            {
				ClassChooser.Handle(aContext, Sessions, OnlineConnections);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void OnSend(UserContext aContext)
        {
	        logger.Info("Data Sent To : " + aContext.ClientAddress);
        }
		
        public static void OnDisconnect(UserContext aContext)
        {
	        logger.Info("Client Disconnected : " + aContext.ClientAddress);
	        Sessions = Sessions;
            // Remove the connection Object from the thread-safe collection
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
	        if (conn != null) //E' riuscito a rimuovere la connessione.
	        {
		        Sessions.Remove(Sessions.First(s =>
											   {
												   bool res = (s.GetContext().ClientAddress == conn.IP);
												   if (res)
												   {
													   if(s.GetID() > 0)
													       s.GetUser().SetOffline();   
												   }
												   return res;
											   }));
		        // Dispose timer to stop sending messages to the client.
		        conn.timer.Dispose();
	        }
            
        }

        

    }
}