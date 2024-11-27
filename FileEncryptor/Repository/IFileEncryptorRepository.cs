using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Repository
{
    public interface IFileEncryptorRepository
    {
        void EncryptString(string text, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback);
        void DecryptString(string text, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback);
        void EncryptFile(string inputFilePath, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback);
        void DecryptFile(string inputFilePath, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback);
        byte[] GenerateRandomKey();
        byte[] GenerateRandomIv();

    }
}
