using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using System.Xml;

namespace SoapServiceTest
{
    public class Program
    {
        static void Main(string[] args)
        {

            RSACryptoServiceProvider prov = new RSACryptoServiceProvider();
            string privkey = prov.ToXmlString(true);
            string pubkey = prov.ToXmlString(false);

            XMLRequest();

            //JSONRequest();

            //GooglePlaces("Vista Health and Wellness Huntersville NC");
            //GooglePlaces("lakeshore pediatrics denver nc");
            //GooglePlaces("Dr Wissam E. Nadra Denver NC");
           
            //USPS usps = new USPS();

            //USPS.Address addr1 = new USPS.Address();
            //addr1.Address1 = "";  //Used to provide and apartment or suite number if applicable
            //addr1.Address2 = "4694 Lake Shore Road North"; //Street address
            //addr1.City = "Denver";
            //addr1.State = "NC";
            //addr1.Zip5 = "28037";

            //USPS.Address addr2 = new USPS.Address();
            //addr2.Address1 = "4694 Lake Shore Rd N";
            //addr2.City = "Denver";
            //addr2.State = "NC";
            //addr2.Zip5 = "28037";

            //try
            //{
            //    //Compare the addresses to see if they have the same delivery point and carrier route
            //    bool isSame = usps.Compare(addr1, addr2);
            //}
            //catch (Exception ex)
            //{
            //    //TODO: Error Handling
            //}

            //USPS.Address pplusAddess = usps.Normalize("<Address><Address1>Suite A</Address1><Address2>4694 Lake Shore Road North</Address2><City>Denver</City><State>NC</State><Zip5>28037</Zip5><Zip4/></Address>");
            //USPS.Address importAddress = usps.Normalize("<Address><Address1></Address1><Address2>4694 Lake Shore Road N</Address2><City>Denver</City><State>NC</State><Zip5>28036</Zip5><Zip4/></Address>");

            //bool same = pplusAddess.Equals(importAddress);          
            

        }

        


        private static void GooglePlaces(string searchValue)
        {
           
            //HttpServerUtility util = new HttpServerUtility();
            //searchValue = util.HtmlEncode(searchValue);

            string url = "https://maps.googleapis.com/maps/api/place/textsearch/xml?query=" + searchValue + "&key=AIzaSyCB33abdcmM1zQ0e793o8I53gEHD2ss6BU";
                        
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.ContentType = "application/json;charset=\"utf-8\"";
            req.ContentType = "application/xml";
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string placeResponseJSON = reader.ReadToEnd();
            reader.Close();

        }

        private static void JSONRequest()
        {

            string jsonMsg = "{\"Action\":\"Referral\",\"Data\":\"UBpOGJ1um4izT5\\/M8P1zUNZ1qKRDrohOtunDEtA4uQamXamAsjxdH4+ApJei7Elo70SCxLfjQxFqNROViUPVYd9cKyEcmE6ZQFTWTpZYgmoNnSRfzM4uYlVaVZIxUcSfd8CQAPHQImK\\/WA989k8a06pZfB9J3jNWLMgScFtyaDEoVfRsW45ZdnPc33ZFy0pi\\/FAzi0yu4HykWnNVBD3JL6wJ\\/ru4IEAQ8dbMBzZ2eOiA4SR4sfdUhhRwLHn\\/QwbNIH80AuSMAf6m1\\/yZ+3PvFzuoobxdC8ySzeLtQBFqxJi4\\/mzH7b0uAQpLi2R2hMEVaqGu+14sdXLYqze7DHBmRg==\"}";

            Referral reqstDet = new Referral();
            reqstDet.TransactionID = "123";
            reqstDet.TransactionDate = DateTime.Now.ToString();
            reqstDet.Version = "1.0.0";
            reqstDet.LashSystemID = "1234";
            reqstDet.Patients = new List<Patient>();

            Patient pat = new Patient();
            pat.FirstName = "Brian";
            pat.LastName = "Spencer";

            reqstDet.Patients.Add(pat);

            DataContractJsonSerializer serdet =
                    new DataContractJsonSerializer(typeof(Referral));
            MemoryStream memdet = new MemoryStream();
            serdet.WriteObject(memdet, reqstDet);
            string dataDetails =
                Encoding.UTF8.GetString(memdet.ToArray(), 0, (int)memdet.Length);

            JSONRequest reqst = new JSONRequest();
            reqst.ServiceAction = "Referral";
            reqst.Data = EncryptValue(dataDetails);

            DataContractJsonSerializer ser =
                    new DataContractJsonSerializer(typeof(JSONRequest));
            MemoryStream mem = new MemoryStream();
            ser.WriteObject(mem, reqst);
            string data =
                Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);

            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString("http://localhost:48882/RESTService.svc/json", "POST", data); 

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonMsg);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:48882/RESTService.svc/json");

            req.ContentType = "application/json;charset=\"utf-8\"";
            req.ContentLength = byteArray.Length;
            req.Method = "PUT";


            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string soapResp = reader.ReadToEnd();
            reader.Close();


            Console.WriteLine(soapResp);
        }

        private static void XMLRequest()
        {
            string encryptedString = "Brian's data";

            Request reqMsg = new Request();
            reqMsg.Action = "Referral";
            reqMsg.Data = encryptedString;

            StringWriter sw = new StringWriter();
            //Sealize the object to XML
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("ns", "http://dataservice.lashgroup.com/Request");
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            ser.Serialize(sw, reqMsg,ns);
            string xmlMsg = sw.ToString();
            sw.Close();
            sw.Dispose();

            xmlMsg = xmlMsg.Replace("\r\n", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Replace(">  <", "><").Replace(":ns","");


            byte[] byteArray = Encoding.UTF8.GetBytes(xmlMsg);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost.fiddler:48882/RESTService.svc/username/xml");

            req.ContentType = "application/xml;charset=UTF-8";
            req.ContentLength = byteArray.Length;
            req.Method = "PUT";


            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string soapResp = reader.ReadToEnd();
            reader.Close();


            Console.WriteLine(soapResp);
        }

        [DataContract(Namespace = "http://dataservice.lashgroup.com/Request")]
        public class Request
        {
            [DataMember]
            public string Action { get; set; }

            [DataMember]
            public string Data { get; set; }
        }

        private static void SoapRequest()
        {
            //string test = "this is my test value";
            //test = EncryptValue(test);
            //test = Decrypt(test);

            //DataIntegrationService.DataIntegrationServiceClient client = new DataIntegrationService.DataIntegrationServiceClient();
            //ServiceRequest sr = new ServiceRequest();
            //sr.Data = "My test data";
            //MessageVersion mv = client.Endpoint.Binding.MessageVersion;
            //Message request = Message.CreateMessage(MessageVersion.Soap11, "http://dis.lashgroup.com/Request",sr);            
            //request.Headers.Add(MessageHeader.CreateHeader("ServiceAction","","my action"));
            //request.Headers.Add(MessageHeader.CreateHeader("UserName","","user name"));
            //request.Headers.Add(MessageHeader.CreateHeader("Password", "", EncryptValue("pass word")));
            //Message response = client.Request(request);


            string soapMsg = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                              "<s:Header>" +
                                "<ServiceAction>ActionValue</ServiceAction>" +
                                "<Password>" + EncryptValue("PasswordValue") + "</Password>" +
                                "<UserName>UsernameValue</UserName>" +
                              "</s:Header>" +
                              "<s:Body>" +
                                "<ServiceRequest xmlns=\"http://schemas.datacontract.org/2004/07/DataIntegrationService\">" +
                                  "<Data>" + EncryptValue("BrianSpencer") + "</Data>" +
                                "</ServiceRequest>" +
                              "</s:Body>" +
                            "</s:Envelope>";

            //xmlns=\"http://dis.lashgroup.com\"
            //"<h:Password xmlns:h=\"http://dis.lashgroup.com\">PasswordValue</h:Password>" +
            //"<h:UserName xmlns:h=\"http://dis.lashgroup.com\">UsernameValue</h:UserName>"

            byte[] byteArray = Encoding.UTF8.GetBytes(soapMsg);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:46303/DataIntegrationService.svc");

            req.Headers.Add("SOAPAction", "http://dis.lashgroup.com/Request");
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.ContentLength = byteArray.Length;
            req.Method = "POST";


            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string soapResp = reader.ReadToEnd();
            reader.Close();


            Console.WriteLine(soapResp);
        }

        private static string EncryptValue(string value)
        {
            try
            {
                X509Store store = new X509Store(StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certCollection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection foundCollection = (X509Certificate2Collection)certCollection.Find(X509FindType.FindBySubjectName, "ServerWCFCA", false);
                X509Certificate2 cert = foundCollection[0];
               
                RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)cert.PublicKey.Key;
                byte[] cipherData = rsaEncryptor.Encrypt(Encoding.UTF8.GetBytes(value),false);

                return Convert.ToBase64String(cipherData);

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string Decrypt(string value)
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

    [MessageContract]
    public class ServiceRequest
    {
        [MessageBodyMember]
        public string Data { get; set; }
    }

    [DataContract]
    public class JSONRequest
    {
        [DataMember]
        public string ServiceAction { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public object Data { get; set; }
    }

    public class Referral
    {
        public string Version { get; set; }
        public string TransactionID { get; set; }
        public string TransactionDate { get; set; }
        public string LashSystemID { get; set; }
        public List<Patient> Patients { get; set; }
    }

    public class Patient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
