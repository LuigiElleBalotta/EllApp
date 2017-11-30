using System;
using System.Text;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace Server.Classes
{
    public class Account
    {
        public static bool CreateAccount(string username, string password, string email)
        {
	        username = username.ToUpper();
	        password = password.ToUpper();
	        email = email.ToUpper();

            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            var hashedpsw = Utility.HexStringFromBytes(bytehash);

            /*MySqlCommand cmd = new MySqlCommand("INSERT INTO accounts(username, password, email) VALUES(@username, @password, @email);", DB.EllAppDB);
            MySqlParameter passwordParameter = new MySqlParameter("@password", MySqlDbType.VarChar, 0);
            MySqlParameter usernameParameter = new MySqlParameter("@username", MySqlDbType.VarChar, 0);
            MySqlParameter emailParameter = new MySqlParameter("@email", MySqlDbType.VarChar, 0);
            passwordParameter.Value = hashedpsw.ToUpper();
            usernameParameter.Value = username.ToUpper();
            emailParameter.Value = email.ToUpper();
            cmd.Parameters.Add(usernameParameter);
            cmd.Parameters.Add(passwordParameter);
            cmd.Parameters.Add(emailParameter);
	        if (cmd.ExecuteNonQuery() >= 1)
	        {
		        Console.WriteLine("Account created.");
		        return true;
	        }
	        else
	        {
		        Console.WriteLine("Failure creating account.");
		        return false;
	        }*/
            return  false;
        }
    }
}
