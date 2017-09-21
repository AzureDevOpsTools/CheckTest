using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ConvertLogFilesToTrx
{
    public static class Serializer
    {
        /// <summary>
        /// Serializer which doesnt include std namespaces, but adds your own
        /// Object must have namespace property, see https://stackoverflow.com/questions/625927/omitting-all-xsi-and-xsd-namespaces-when-serializing-an-object-in-net/11129732#11129732
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize">whatever object</param>
        /// <param name="typename">Use nameof() here to get the correct name</param>
        /// <param name="qualifiedNs">Form: "urn:Abracadabra"</param>
        /// <returns></returns>
        public static string SerializeObjectWithNamespace<T>(this T toSerialize,string typename, string qualifiedNs)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType(),new XmlRootAttribute(typename) {Namespace = qualifiedNs});
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static string SerializeObjectWithOutNamespace<T>(this T toSerialize,string nas)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType(),"");
            var ns = new XmlSerializerNamespaces();
            ns.Add("http", nas);
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize,ns);
                return textWriter.ToString();
            }
        }


        public static T DeserializeObject<T>(string s)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(s))
            {
                var result = (T)xmlSerializer.Deserialize(textReader);
                return result;
            }
        }

    }
}