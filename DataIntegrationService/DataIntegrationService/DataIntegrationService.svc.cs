using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DataIntegrationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DataIntegration : IDataIntegrationService
    {
        public Message Request(Message req)
        {
            ServiceResponse resp = new ServiceResponse();

            try
            {

                ServiceRequest servReq = req.GetBody<ServiceRequest>();
                servReq.Password = req.Headers.GetHeader<string>("Password", "");
                servReq.ServiceAction = req.Headers.GetHeader<string>("ServiceAction", "");

                servReq.Password = Decrypt(servReq.Password);
                servReq.Data = Decrypt(servReq.Data);
                
                resp.ErrorNumber = 0;
                resp.ErrorMessage = string.Empty;
                resp.IsSuccessful = true;
                //resp.Data = "Brian Spencer Test";
                resp.Data = string.Format("Action: {0}; Username: {1}; Password: {2}; Data: {3}", servReq.ServiceAction, servReq.UserName, servReq.Password, servReq.Data);

            }
            catch (Exception ex)
            {
                resp.ErrorNumber = 9000;
                resp.ErrorMessage = ex.Message;
                resp.IsSuccessful = false;
                resp.Data = "Error";
            }

            MessageVersion ver = OperationContext.Current.IncomingMessageVersion;
            return Message.CreateMessage(ver, "http://dis.lashgroup.com/Response", resp);
        }

        private string Decrypt(string value)
        {
            try
            {
                X509Store store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certCollection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindBySubjectName, "ServerWCFCA", false);
                X509Certificate2 cert = foundCollection[0];

                RSACryptoServiceProvider rsaDecryptor = (RSACryptoServiceProvider)cert.PrivateKey;
                byte[] decryptedBytes = rsaDecryptor.Decrypt(Convert.FromBase64String(value), false);

                return Encoding.UTF8.GetString(decryptedBytes);

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
