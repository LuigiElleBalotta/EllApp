using System;
using System.Threading;
using Server.Config;

namespace Server
{
    public static class Program
    {
		public static readonly Config.Config Configuration = ConfigReader.GetConfig();
        static void Main(string[] args)
        {

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Title = "EllApp WebSocket Server";
			Console.WriteLine("Running EllApp Socket Server ...");
			Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");

			if( Configuration != null ) {
				// Start the server  
                TcpHelper.StartListening();
			} else {
				Console.WriteLine( "No configuration file found. Exit in 5 seconds.." );
				Thread.Sleep( 5 * 1000 );
			}
        }
    }
}
