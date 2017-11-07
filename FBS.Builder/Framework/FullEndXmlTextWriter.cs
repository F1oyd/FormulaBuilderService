using System.IO;
using System.Text;
using System.Xml;

namespace FBS.Builder.Framework
{
    /// <summary>
    /// Overriden Xml text writer to serialize desired xml string.
    /// </summary>
    public sealed class FullEndXmlTextWriter : XmlTextWriter
    {
        public FullEndXmlTextWriter(TextWriter w) : base(w)
        {
            // Formats output xml with default indent chars.
            base.Formatting = Formatting.Indented;
        }

        public override void WriteEndElement()
        {
            // Required for recording full closing element for empty tags.
            base.WriteFullEndElement();
        }

        public override void WriteStartDocument()
        {
            // Leave empty to avoid recording XML declarations.
        }
    }
}
