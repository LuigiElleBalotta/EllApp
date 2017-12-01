using Lappa.ORM;
using Server.definitions;

namespace Server.Classes
{
	[DBTable( Pluralize = false, Name = "logs")]
    public class Log_Manager : Entity
    {
        Database db;
        public string ChatID = "";
        public string content = "";
        public ChatType to_type = ChatType.CHAT_TYPE_NULL;
        public int from = 0, to = 0;

        public Log_Manager()
        {
            db = Utils.mysqlDB.EllAppDB;
        }

        public void SaveLog()
        {
			//db.Insert<Log_Manager>( this );
        }
    }
}
