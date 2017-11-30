using System;
using Lappa.ORM;
using Lappa.ORM.Constants;
using MySql.Data.MySqlClient;

namespace Server.Classes
{
    public class DB
    {
        public static Database EllAppDB = new Database();
		public bool Connected { get; set; }

		public DB( DatabaseType dbtype, Config.Config conf )
		{
			switch( dbtype ) {
				case DatabaseType.MySql:
					Connected = EllAppDB.Initialize( GetMySqlConnectionString( conf ), dbtype, false, false );
					break;
				case DatabaseType.MSSql:
					throw new NotImplementedException( "MSSQL non ancora implementato." );
					break;
			}

			
		}

        public static string GetMySqlConnectionString( Config.Config cfg )
        {
            return $"server={cfg.MySQLHost};uid={cfg.MySQLUser};pwd={cfg.MySQLPassword};database={cfg.MySQLDB};";
        }
    }
}
