using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RESTServiceExample
{
    [ServiceContract]
    public interface IRESTService
    {
        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat=WebMessageFormat.Json,
            ResponseFormat=WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate="/{userToken}/json")]
        Response JSON(string userToken, Request jsonData);


        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/{userToken}/xml")]
        Response XML(string userToken, Request xmlData);


        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/{userToken}/rsakey")]
        RSAKeyValue GetRSAKey(string userToken);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/{userToken}/echo/json")]
        Response EchoJson(string userToken, Request jsonData);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/{userToken}/echo/xml")]
        Response EchoXml(string userToken, Request echoData);

    }

    [DataContract(Namespace="")]
    public class RSAKeyValue
    {
        //"3nAtE8P3qmYFQSpAMth8dTewX8g5b17RiLifYqEMKeh51SthXeIA2uGhGnwFldk7ukvoUzyNfPy8TyiJwRfjgQ==";
        //<RSAKeyValue><Modulus>vYdhPuIqr2WJPRI9LORDITvNz8iDeHQM7E1GBfy9htkzzfg1AEP3cmqzWMHs50fqnEA9gln6Pu19RO/gwtghG8iqxCyL2XLszwTqB6kMRRmBgmheuLc2v10Gv5TGZKUdojzsNTZsRV6v3BQu8nPv9oqgXchFOuA75/3VMyVhKzk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>
        [DataMember]
        public string Modulus = "vYdhPuIqr2WJPRI9LORDITvNz8iDeHQM7E1GBfy9htkzzfg1AEP3cmqzWMHs50fqnEA9gln6Pu19RO/gwtghG8iqxCyL2XLszwTqB6kMRRmBgmheuLc2v10Gv5TGZKUdojzsNTZsRV6v3BQu8nPv9oqgXchFOuA75/3VMyVhKzk=";

        [DataMember]
        public string Exponent = "AQAB";
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

    [DataContract(Namespace="")]
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
