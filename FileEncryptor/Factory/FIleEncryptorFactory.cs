using FileEncryptor.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Factory
{
    public class FileEncryptorFactory : IFileEncryptorFactory
    {
        private IFileEncryptorRepository _factory;

        public IFileEncryptorRepository GetFileEncryptorRepository(string encryptionType)
        {
            switch (encryptionType)
            {
                case "AES":
                    _factory = new AESEncryptionRepository();
                    break;
                case "DES":
                    _factory = new DESEncryptionRepository();
                    break;  
                default: throw new ArgumentException(encryptionType);
            }

            return _factory;
        }
    }
}
