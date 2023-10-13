using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.DTO;
using Dal.DBModels;
using Core.Enums;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using Core.Models;

namespace Dal.Classes
{
    internal class DBContext
    {
        string CS = "SERVER=127.0.0.1;UID=root;PASSWORD=;DATABASE=asplogintest";

        public bool TryAddUser(NewUserDto user, out UserCreationEnum message)
        {
            Encryption encryption = new Encryption();
            message = UserCreationEnum.created;
            if (DoesUserExist(user.username))
            {
                message = UserCreationEnum.usernameTaken;
                return false;
            }
            try
            {
                using (MySqlConnection con = new MySqlConnection(CS))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO users(username, password, email) VALUES(@user, @password, @email)", con);
                    cmd.Parameters.AddWithValue("@user", user.username);
                    cmd.Parameters.AddWithValue("@password", encryption.EncryptNewString(user.password));
                    cmd.Parameters.AddWithValue("@email", user.email);

                    cmd.ExecuteNonQuery();
                    cmd.CommandType = CommandType.Text;

                    con.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                message = UserCreationEnum.failed;
                return false;
                throw;
            }
        }

        public bool DoesUserExist(string UserName)
        {
            using (MySqlConnection con = new MySqlConnection(CS))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username = @username LIMIT 1", con);
                cmd.Parameters.AddWithValue("@username", UserName);
                cmd.CommandType = CommandType.Text;
                con.Open();
            
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    con.Close();
                    return true;
                }
                con.Close();
                
            }
            return false;
        }

        /*public bool TryRemoveUser()
        {

        }*/
        public bool TryLogin(string Username, string Password, out UserDto user)
        {
            user = null;
            Encryption encryption = new Encryption();

            using (MySqlConnection con = new MySqlConnection(CS))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username = @username LIMIT 1", con);
                cmd.Parameters.AddWithValue("@username", Username);
                cmd.CommandType = CommandType.Text;
                con.Open();

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    user = new UserDto();
                    user.id = Convert.ToInt32(rdr["id"]);
                    user.username = Convert.ToString(rdr["username"]);
                    user.password = Convert.ToString(rdr["password"]);
                    user.email = Convert.ToString(rdr["email"]);

                    user.rememberToken = GenerateToken();

                    TryPostToken(user.id, user.rememberToken);

                    
                }
                con.Close();
                if (user != null)
                {
                    if (encryption.CompareEncryptedString(Password, user.password))
                    {
                        return true;
                    }
                    else
                    {
                        user = null;
                    }
                }
                return false;
            }
            
        }

        public bool IsTokenValid(int UserId, string Token)
        {
            using (MySqlConnection con = new MySqlConnection(CS))
            {	
                MySqlCommand cmd = new MySqlCommand("SELECT rememberToken,tokenValidUntil FROM users WHERE id = @UserId LIMIT 1", con);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandType = CommandType.Text;
                con.Open();

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    string DBtoken = Convert.ToString(rdr["rememberToken"]);
                    DateTime dateTime = Convert.ToDateTime(rdr["tokenValidUntil"]);
                    con.Close();
                    if (Token!=DBtoken || DateTime.Now > dateTime)
                    {
                        return false;
                    }


                    return true;
                }
                con.Close();

            }
            return false;
        }

        public bool TryPostToken(int UserId, string Token)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(CS))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE users SET rememberToken = @newToken, tokenValidUntil = @until", con);
                    cmd.Parameters.AddWithValue("@newToken", Token);
                    cmd.Parameters.AddWithValue("@until", DateTime.Now.AddHours(2));

                    cmd.ExecuteNonQuery();
                    cmd.CommandType = CommandType.Text;

                    con.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public string GenerateToken(int Length = 40)
        {
            byte[] TokenBytes = new byte[Length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(TokenBytes);
            }

            // Convert the random bytes to a hex string for display.
            string hexString = BitConverter.ToString(TokenBytes);

            return hexString;
        }
    }
}
