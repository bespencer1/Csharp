using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace AES_Encryption
{
    public class AES
    {
        private string _passPhrase = "PassPhrase";
        private string _initVector = "InitVector";
        private int _keySize = 256;

        public string PassPhrase {
            get { return _passPhrase; }
            set { _passPhrase = value; }
        }

        public string InitVector {
            get { return _initVector; }
            set { _initVector = value; }
        }


        public string EncryptStringToBytes_AES(string plainText)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] initVectorBytes = encoding.GetBytes(_initVector);
            byte[] plainTextBytes = encoding.GetBytes(plainText);

            byte[] keyBytes = encoding.GetBytes(_passPhrase);

            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            RijndaelManaged aesAlg = null;
            byte[] cipherTextBytes = null;

            try
            {
                aesAlg = new RijndaelManaged();
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.KeySize = _keySize;
                aesAlg.BlockSize = _keySize;
                aesAlg.IV = initVectorBytes;
                aesAlg.Key = keyBytes;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(keyBytes, initVectorBytes);

                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                csEncrypt.FlushFinalBlock();

                cipherTextBytes = msEncrypt.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (csEncrypt != null) csEncrypt.Close();
                //if (msEncrypt != null) msEncrypt.Close();
                if (aesAlg != null) aesAlg.Clear();
            }

            return Convert.ToBase64String(cipherTextBytes);


        }

        public string DecryptStringFromBytes_AES(string cipherText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] initVectorBytes = encoding.GetBytes(_initVector);

            byte[] keyBytes = encoding.GetBytes(_passPhrase);

            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;
            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                aesAlg = new RijndaelManaged();
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.KeySize = _keySize;
                aesAlg.BlockSize = _keySize;
                aesAlg.IV = initVectorBytes;
                aesAlg.Key = keyBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(keyBytes, initVectorBytes);

                msDecrypt = new MemoryStream(cipherTextBytes);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                plaintext = srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (srDecrypt != null) srDecrypt.Close();
                //if (csDecrypt != null) csDecrypt.Close();
                //if (msDecrypt != null) msDecrypt.Close();
                if (aesAlg != null) aesAlg.Clear();
            }

            return plaintext;
        }

    }
}
