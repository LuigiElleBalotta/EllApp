using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    class Log_Manager
    {
        MySqlConnection conn = null;
        public string ChatID = "";
        public string content = "";
        public ChatType to_type = ChatType.CHAT_TYPE_NULL;
        public int from = 0, to = 0;

        public Log_Manager()
        {
            conn = DB.EllAppDB;
        }

        public void SaveLog()
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO log_chat(ChatID, `content`, `from`, `to_type`, `to`) VALUES(@chatid, @content, @from, @totype, @to);", conn);
            MySqlParameter _chatid = new MySqlParameter("@chatid", MySqlDbType.VarChar, 0);
            MySqlParameter _content = new MySqlParameter("@content", MySqlDbType.LongText, 0);
            MySqlParameter _from = new MySqlParameter("@from", MySqlDbType.Int32, 0);
            MySqlParameter _totype = new MySqlParameter("@totype", MySqlDbType.VarChar, 0);
            MySqlParameter _to = new MySqlParameter("@to", MySqlDbType.Int32, 0);
            _chatid.Value = ChatID;
            _content.Value = content;
            _from.Value = from;
            _totype.Value = to_type;
            _to.Value = to;
            cmd.Parameters.Add(_chatid);
            cmd.Parameters.Add(_content);
            cmd.Parameters.Add(_from);
            cmd.Parameters.Add(_totype);
            cmd.Parameters.Add(_to);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }
    }
}
