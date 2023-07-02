using System.Collections.Generic;
using System.Xml.Serialization;


namespace Klyte.Commons.Utils.UtilitiesClasses
{
    [XmlRoot("SimpleXmlDictionary")]

    public class SimpleXmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable where TKey : class where TValue : class
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
            var valueSerializer = new XmlSerializer(typeof(EntryStructValueContainer<TKey, TValue>), "");
            reader.ReadStartElement();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.NodeType != System.Xml.XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                var value = (EntryStructValueContainer<TKey, TValue>)valueSerializer.Deserialize(reader);
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

            reader.ReadEndElement();


        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(EntryStructValueContainer<TKey, TValue>), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (TKey key in Keys)
            {
                TValue value = this[key];
                valueSerializer.Serialize(writer, new EntryStructValueContainer<TKey, TValue>()
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
    public class EntryStructValueContainer<TKey, TValue> where TKey : class where TValue : class
    {
        [XmlAttribute("key")]
        public TKey Id { get; set; }

        [XmlElement]
        public TValue Value { get; set; }
    }
    public interface IKeyGetter<TKey>
    {
         TKey GetKeyString();
    }

}
