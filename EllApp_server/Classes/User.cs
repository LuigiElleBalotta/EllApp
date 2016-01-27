using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace EllApp_server.Classes
{
    class User
    {
        int ID = 0;
        string username, last_ip, email;
        static Config_Manager conf = new Config_Manager();
        MySqlConnection conn = new MySqlConnection("Server=" + conf.getValue("mysql_host") + ";Database=" + conf.getValue("mysql_db") + ";Uid=" + conf.getValue("mysql_user") + ";Pwd=" + conf.getValue("mysql_password") + ";");

        public User(string _username, string _password)
        {
            username = _username;
            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + _password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            _password = HexStringFromBytes(bytehash);
            Console.WriteLine(_password.ToUpper());


            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT idAccount, email, last_ip FROM accounts WHERE username = @username AND password = @password;", conn);
            MySqlParameter passwordParameter = new MySqlParameter("@password", MySqlDbType.VarChar, 0);
            MySqlParameter usernameParameter = new MySqlParameter("@username", MySqlDbType.VarChar, 0);
            passwordParameter.Value = _password.ToUpper();
            usernameParameter.Value = username.ToUpper();
            cmd.Parameters.Add(usernameParameter);
            cmd.Parameters.Add(passwordParameter);
            MySqlDataReader row = cmd.ExecuteReader();
            while(row.Read())
            {
                ID = Convert.ToInt32(row["idAccount"]);
                last_ip = row["last_ip"].ToString();
                email = row["email"].ToString();
            }
            row.Close();
            conn.Close();
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public bool Validate()
        {
            if (ID > 0)
                return true;
            else
                return false;
        }

        public string GetUsername()
        {
            return username;
        }

        public int GetID()
        {
            return ID;
        }

        public void SetOnline()
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("UPDATE accounts SET isOnline = 1 WHERE idAccount = @id;", conn);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = ID;
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void SetOffline()
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("UPDATE accounts SET isOnline = 0 WHERE idAccount = @id;", conn);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = ID;
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
