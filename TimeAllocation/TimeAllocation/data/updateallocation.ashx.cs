using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TimeAllocation.data
{
    /// <summary>
    /// Summary description for updateallocation
    /// </summary>
    public class updateallocation : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.RequestType == "PUT")
            {
                string retJSON = string.Empty;
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    retJSON = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                }

                //process the JSON
                List<vw_Assignment_Allocation> allocations = (List<vw_Assignment_Allocation>) new JavaScriptSerializer().Deserialize(retJSON, typeof(List<vw_Assignment_Allocation>));

                Regex rex = new Regex(@"^C\d{2}_\d{2}_\d{4}$");

                AllocationEntities ae = new AllocationEntities();
                foreach (vw_Assignment_Allocation allocation in allocations)
                {
                    foreach (var prop in allocation.GetType().GetProperties())
                    {
                        //allocation.C06_17_2016
                        Match m = rex.Match(prop.Name);
                        if(m.Success)
                            ae.upds_Assignment_Allocation_Update(null, allocation.Assignment_ID, GetDate(prop.Name), (double)prop.GetValue(allocation));
                    }
                    
                }
            }

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"status\":\"ok\"}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private DateTime GetDate(string propName)
        {
            return DateTime.Parse(propName.Remove(0,1).Replace("_","/"));
        }

    }
}