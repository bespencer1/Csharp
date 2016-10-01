using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;

namespace RESTServiceExample
{

    public class RESTService : IRESTService
    {

        public Response JSON(string userToken, Request jsonRequest)
        {
            Response resp = new Response();

            try
            {
                resp = ProcessMessage(jsonRequest,true);
            }
            catch (Exception ex)
            {
                resp.ErrorMessage += ex.Message;
            }

            return resp;
        }


        public Response XML(string userToken, Request xmlData)
        {
            Response resp = new Response();

            try
            {
                resp = ProcessMessage(xmlData,true);
            }
            catch (Exception ex)
            {
                resp.ErrorMessage += ex.Message;
            }

            return resp;
        }

        public RSAKeyValue GetRSAKey(string userToken)
        {
            RSAKeyValue key = new RSAKeyValue();
            return key;
        }

        public Response EchoXml(string userToken, Request echoData)
        {
            return Echo(userToken, echoData);
        }

        public Response EchoJson(string userToken, Request echoData)
        {
            return Echo(userToken, echoData);
        }

        private Response Echo(string userToken, Request echoData)
        {
            Response resp = new Response();

            try
            {
                resp = ProcessMessage(echoData, false);
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
            }

            return resp;
        }

        private Response ProcessMessage(Request request, bool encrypt)
        {

            Response resp = new Response();
            resp.ErrorMessage = string.Empty;
            resp.IsSuccessful = true;

            if(string.IsNullOrEmpty(request.Action))
            {
                resp.ErrorMessage += " \"Action\" value must be provided.";
                resp.IsSuccessful = false;
            }
            if (string.IsNullOrEmpty(request.Data))
            {
                resp.ErrorMessage += " \"Data\" value must be provided.";
                resp.IsSuccessful = false;
            }
            if (string.IsNullOrEmpty(request.IV))
            {
                resp.ErrorMessage += " \"IV\" value must be provided.";
                resp.IsSuccessful = false;
            }
            if (string.IsNullOrEmpty(request.Key))
            {
                resp.ErrorMessage += " \"Key\" value must be provided.";
                resp.IsSuccessful = false;
            }
            if(!resp.IsSuccessful)
                return resp;
                

            //Get Key from RSA encrypted Base64 string
            DataSecurity.RSA rsa = new DataSecurity.RSA();
            //rsa.PrivateKey = "<RSAKeyValue><Modulus>3nAtE8P3qmYFQSpAMth8dTewX8g5b17RiLifYqEMKeh51SthXeIA2uGhGnwFldk7ukvoUzyNfPy8TyiJwRfjgQ==</Modulus><Exponent>AQAB</Exponent><P>98N2t3Tkcc72ISTZ0hRSPMp1qvSSZv/8P4PznPUUuD0=</P><Q>5dUvyXpvmqp5Da4oBhoardGJz72cNQPzUnUV9+UNyJU=</Q><DP>P/ZnNI9p1nO+mOnhjdmPLYB7BipjjVGytlcDUlb3BWU=</DP><DQ>2SNB46tOF6Tpe3hHjgRJomCuP9uW4iuaThQarjr0e8U=</DQ><InverseQ>SWD8EEJU6K5SNrn2FTz9Ve4CDpFVb8RcAzrq+oH9ioc=</InverseQ><D>d4vmThcmRigqbslcU3dR/FV/rxqW86oMBuTilElllidglI39aK1WSAEQNiYNvOXT+uqsiMBLpPlUZZChoncDIQ==</D></RSAKeyValue>";                        
            rsa.PrivateKey = "<RSAKeyValue><Modulus>vYdhPuIqr2WJPRI9LORDITvNz8iDeHQM7E1GBfy9htkzzfg1AEP3cmqzWMHs50fqnEA9gln6Pu19RO/gwtghG8iqxCyL2XLszwTqB6kMRRmBgmheuLc2v10Gv5TGZKUdojzsNTZsRV6v3BQu8nPv9oqgXchFOuA75/3VMyVhKzk=</Modulus><Exponent>AQAB</Exponent><P>8KRAelPeeOobS3wUufL+YKDw8gHAvg2l7LYbSjniaO/VFzPGAhOJbGDNgzhHP6VYgOztuBeh/B1Q55+PjWiwtw==</P><Q>yaAEN0oBbzci7xw73c9sz1QLd9yYkcH52WAML1WcPKrI3F8tsIHj8c6AkyoD4sd6e3YN2nQNi6UXcBTYYt4zjw==</Q><DP>V5aRWXUZbs1XLfx11tDZQfaSfKOisgppsGUAPd4fyK4y04Umty7BQE9jvjVHyaS3OAYE6JLBhPBuuD/dkkzetQ==</DP><DQ>lBKCRiCBgS3a6AOyK6nj3Td53KMTISh11yBkknaq8RrzvHPh2BjsUG85mdpnVCrvVrBSaDaGpCGwXs3QRHuvHQ==</DQ><InverseQ>ajYqS2J1QYj4om9/XSn0ulNsE9xxEWw+tXSbDjYQVWVjPk048m6CxGUjjIbzuBezWUCt7s+ZPGfIL+60uUS//Q==</InverseQ><D>j64ZD8X/XBaactzn2ad2KLSgsxusZ117Co0OP625tIfm2QeIjpn39hpjui7RvQftGOUufCpgJuJ9rAw6FRwqBAnUtGHqFNqetjZaunD1zqhaKp6kjS7jo7QW0wkWlkgc99vvOVaMQhYnXv1/73xA8Wx8pn5aGgRJn3a+t66bLB0=</D></RSAKeyValue>";
            rsa.OEAP = true;
            rsa.KeySize = 256;
            string key = rsa.Decrypt(request.Key);

            //Decrypt the data
            DataSecurity.AES aes = new DataSecurity.AES();
            aes.IV = rsa.Decrypt(request.IV);
            string data = aes.Decrypt(request.Data, key);

            //Encrypt the data if requested
            if (encrypt)
            {                
                data = aes.Encrypt(data, key);
            }

            resp.Data = data;
            return resp;

        }

        //private AESKey GetKey(string key)
        //{
        //    AESKey aesKey = new AESKey();

        //    //Deserialize the XML to the Error class.
        //    XmlSerializer ser = new XmlSerializer(typeof(AESKey));
        //    AESKey errorResponse = (AESKey)ser.Deserialize(new StringReader(key)); 

        //    return aesKey;
        //}

    }
}
