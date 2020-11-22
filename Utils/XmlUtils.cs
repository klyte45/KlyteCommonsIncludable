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

        public static T CloneViaXml<T>(T input) => XmlUtils.DefaultXmlDeserialize<T>(XmlUtils.DefaultXmlSerialize(input));
        public static T DefaultXmlDeserialize<T>(string s, Action<string, Exception> OnException = null)
        { 
            var xmlser = new XmlSerializer(typeof(T));
            return DefaultXmlDeserializeImpl<T>(s, xmlser, OnException);
        }
        public static object DefaultXmlDeserialize(Type t, string s, Action<string, Exception> OnException = null)
        {
            var xmlser = new XmlSerializer(t);
            return DefaultXmlDeserializeImpl<object>(s, xmlser, OnException);
        }

        private static T DefaultXmlDeserializeImpl<T>(string s, XmlSerializer xmlser, Action<string, Exception> OnException = null)
        {
            try
            {
                using TextReader tr = new StringReader(s);
                using var reader = XmlReader.Create(tr);
                if (xmlser.CanDeserialize(reader))
                {
                    var val = (T)xmlser.Deserialize(reader);
                    return val;
                }
                else
                {
                    LogUtils.DoErrorLog($"CAN'T DESERIALIZE {typeof(T)}!\nText : {s}");
                    OnException?.Invoke(s, null);
                }
            }
            catch (Exception e)
            {
                LogUtils.DoErrorLog($"CAN'T DESERIALIZE {typeof(T)}!\nText : {s}\n{e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                OnException?.Invoke(s, e);
                throw e;
            }
            return default;
        }

        public static string DefaultXmlSerialize<T>(T targetObj, bool indent = true)
        {
            var xmlser = new XmlSerializer(targetObj?.GetType() ?? typeof(T));
            var settings = new XmlWriterSettings { Indent = indent, OmitXmlDeclaration = true };
            using var textWriter = new StringWriter();
            using var xw = XmlWriter.Create(textWriter, settings);
            var ns = new XmlSerializerNamespaces();
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
