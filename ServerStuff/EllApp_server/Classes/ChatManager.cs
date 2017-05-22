using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.Classes.Entities;
using MySql.Data.MySqlClient;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    public class ChatManager : ChatMessage
    {
        public void Save()
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO chatmessage(ChatRoom, MessageFrom, MessageToType, MessageTo, Text) VALUES(@chatid, @from, @totype, @to, @content);", DB.EllAppDB);
            MySqlParameter _chatid = new MySqlParameter("@chatid", MySqlDbType.VarChar, 0);
            MySqlParameter _content = new MySqlParameter("@content", MySqlDbType.LongText, 0);
            MySqlParameter _from = new MySqlParameter("@from", MySqlDbType.Int32, 0);
            MySqlParameter _totype = new MySqlParameter("@totype", MySqlDbType.VarChar, 0);
            MySqlParameter _to = new MySqlParameter("@to", MySqlDbType.Int32, 0);
            _chatid.Value = ChatRoom;
            _content.Value = Text;
            _from.Value = MessageFrom;
            _totype.Value = MessageToType;
            _to.Value = MessageTo;
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
