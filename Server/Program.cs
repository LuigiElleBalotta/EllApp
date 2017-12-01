using System;
using System.Reflection;
using System.Threading;
using NLog;
using Server.Classes;
using Server.Commands;
using Server.Config;

namespace Server
{
    public static class Program
    {
		public static readonly Config.Config Configuration = ConfigReader.GetConfig();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static ServerContext Server;

        private static Thread socketThread;

        static void Main(string[] args)
        {

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Title = "EllApp WebSocket Server";
			Console.WriteLine("Running EllApp Socket Server ...");
			Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");

			if( Configuration != null ) {
                // Start the server  
                socketThread = new Thread( new ThreadStart( StartSocketServer ));
                socketThread.Start();

                string content = "";
                while (content != null && content != "exit")
                {  

                    if (content != "")
                    {
                        CommandHandlers cmd = new CommandHandlers();
                        Type type = cmd.GetType();
                        MethodInfo metodo = type.GetMethod(Utility.ToTitleCase(content));
                        switch (content)
                        {
                            case "fakemessage":
                            case "gsm":
                            case "online":
                                metodo.Invoke(cmd, new object[]{ Server.Sessions });
                                break;
                            case "serverinfo":
                                metodo.Invoke(cmd, new object[]{ Server });
                                break;
                            case "commands":
                                metodo.Invoke(cmd, null);
                                break;
                            case "createaccount":
                            case "clearconsole":
                                break;
                            default:
                                logger.Warn($"Unknown command \"{content}\".");
                                break;
                        }
                    }
                    content = Console.ReadLine();
                }

			} else {
				Console.WriteLine( "No configuration file found. Exit in 5 seconds.." );
				Thread.Sleep( 5 * 1000 );
                Environment.Exit( 0 );
			}
        }

        private static void StartSocketServer()
        {
            TcpHelper.StartListening( Configuration );
        }
    }
}
