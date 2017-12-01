using System;
using Lappa.ORM;
using Lappa.ORM.Constants;

namespace Server.Classes
{
    public class DB
    {
        public readonly Database EllAppDB;
		public bool Connected { get; set; }

		public DB( DatabaseType dbtype, Config.Config conf )
		{
            EllAppDB = new Database();
			switch( dbtype ) {
				case DatabaseType.MySql:
					Connected = EllAppDB.Initialize( GetMySqlConnectionString( conf ), dbtype, false, false, true );
					break;
				case DatabaseType.MSSql:
					throw new NotImplementedException( "MSSQL non ancora implementato." );
					break;
			}

			
		}

        public static string GetMySqlConnectionString( Config.Config conf )
        {
            return $"server={conf.MySQLHost};uid={conf.MySQLUser};pwd={conf.MySQLPassword};database={conf.MySQLDB};SslMode=none;AllowZeroDateTime=True;Convert Zero Datetime=True;";
        }
    }
}
