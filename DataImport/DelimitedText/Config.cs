using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImport.Common;
using System.Xml.Serialization;

namespace DataImport.DelimitedText
{
    [XmlRoot("Config")]
    public class Config : DataImport.Common.SettingsFile
    {
        /* DataImport.Common.File provides the following properties
         * File Pattern
         * Import Folder
         * Working Folder
         * Archive Folder
         * Column Count
         * File Type
         */


        //Extra properties for Delimited text
        [XmlAttribute("Row_Delimiter")]
        public string RowDelimiter
        {
            get;
            set;
        }

        [XmlAttribute("Column_Delimiter")]
        public string ColumnDelimiter
        {
            get;
            set;
        }

        public  List<Column> Columns;
    }
}
