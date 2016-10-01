using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace SoapServiceTest
{
    public class USPS
    {

        //For more information about the postal services API. See the documentation here https://www.usps.com/business/web-tools-apis/address-information-api.htm

        /// <summary>
        /// URL to access the US PS Shipping API
        /// </summary>
        public string URL = "http://production.shippingapis.com/ShippingAPI.dll";

        /// <summary>
        /// User ID registered at https://registration.shippingapis.com/
        /// </summary>
        public string UserID = "275LASHG6814";


        /// <summary>
        /// Compare addresses to see if they have the same delivery point and carrier route.
        /// </summary>
        /// <param name="addr1">Address 1</param>
        /// <param name="addr2">Address 2</param>
        /// <returns>boolean</returns>
        public bool Compare(Address addr1, Address addr2)
        {
            bool retVal = false;

            //Normalize the addresses
            ReturnAddress addr1Obj;
            ReturnAddress addr2Obj;

            retVal = Compare(addr1, addr2, out addr1Obj, out addr2Obj);

            return retVal;
        }

        /// <summary>
        /// Compare addresses to see if they have the same delivery point and carrier route.
        /// Returns address provided by the US Postal Service with the delivery point and carrier route
        /// </summary>
        /// <param name="addr1">Address 1</param>
        /// <param name="addr2">Address 1</param>
        /// <param name="returnAddr1">USPS Address 1</param>
        /// <param name="returnAddr2">USPS Address 2</param>
        /// <returns>boolean</returns>
        public bool Compare(Address addr1, Address addr2, out ReturnAddress returnAddr1, out ReturnAddress returnAddr2)
        {
            bool retVal = false;

            //Normalize the addresses
            returnAddr1 = Normalize(addr1);
            returnAddr2 = Normalize(addr2);

            //Compare the data
            if (returnAddr1 != null && returnAddr2 != null)
            {
                if (returnAddr1.DeliveryPoint == returnAddr2.DeliveryPoint && returnAddr1.CarrierRoute == returnAddr2.CarrierRoute)
                    retVal = true;
            }

            return retVal;
        }


        /// <summary>
        /// Validate and Normalize an address using the US Postal Service shipping API
        /// </summary>
        /// <param name="address">Address to normalize</param>
        /// <returns>Normalized Address</returns>
        public ReturnAddress Normalize(Address address)
        {

            ReturnAddress addr = null;

            string addressSearch = SerializeAddress(address);

            string apiurl = string.Format("{0}?API=Verify&XML=<AddressValidateRequest USERID=\"{1}\"><IncludeOptionalElements>true</IncludeOptionalElements><ReturnCarrierRoute>true</ReturnCarrierRoute>{2}</AddressValidateRequest>", URL, UserID, addressSearch);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiurl);
            //req.ContentType = "application/json;charset=\"utf-8\"";
            req.ContentType = "application/xml";
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());

            //Get the response
            string response = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            if (response.Contains("Error"))
            {
                //Specify the root element
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = "Error";
                xRoot.IsNullable = true;

                //Deserialize the XML to the Error class.
                XmlSerializer ser = new XmlSerializer(typeof(Error), xRoot);
                Error errorResponse = (Error)ser.Deserialize(new StringReader(response));              
                
                //Throw the error
                throw new Exception(errorResponse.Description);
            }
            else
            {
                //Specify the root element
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = "AddressValidateResponse";
                xRoot.IsNullable = true;

                //Deserialize the XML to the Address Validate Response class
                XmlSerializer ser = new XmlSerializer(typeof(AddressValidateResponse), xRoot);
                AddressValidateResponse addrResponse = (AddressValidateResponse)ser.Deserialize(new StringReader(response));
                addr = addrResponse.Address;
            }         

            return addr;

        }

        private string SerializeAddress(Address addr)
        {
            string retVal = null;

            //Used to remove namespaces from elements
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringWriter sw = new StringWriter();

            //Sealize the object to XML
            XmlSerializer ser = new XmlSerializer(typeof(Address));
            ser.Serialize(sw, addr,ns);
            retVal = sw.ToString();
            sw.Close();
            sw.Dispose();

            //Remove line feeds and other items we don't need
            retVal = retVal.Replace("\r\n", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>","").Replace(">  <","><");

            return retVal;
            
        }

        public class AddressValidateResponse
        {
            [XmlElement("Address")]
            public ReturnAddress Address;
        }

        public class ReturnAddress : Address
        {
            [XmlElement]
            public string DeliveryPoint;
            [XmlElement]
            public string CarrierRoute;
            [XmlElement]
            public string ReturnText;
        }

        public class Address
        {
            /// <summary>
            /// Optional. Address Line 1 is used to provide an apartment or suite number, if applicable.  Maximum characters allowed: 38
            /// </summary>
            [XmlElement]
            public string Address1 = "";

            /// <summary>
            /// Street address.  Maximum characters allowed: 38
            /// </summary>
            [XmlElement]
            public string Address2 = "";

            /// <summary>
            /// Maximum characters allowed: 15
            /// </summary>
            [XmlElement]
            public string City = "";

            /// <summary>
            /// Maximum characters allowed: 15
            /// </summary>
            [XmlElement]
            public string State = "";

            /// <summary>
            /// Maximum characters allowed: 5
            /// </summary>
            [XmlElement]
            public string Zip5 = "";

            /// <summary>
            /// Optional.
            /// </summary>
            [XmlElement]
            public string Zip4 = "";
            
        }

        public class Error
        {
            [XmlElement]
            public string Number;
            [XmlElement]
            public string Source;
            [XmlElement]
            public string Description;
            [XmlElement]
            public string HelpFile;
        }

    }
}
