using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TimeAllocation.data
{
    /// <summary>
    /// Summary description for resources
    /// </summary>
    public class resources : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            
            AllocationEntities ae = new AllocationEntities();
            string json = new JavaScriptSerializer().Serialize(ae.Resources.OrderBy(o => o.Resource_Name));            
            context.Response.Write(json);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}