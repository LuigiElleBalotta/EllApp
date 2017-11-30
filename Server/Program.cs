using System;
using System.Threading;
using Lappa.ORM.Constants;
using Server.Classes;
using Server.Config;

namespace Server
{
    public static class Program
    {
		public static readonly Config.Config Configuration = ConfigReader.GetConfig();
        public static DB mysql = new DB( DatabaseType.MySql, Configuration );
        static void Main(string[] args)
        {

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Title = "EllApp WebSocket Server";
			Console.WriteLine("Running EllApp Socket Server ...");
			Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");

			if( Configuration != null ) {

                if( mysql.Connected ) {
                    // Start the server  
                    TcpHelper.StartListening();
                } else {
                    Console.WriteLine( "Cannot connect to database." );
                    Thread.Sleep( 5 * 1000 );
                    Environment.Exit( 0 );
                }
				
			} else {
				Console.WriteLine( "No configuration file found. Exit in 5 seconds.." );
				Thread.Sleep( 5 * 1000 );
                Environment.Exit( 0 );
			}
        }
    }
}
