using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TimeAllocation.data
{
    /// <summary>
    /// Summary description for assignment
    /// </summary>
    public class assignment : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string resource = context.Request.QueryString.Get(0);
            context.Response.ContentType = "application/json";

            AllocationEntities ae = new AllocationEntities();
            var query = ae.Assignments.AsQueryable().Where(o => o.Developer == resource && o.Dev_Status != "Complete");
            string json = new JavaScriptSerializer().Serialize(query);
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