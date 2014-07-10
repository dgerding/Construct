using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class GenerateStrongNameKeyFile
    {
        /// <summary>
        /// Generating a strong name key pair file programmatically
        /// </summary>
        /// <param name=”fileName”>Fullname of the keyfile with full path, which will be generated.</param>
        /// <param name=”keySize”>RSA key size. Default is 1024. Range is 384-16384 in 8 bit increments.</param>
        public static void Generate(string fileName, int keySize)
        {
            if ((keySize % 8) != 0)
            {
                throw new CryptographicException("Invalidkey size. Valid size is 384 to 16384 mod 8. Default 1024.");
            }

            CspParameters parms = new CspParameters();
            parms.KeyNumber = 2;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize, parms);
            byte[] array = provider.ExportCspBlob(!provider.PublicOnly);

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(array, 0, array.Length);
            }
        }
    }
}