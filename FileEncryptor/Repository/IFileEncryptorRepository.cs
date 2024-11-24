using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Repository
{
    public interface IFileEncryptorRepository
    {
        byte[] EncryptString(string text, byte[] key, byte[] iv);
        void EncryptFile(string inputFilePath, string outputFilePath, byte[] key, byte[] iv, Action<int> progressCallback);
        byte[] GenerateRandomKey();
        byte[] GenerateRandomIv();

    }
}
