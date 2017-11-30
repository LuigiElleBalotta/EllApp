using System;
using System.IO;
using Newtonsoft.Json;

namespace Server.Config
{
    public static class ConfigReader
    {
		public static Config GetConfig()
		{
			Config ret = null;
			try 
			{
				ret = JsonConvert.DeserializeObject<Config>( File.ReadAllText( "config.json" ));
			}
			catch( FileNotFoundException ex ) {
				Console.WriteLine( ex.Message );
			}

			return ret;
		}
    }
}
