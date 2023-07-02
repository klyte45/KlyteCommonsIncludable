using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Klyte.Commons.Utils.UtilitiesClasses
{
    [XmlRoot("XmlDictionary")]

    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable where TKey : class, new() where TValue : class, new()
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() => null;



        public void ReadXml(System.Xml.XmlReader reader)

        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            var valueSerializer = new XmlSerializer(typeof(EntryValueContainer<TKey, TValue>), "");
            reader.ReadStartElement();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.NodeType != System.Xml.XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }
                try
                {
                    var value = (EntryValueContainer<TKey, TValue>)valueSerializer.Deserialize(reader);
                    if (value.Value is IKeyGetter<TKey> keyGetter)
                    {
                        value.Id = keyGetter.GetKeyString() ?? value.Id;
                    }

                    if (value.Id == null)
                    {
                        continue;
                    }
                    Add(value.Id, value.Value ?? null);
                }
                catch (Exception e)
                {
                    LogUtils.DoErrorLog($"Error deserializing {GetType()}:\n{e.StackTrace}");
                }

            }

            reader.ReadEndElement();


        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(EntryValueContainer<TKey, TValue>), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (TKey key in Keys)
            {
                TValue value = this[key];
                valueSerializer.Serialize(writer, new EntryValueContainer<TKey, TValue>()
                {
                    Id = key,
                    Value = value
                }, ns);
                ;
            }

        }


        #endregion

    }
    [XmlRoot("Entry")]
    public class EntryValueContainer<TKey, TValue> where TKey : class, new() where TValue : class
    {
        private TKey id;

        [XmlAttribute("key")]
        [Obsolete]
        public string OldId
        {
            get => null; set
            {
                if (value is TKey strVal)
                {
                    Id = strVal;
                }
                else if (new TKey() is ILegacyConverter<TKey> conv)
                {
                    Id = conv.From(value);
                }
            }
        }
        [XmlElement]
        public TKey Id
        {
            get => id; set
            {
                if (!(value is null))
                {
                    id = value;
                }
            }
        }

        [XmlElement]
        public TValue Value { get; set; }
    }

    public interface ILegacyConverter<T> where T : class
    {
        T From(string strVal);
    }
}
