using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Repository
{
    public class AESEncryptionRepository : IFileEncryptorRepository
    {
        public byte[] EncryptString(string text, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {

                using (StreamWriter writer = new StreamWriter(cs))
                {
                    writer.Write(text);
                }
                return ms.ToArray();
            };
        }

        public void EncryptFile(string inputFilePath, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback)
        {
            using (Aes aes = Aes.Create())
            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
            using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (CryptoStream cs = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
            {
                byte[] buffer = new byte[4096];
                long totalBytes = inputFileStream.Length;
                long progress = 0;
                int bytesRead;

                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, bytesRead);
                    progress += bytesRead;

                    int progressPercentage = (int)((progress * 100) / totalBytes);
                    progressCallback(progressPercentage);
                }
                cs.FlushFinalBlock();
            }
        }
        public byte[] GenerateRandomKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public byte[] GenerateRandomIv()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

    }
}