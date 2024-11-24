using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Data
{
    public class EncryptionMethod
    {
        public string Name { get; set; }
    }

    public static class EncryptionMethods
    {
        public static List<EncryptionMethod> GetEncryptionMethods()
        {
            return new List<EncryptionMethod>()
            {
                new EncryptionMethod() {Name = "AES" },
                new EncryptionMethod() {Name = "DES" }
            };
        }
    }
}
