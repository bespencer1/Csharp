using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TimeAllocation.data
{
    /// <summary>
    /// Summary description for weekending
    /// </summary>
    public class weekending : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            AllocationEntities ae = new AllocationEntities();
            string json = new JavaScriptSerializer().Serialize(ae.WeekEnding.AsQueryable().Where(o => o.Week_Ending > DateTime.Today).OrderBy(o => o.Week_Ending_Text));

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