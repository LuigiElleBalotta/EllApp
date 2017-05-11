using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace EllApp_server.Classes
{
    class CommandManager
    {
        public static void CreateAccount(string username, string password, string email)
        {
            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            var hashedpsw = Utility.HexStringFromBytes(bytehash);

            MySqlCommand cmd = new MySqlCommand("INSERT INTO accounts(username, password) VALUES(@username, @password, @email);", DB.EllAppDB);
            MySqlParameter passwordParameter = new MySqlParameter("@password", MySqlDbType.VarChar, 0);
            MySqlParameter usernameParameter = new MySqlParameter("@username", MySqlDbType.VarChar, 0);
            MySqlParameter emailParameter = new MySqlParameter("@email", MySqlDbType.VarChar, 0);
            passwordParameter.Value = hashedpsw.ToUpper();
            usernameParameter.Value = username.ToUpper();
            emailParameter.Value = email.ToUpper();
            cmd.Parameters.Add(usernameParameter);
            cmd.Parameters.Add(passwordParameter);
            cmd.Parameters.Add(emailParameter);
            if(cmd.ExecuteNonQuery() >= 1)
                Console.WriteLine("Account created.");
            else
                Console.WriteLine("Failure creating account.");
        }
    }
}
