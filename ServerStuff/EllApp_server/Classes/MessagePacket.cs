using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;
using MySql.Data.MySqlClient;

namespace EllApp_server.Classes
{
    public class MessagePacket
    {
        public MessageType MessageType;
        public string from;
        public int to;
        public string data;

        public MessagePacket(MessageType _msgtype, int _from, int _to, string _data)
        {
            MessageType = _msgtype;
            from = GetUsername(_from);
            to = _to;
            data = _data;
        }

        private string GetUsername(int id)
        {
            string _from = "";
            if (id != 0)
            {
                MySqlCommand cmd = new MySqlCommand("SELECT username FROM accounts WHERE idAccount = @id;", DB.EllAppDB);
                MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
                idParameter.Value = id;
                cmd.Parameters.Add(idParameter);
                MySqlDataReader row = cmd.ExecuteReader();
                while (row.Read())
                {
                    from = row["username"].ToString();
                }
                row.Close();
            }
            else
                _from = "Server Message";
            return _from;
        }
    }
}
