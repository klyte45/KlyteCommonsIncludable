using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Klyte.Commons.Utils
{
    public class XmlUtils
    {
        #region XML Utils

        public static T DefaultXmlDeserialize<T>(string s)
        {
            XmlSerializer xmlser = new XmlSerializer(typeof(T));
            try
            {
                using TextReader tr = new StringReader(s);
                using XmlReader reader = XmlReader.Create(tr);
                if (xmlser.CanDeserialize(reader))
                {
                    var val = (T)xmlser.Deserialize(reader);
                    return val;
                }
                else
                {
                    LogUtils.DoErrorLog($"CAN'T DESERIALIZE {typeof(T)}!\nText : {s}");
                }
            }
            catch (Exception e)
            {
                LogUtils.DoErrorLog($"CAN'T DESERIALIZE {typeof(T)}!\nText : {s}\n{e.Message}\n{e.StackTrace}");
            }
            return default;
        }

        public static string DefaultXmlSerialize<T>(T targetObj, bool indent = true)
        {
            XmlSerializer xmlser = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings { Indent = indent };
            using StringWriter textWriter = new StringWriter();
            using XmlWriter xw = XmlWriter.Create(textWriter, settings);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xmlser.Serialize(xw, targetObj, ns);
            return textWriter.ToString();
        }

        public class ListWrapper<T>
        {
            [XmlElement("item")]
            public List<T> listVal = new List<T>();
        }

        #endregion
    }
}
