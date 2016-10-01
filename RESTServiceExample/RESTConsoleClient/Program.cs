using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using System.Xml;


namespace RESTConsoleClient
{
    public class Program
    {
        private static string _publicKey = string.Empty;
        private static string _userID = System.Guid.NewGuid().ToString();
        private static string _URL = "http://localhost:48882/RESTService.svc";

        private static DataSecurity.AES _aes;
        private static string _key;

        static void Main(string[] args)
        {
            string data = "This is the data to encrypt. I want it to be a lot of data so that I can test the response time of the system";

            Console.Write("Getting RSA key ....");
            GetRSAKey();
            Console.WriteLine(_publicKey);

            Console.Write("Build request ....");
            Request requestData = BuildRequest(data);

            Console.Write("JSON Request ....");
            JSONRequest(requestData);

            Console.Write("XML Request ....");
            XMLRequest(requestData);

            Console.ReadLine();
        }

        private static void GetRSAKey()
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}/rsakey", _URL, _userID));
            req.ContentType = "application/xml;charset=\"utf-8\"";
            req.Method = "GET";

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                _publicKey = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
        }

        private static Request BuildRequest(string data)
        {
            DataSecurity.RSA rsa = new DataSecurity.RSA();
            rsa.PublicKey = _publicKey;
            rsa.OEAP = true;
            rsa.KeySize = 256;

            _aes = new DataSecurity.AES();
            //aes.IV = aes.GenerateIV();
            _aes.IV = "1234567812345678"; //Must be 16 bytes
            _aes.SaltValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";//"20160928"; //Any string
            _aes.Passphrase = "AESPassword20160925BrianSpencer!"; //"BrianSpencer"; //Any string
            _key = _aes.GenerateKey();

            Request reqst = new Request();
            reqst.Action = "Referral";
            reqst.Data = _aes.Encrypt(data, _key);
            reqst.IV = rsa.Encrypt(_aes.IV);
            reqst.Key = rsa.Encrypt(_key);            

            return reqst;
        }

        private static string HTTPRequestResponse(string URL, byte[] byteArray, string method, string contentType)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.ContentType = contentType;
            req.ContentLength = byteArray.Length;
            req.Method = method;

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string response = reader.ReadToEnd();
            reader.Close();

            return response;
        }

        private static void JSONRequest(Request reqst)
        {

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Request));
            MemoryStream mem = new MemoryStream();
            ser.WriteObject(mem, reqst);
            string jsonData = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
            mem.Close();
            mem.Dispose();

            Console.WriteLine(jsonData);

            string json = HTTPRequestResponse(string.Format("{0}/{1}/{2}", _URL, _userID, "json"), byteArray, "PUT", "application/json;charset=\"utf-8\"");
            Console.WriteLine(json);

            DataContractJsonSerializer respSer = new DataContractJsonSerializer(typeof(Response));
            Response resp = (Response)respSer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(json)));

            if (resp.IsSuccessful)
            {
                string responseData = _aes.Decrypt(resp.Data, _key);
                Console.WriteLine(responseData);
            }
            else
                Console.WriteLine(resp.ErrorMessage);
            
        }

        private static void XMLRequest(Request reqst)
        {
            byte[] byteArray;
            
            //Sealize the object to XML
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("ns", "http://dataservice.lashgroup.com/Request");
            XmlSerializer ser = new XmlSerializer(typeof(Request));

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = false, NewLineHandling = NewLineHandling.None }))
                {
                    ser.Serialize(xw, reqst, ns);  
                    xw.Close();
                    xw.Dispose();
                }
                byteArray = Encoding.UTF8.GetBytes(sw.ToString().Replace(":ns", ""));
                sw.Close();
                sw.Dispose();
            }
            
            string xml = HTTPRequestResponse(string.Format("{0}/{1}/{2}", _URL, _userID, "xml"), byteArray, "PUT", "application/xml;charset=\"utf-8\"");
            Console.WriteLine(xml);

            XmlSerializer serResp = new XmlSerializer(typeof(Response));
            Response resp = (Response)serResp.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml)));
            if (resp.IsSuccessful)
            {
                string responseData = _aes.Decrypt(resp.Data, _key);
                Console.WriteLine(responseData);
            }
            else
                Console.WriteLine(resp.ErrorMessage);
        }

        //Keep this list in alphabetical order or us the Order property
        [DataContract(Namespace = "http://dataservice.lashgroup.com/Request")]
        public class Request
        {
            [DataMember]
            public string Action { get; set; }

            [DataMember]
            public string Data { get; set; }

            [DataMember]
            public string IV { get; set; }

            [DataMember]
            public string Key { get; set; }            
            
        }

        [DataContract(Namespace = "")]
        public class Response
        {
            [DataMember]
            public string ErrorMessage { get; set; }

            [DataMember]
            public int ErrorNumber { get; set; }

            [DataMember]
            public bool IsSuccessful { get; set; }

            [DataMember]
            public string Data { get; set; }
        }
       
    }
}
