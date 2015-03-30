using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Toolbox
{
    public static class XmlUtilities
    {
        private static Dictionary<Type, XmlSerializer> serializerCache = new Dictionary<Type, XmlSerializer>();

        public static XmlSerializer GetCachedSerializer(Type type)
        {
            lock (serializerCache)
            {
                if (serializerCache.ContainsKey(type))
                {
                    return serializerCache[type];
                }

                var serializer = new XmlSerializer(type);
                serializerCache.Add(type, serializer);
                return serializer;
            }
        }

        public static XElement SerializeToXElement(object obj, XmlSerializerNamespaces namespaces = null)
        {
            var xml = new XDocument();
            using (var writer = xml.CreateWriter())
            {
                GetCachedSerializer(obj.GetType()).Serialize(writer, obj, namespaces);
            }

            return xml.Elements().Single();
        }

        public static void SerializeToXmlFile(object obj, string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var xml = XmlUtilities.SerializeToXElement(obj);
            xml.Save(filePath);
        }

        public static string SerializeToString(object obj, XmlWriterSettings writerSettings = null, XmlSerializerNamespaces namespaces = null) //  this XmlSerializer serializer, )
        {
            var stringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(stringBuilder, writerSettings))
            {
                GetCachedSerializer(obj.GetType()).Serialize(writer, obj, namespaces);
            }

            return stringBuilder.ToString();
        }

        public static TResult DeserializeFromString<TResult>(string objectXml)
        {
            using (var stringReader = new StringReader(objectXml))
            {
                return (TResult)GetCachedSerializer(typeof(TResult)).Deserialize(stringReader);
            }
        }

        public static TResult DeserializeFromXmlFile<TResult>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Deserializer could not find file: {0}", filePath));
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                XElement xml = XElement.Load(fileStream);
                return XmlUtilities.DeserializeFromXElement<TResult>(xml);
            }
        }

        public static TResult DeserializeFromXElement<TResult>(XElement x)
        {
            return (TResult)GetCachedSerializer(typeof(TResult)).Deserialize(x.CreateReader());
        }
    }
}