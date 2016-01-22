using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataImport.Common
{
    public class Database
    {

        private int _batchSize = 100000;

        [XmlAttribute("Connection_String")]
        public string ConnectionString
        {
            get;
            set;
        }

        [XmlAttribute("Table_Name")]
        public string TableName
        {
            get;
            set;
        }

        [XmlAttribute("Batch_Size")]
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

    }
}
