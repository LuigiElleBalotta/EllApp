using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy.Classes;

namespace EllApp_server.Network
{
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
