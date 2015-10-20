using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AES_Encryption;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Encrypt_Decrypt_String()
        {
            string expected = "Let's encrypt some data.";

            AES aes = new AES();

            //Set the properties
            aes.PassPhrase = "mT4o126d95oK7uKw5q7EG6F8g18512LG";
            aes.InitVector = "A5wei261N5rWR5sFYbaD122N4piYKq3C";
            
            //Encrypt the data
            string encResult = aes.EncryptStringToBytes_AES(expected);

            //Decrypt the data
            string actual = aes.DecryptStringFromBytes_AES(encResult);

            Assert.AreEqual(expected, actual, "The decrypted value is not correct");

        }
    }
}
