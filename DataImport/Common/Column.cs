using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataImport.Common
{
    [XmlRoot("Column")]
    public class Column
    {
        [XmlAttribute("Name")]
        public string ColumnName;

        [XmlAttribute("Type")]
        public DataType ColumnType;

        [XmlAttribute("Position")]
        public int ColumnPosition;

        [XmlAttribute("Max_Length")]
        public int MaxLength;

        [XmlAttribute("IsRequired")]
        public bool IsRequired;

        [XmlAttribute("RegEx")]
        public string RegEx;

        public enum DataType
        {
            Text
            , Number
            , Decimal
            , Date
            , DateTime
            , Time
            , ID
        }
    }
}
