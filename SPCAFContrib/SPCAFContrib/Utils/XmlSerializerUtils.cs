using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SPCAFContrib.Utils
{
    public class XmlSerializerUtils
    {
        #region methods

        public static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);

                return writer.ToString();
            }
        }

        public static T DeserializeFromString<T>(string value)
        {
            object result = DeserializeFromString(typeof(T), value);

            if (result != null)
                return (T)result;

            return default(T);
        }

        public static object DeserializeFromString(Type type, string value)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            if (!string.IsNullOrEmpty(value))
            {
                using (StringReader reader = new StringReader(value))
                {
                    return serializer.Deserialize(reader);
                }
            }

            return null;
        }

        #endregion
    }
}
