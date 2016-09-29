using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EllApp_server.Classes
{
    class Log_Manager
    {
        Config_Manager conf = new Config_Manager();
        MySqlConnection conn = null;
        public string content = "", to_type = "";
        public int from = 0, to = 0;

        public Log_Manager()
        {
            conn = new MySqlConnection("Server=" + conf.getValue("mysql_host") + ";Database=" + conf.getValue("mysql_db") + ";Uid=" + conf.getValue("mysql_user") + ";Pwd=" + conf.getValue("mysql_password") + ";");
        }

        public void SaveLog()
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO log_chat(`content`, `from`, `to_type`, `to`) VALUES(@content, @from, @totype, @to);", conn);
            MySqlParameter _content = new MySqlParameter("@content", MySqlDbType.LongText, 0);
            MySqlParameter _from = new MySqlParameter("@from", MySqlDbType.Int32, 0);
            MySqlParameter _totype = new MySqlParameter("@totype", MySqlDbType.VarChar, 0);
            MySqlParameter _to = new MySqlParameter("@to", MySqlDbType.Int32, 0);
            _content.Value = content;
            _from.Value = from;
            _totype.Value = to_type;
            _to.Value = to;
            cmd.Parameters.Add(_content);
            cmd.Parameters.Add(_from);
            cmd.Parameters.Add(_totype);
            cmd.Parameters.Add(_to);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
