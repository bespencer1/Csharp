using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RESTServiceExample
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "XMLTest" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select XMLTest.svc or XMLTest.svc.cs at the Solution Explorer and start debugging.
    public class XMLTest : IXMLTest
    {
        public string XMLRequest(Request xmlData)
        {
            return "Brian got it working";
        }
    }
}
