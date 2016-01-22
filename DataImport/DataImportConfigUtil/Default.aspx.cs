using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace DataImportConfigUtil
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack && !Page.IsCallback)
                ReadSchema();            

        }

        private void ReadSchema()
        {
            string xsdFile = HttpContext.Current.Server.MapPath("~/XSD/DelimitedText.xsd");
            XmlReader reader = XmlReader.Create(xsdFile);

            XmlSchema schema = XmlSchema.Read(reader, null);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);
            schemaSet.Compile();

            foreach(XmlSchemaElement element in schema.Elements.Values)
            {
                Response.Write(string.Format("Element: {0}", element.Name));
                //element.ElementSchemaType.Name 
            }

        }

    }
}