using System;
using System.Net;
using Server.Network.Alchemy.Classes;

namespace Server.Network
{
	public class Connection
	{
		public System.Threading.Timer timer;
		
		public UserContext Context { get; set; }
		public EndPoint IP { get; set; }

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
