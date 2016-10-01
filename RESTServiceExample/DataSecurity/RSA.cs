using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DataSecurity
{
    public class RSA
    {

        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public bool OEAP { get; set; }
        public int KeySize { get; set; }

        public string Encrypt(string value)
        {
            using (RSACryptoServiceProvider rsaEncryptor = new RSACryptoServiceProvider(KeySize))
            {
                rsaEncryptor.FromXmlString(this.PublicKey);

                byte[] dataBytes = Encoding.UTF8.GetBytes(value);
                byte[] cipherData = rsaEncryptor.Encrypt(dataBytes, OEAP);

                return Convert.ToBase64String(cipherData);
            }
        }

        public string Decrypt(string value)
        {

            //X509Store store = new X509Store(StoreLocation.LocalMachine);
            //store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //X509Certificate2Collection certCollection = (X509Certificate2Collection)store.Certificates;
            //X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindBySubjectName, "ServerWCFCA", false);
            //X509Certificate2 cert = foundCollection[0];

            //RSACryptoServiceProvider rsaDecryptor = (RSACryptoServiceProvider)cert.PrivateKey;

            //CspParameters cspParams = new CspParameters();
            //cspParams = new CspParameters(1);
            //cspParams.KeyContainerName = "Tracker";

            using (RSACryptoServiceProvider rsaDecryptor = new RSACryptoServiceProvider(KeySize))
            {
                rsaDecryptor.FromXmlString(PrivateKey);

                byte[] decryptedBytes = rsaDecryptor.Decrypt(Convert.FromBase64String(value), OEAP);

                return Encoding.UTF8.GetString(decryptedBytes);
            }

        }

    }
}
