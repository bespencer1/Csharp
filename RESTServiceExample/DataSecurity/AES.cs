using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace DataSecurity
{
    public class AES
    {
        private string _initVector = null;
        private string _saltValue = null;
        private string _passPhrase = null;
        private string _hashAlgorithm = "SHA1";
        private int _passwordIterations = 1;
        private int _keySize = 256;

        public AES()
        {
        }

        public AES(string passPhrase, string salt, string IV)
        {
            _passPhrase = passPhrase;
            _saltValue = salt;
            _initVector = IV;
        }

        public string Passphrase { 
            get{return _passPhrase;}
            set { _passPhrase = value; }
        }

        public string SaltValue
        {
            get { return _saltValue; }
            set { _saltValue = value; }
        }

        public string IV
        {
            get { return _initVector; }
            set { 
                _initVector = value;
                if (_initVector.Length != 16)
                    throw new Exception("IV must be 16 characters");
            }
        }

        public string HashAlgorithm
        {
            get { return _hashAlgorithm; }
            set
            {
                _hashAlgorithm = value;
                if (_hashAlgorithm.Length == 0)
                    _hashAlgorithm = "SHA1";
            }
        }

        public int PasswordIterations
        {
            get { return _passwordIterations; }
            set
            {
                try
                {
                    _passwordIterations = value;
                    if (_passwordIterations == 0)
                        _passwordIterations = 1;
                }
                catch
                {
                    _passwordIterations = 1;
                }
            }
        }

        public int KeySize
        {
            get { return _keySize; }
            set
            {
                try
                {
                    _keySize =value;
                    if (_keySize != 256 && _keySize != 128)
                        _keySize = 256;
                }
                catch
                {
                    _keySize = 256;
                }
            }
        }

        public string GenerateIV()
        {

            using (RijndaelManaged aesAlg = new RijndaelManaged())
            {
                aesAlg.GenerateIV();
                _initVector = Encoding.ASCII.GetString(aesAlg.IV);
            }

            return _initVector;
        }

        public string GenerateKey()
        {
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);

            PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
            byte[] keyBytes = password.GetBytes(_keySize / 8);
            return Convert.ToBase64String(keyBytes);
        }

        public string Encrypt(string text, string key)
        {
            string encryptedText = string.Empty;

            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);                

                using (RijndaelManaged aesAlg = new RijndaelManaged())
                {
                    aesAlg.Mode = CipherMode.CBC;

                    //PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
                    //byte[] keyBytes = password.GetBytes(_keySize / 8);
                    //key = Convert.ToBase64String(keyBytes);

                    byte[] keyBytes = Convert.FromBase64String(key);

                    aesAlg.Key = keyBytes;
                    aesAlg.IV = initVectorBytes;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(keyBytes, initVectorBytes);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(text);
                                swEncrypt.Flush();
                            }
                            byte[] encryptedBytes = msEncrypt.ToArray();
                            encryptedText = Convert.ToBase64String(encryptedBytes);
                        }
                    }
                    aesAlg.Clear();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return encryptedText;
        }
        

        public string Decrypt(string encryptedText, string key)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Convert.FromBase64String(key);

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);

            //byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);
            //PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
            //byte[] keyBytes = password.GetBytes(_keySize / 8);

            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;
            RijndaelManaged aesAlg = null;
            string decryptedText = null;

            try
            {
                aesAlg = new RijndaelManaged();
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(keyBytes, initVectorBytes);

                msDecrypt = new MemoryStream(cipherTextBytes);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                decryptedText = srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (srDecrypt != null) srDecrypt.Close();
                if (csDecrypt != null) csDecrypt.Close();
                if (msDecrypt != null) msDecrypt.Close();
                if (aesAlg != null) aesAlg.Clear();
            }

            return decryptedText;

        }
    }
}