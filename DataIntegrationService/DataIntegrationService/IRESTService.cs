using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DataIntegrationService
{
    [ServiceContract]
    public interface IRESTService
    {
        [OperationContract]
        [WebInvoke(Method="Get",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "json")]
        string JSONData();
    }
}
