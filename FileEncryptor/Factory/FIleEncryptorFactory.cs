using FileEncryptor.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Factory
{
    public class FIleEncryptorFactory : IFileEncryptorFactory
    {
        private IFileEncryptorRepository _factory;

        public IFileEncryptorRepository GetEncryptorRepository(string encryptionType)
        {
            switch (encryptionType)
            {
                case "AES":
                    _factory = new AESEncryptionRepository();
                    break;
                default: throw new ArgumentException("encryptionType");
            }

            return _factory;
        }
    }
}
