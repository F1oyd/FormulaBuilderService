using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FBS.Builder
{
    [XmlRoot("response", Namespace = "")]
    public class FormulaResponse
    {
        [XmlElement("result", IsNullable = false)]
        public string Result { get; set; }

        [XmlElement("errors", IsNullable = false)]
        public Errors Errors { get; set; }
    }

    public class Errors
    {
        [XmlElement("error")]
        public string[] Error { get; set; }
    }
}
