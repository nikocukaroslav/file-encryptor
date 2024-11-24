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
        public void EncryptString(string text, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback)
        {
            using (Aes aes = Aes.Create())
            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
            using (FileStream filestream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (CryptoStream cs = new CryptoStream(filestream, encryptor, CryptoStreamMode.Write))
            using (StreamWriter writer = new StreamWriter(cs))
            {
                int chunkSize = 4096;
                int totalChunks = (text.Length + chunkSize - 1) / chunkSize;
                int currentChunk = 0;

                for (int i = 0; i < totalChunks; i += chunkSize)
                {
                    int length = Math.Min(chunkSize, text.Length - i);
                    writer.Write(text.Substring(i, length));

                    currentChunk++;

                    int progress = (currentChunk * 100) / totalChunks;
                    progressCallback(progress);
                }
            }
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