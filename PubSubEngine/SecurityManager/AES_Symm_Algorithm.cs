using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    //Implementation of AES encryption in ECB mode
    public class AES_Symm_Algorithm
    {
        private const string DefaultKey = "oib"; // Default ključ

        /// <summary>
        /// Encrypts any object using AES in ECB mode.
        /// </summary>
        public static string EncryptData<T>(T data, string key = DefaultKey)
        {
            string jsonData = JsonConvert.SerializeObject(data); // Serijalizacija u JSON
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32)); // 256-bit key
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, null);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(jsonData); // Upis serijalizovanih podataka
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts data and converts it back to the original object type.
        /// </summary>
        public static T DecryptData<T>(string encryptedData, string key = DefaultKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32)); // 256-bit key
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, null);

                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            string jsonData = reader.ReadToEnd(); // Čitanje dekriptovanog JSON-a
                            return JsonConvert.DeserializeObject<T>(jsonData); // Deserijalizacija u originalni tip
                        }
                    }
                }
            }
        }
    }
}
