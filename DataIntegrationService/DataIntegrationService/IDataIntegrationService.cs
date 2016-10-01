using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Text;

namespace DataIntegrationService
{
    [ServiceContract(Namespace = "http://dis.lashgroup.com")]
    public interface IDataIntegrationService
    {
        //[OperationContract]
        //ServiceResponse Request(ServiceRequest req);

        [OperationContract(Action = "http://dis.lashgroup.com/Request", 
            ReplyAction = "http://dis.lashgroup.com/Response")]
        Message Request(Message req);

    }

    [MessageContract]
    public class ServiceRequest
    {
        [MessageHeader(Namespace="")]
        public string ServiceAction { get; set; }

        [MessageHeader(Namespace = "")]
        public string UserName { get; set; }

        [MessageHeader(Namespace = "")]
        public string Password { get; set; }

        [MessageBodyMember]
        public string Data { get; set; }
    }

    [MessageContract]
    public class ServiceResponse
    {

        [MessageBodyMember]
        public bool IsSuccessful { get; set; }

        [MessageBodyMember]
        public int ErrorNumber { get; set; }

        [MessageBodyMember]
        public string ErrorMessage { get; set; }

        [MessageBodyMember]
        public string Data { get; set; }
    }
}
