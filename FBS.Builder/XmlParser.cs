using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FBS.Builder
{
    public class XmlParser : IParser
    {
        public T1 Parse<T1>(string input)
        {
            var serializer = new XmlSerializer(typeof(T1));
            using (var stringReader = new StringReader(input))
            {
                return (T1)serializer.Deserialize(stringReader);
            }
        }

        public string Stringify<T2>(T2 obj)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var serializer = new XmlSerializer(typeof(T2));
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, obj, emptyNamepsaces);
                return stream.ToString();
            }
        }
    }
}
