using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Classes
{
    public class Encryption//put back on internal after test(12-10-2023)
    {

        const int SaltLength = 1;

        public string EncryptNewString(string Password)
        {

            string salt = generateSalt();

            return AddSalt(BitConverter.ToString(Encrypt(Password,salt)).Replace("-", ""), salt);        
        }

        public bool CompareEncryptedString(string Password, string EncyptedPassword)
        {
            string salt = ExtractSalt(EncyptedPassword);

            string toTest= AddSalt(BitConverter.ToString(Encrypt(Password, salt)).Replace("-", ""), salt);

            if(toTest == EncyptedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private byte[] Encrypt(string Password, string salt)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(Password+salt);
            byte[] hashBytes = SHA512.HashData(inputBytes);

            for (int i = 0; i < 27; i++)
            {
                hashBytes = SHA512.HashData(hashBytes);
                hashBytes = SHA256.HashData(hashBytes);
            }

            return hashBytes;
        }
        private string generateSalt()
        {
            byte[] salt = new byte[SaltLength];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Convert the random bytes to a hex string for display.
            string hexString = BitConverter.ToString(salt).Replace("-", "");

            return hexString;
        }
        private string AddSalt(string OrignalString, string Salt)
        {
            int index=0;
            foreach (char s in Salt) {

                OrignalString = OrignalString.Insert(index,s.ToString());

                index += 3;
            }
            return OrignalString;
        }
        private string ExtractSalt(string OrignalString)
        {
            string Salt = string.Empty;

            for (int i = 0; i < SaltLength*2; i++)
            {
                Salt += OrignalString.Substring(i * 3, 1);
            }

            return Salt;
        }

       

    }
}
