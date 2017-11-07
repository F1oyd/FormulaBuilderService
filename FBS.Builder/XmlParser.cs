using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FBS.Builder.Framework;

namespace FBS.Builder
{
    /// <summary>
    /// Xml string parser to a generic model and vice versa.
    /// </summary>
    public class XmlParser : IParser
    {
        /// <summary>
        /// Parse input Xml string to a generic model.
        /// </summary>
        public T1 Parse<T1>(string input)
        {
            var serializer = new XmlSerializer(typeof(T1));
            using (var stringReader = new StringReader(input))
            {
                return (T1)serializer.Deserialize(stringReader);
            }
        }

        /// <summary>
        /// Stringify a generic model to an Xml string.
        /// </summary>
        public string Stringify<T2>(T2 obj)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(T2));
            using (var stream = new StringWriter())
            using (var writer = new FullEndXmlTextWriter(stream))
            {
                serializer.Serialize(writer, obj, emptyNamepsaces);
                return stream.ToString();
            }
        }
    }
}
