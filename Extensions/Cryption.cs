using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Pilgrimage_Of_Embers
{
    public class Cryption
    {
        private string passPhrase = "CSharpAsuna1518";        // can be any string
        private string saltValue = "IDoNTEveNKnOwWhATThIsIS";        // can be any string
        private string hashAlgorithm = "29e20cba55a4b5f0b3e8f4cfe03bf975";             // can be "MD5"
        private int passwordIterations = 1020;                  // can be any number
        private string initVector = "74MA54743B33TL35"; // must be 16 bytes
        private int keySize = 256;                // can be 192 or 128

        public Cryption(string Password, string SaltValue, string HashAlgorithm, int PasswordIterations, string InitVector, int KeySize = 256)
        {
            passPhrase = Password;
            saltValue = SaltValue;
            hashAlgorithm = HashAlgorithm;
            passwordIterations = PasswordIterations;
            initVector = InitVector;
            keySize = KeySize;
        }

        public string Encrypt(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(this.initVector);
            byte[] rgbSalt = Encoding.ASCII.GetBytes(this.saltValue);
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            byte[] rgbKey = new Rfc2898DeriveBytes(this.passPhrase, rgbSalt, this.passwordIterations).GetBytes(keySize / 8);

            RijndaelManaged managed = new RijndaelManaged();
            managed.Mode = CipherMode.CBC;

            ICryptoTransform transform = managed.CreateEncryptor(rgbKey, bytes);

            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);

            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();

            byte[] inArray = stream.ToArray();

            stream.Close();
            stream2.Close();

            return Convert.ToBase64String(inArray);
        }

        public string Decrypt(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(this.initVector);
            byte[] rgbSalt = Encoding.ASCII.GetBytes(this.saltValue);
            byte[] buffer = Convert.FromBase64String(data);

            byte[] rgbKey = new Rfc2898DeriveBytes(this.passPhrase, rgbSalt, this.passwordIterations).GetBytes(this.keySize / 8);
            RijndaelManaged managed = new RijndaelManaged();
            managed.Mode = CipherMode.CBC;
            ICryptoTransform transform = managed.CreateDecryptor(rgbKey, bytes);
            MemoryStream stream = new MemoryStream(buffer);
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            byte[] buffer5 = new byte[buffer.Length];
            int count = stream2.Read(buffer5, 0, buffer5.Length);
            stream.Close();
            stream2.Close();
            return Encoding.UTF8.GetString(buffer5, 0, count);
        }
    }
}
