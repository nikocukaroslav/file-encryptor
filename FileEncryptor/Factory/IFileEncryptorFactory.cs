using FileEncryptor.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Factory
{
   public interface IFileEncryptorFactory
    {
        IFileEncryptorRepository GetEncryptorRepository(string encryptionType);
    }
}
