using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FBS.Builder
{
    [XmlRoot("request")]
    public class FormulaRequest
    {
        [XmlElement("expression")]
        public Expression Expression { get; set; }
    }

    public class Expression
    {
        [XmlElement("operation")]
        public string Operation { get; set; }

        [XmlElement("operand")]
        public Operand[] Operands { get; set; }
    }

    public class Operand
    {
        [XmlElement("const")]
        public decimal? Value { get; set; }

        [XmlElement("expression")]
        public Expression Expression { get; set; }
    }
}
