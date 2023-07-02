using System.Collections.Generic;
using System.Xml.Serialization;


namespace Klyte.Commons.Utils.UtilitiesClasses
{
    [XmlRoot("SimpleXmlDictionaryStructVal")]

    public class SimpleXmlDictionaryStructVal<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable where TKey : class where TValue : struct
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
            var valueSerializer = new XmlSerializer(typeof(EntryStructValueContainerStructVal<TKey, TValue>), "");
            reader.ReadStartElement();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.NodeType != System.Xml.XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                var value = (EntryStructValueContainerStructVal<TKey, TValue>)valueSerializer.Deserialize(reader);
                if (value.Id == null)
                {
                    continue;
                }
                Add(value.Id, value.Value);

            }

            reader.ReadEndElement();


        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(EntryStructValueContainerStructVal<TKey, TValue>), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (TKey key in Keys)
            {
                TValue value = this[key];
                valueSerializer.Serialize(writer, new EntryStructValueContainerStructVal<TKey, TValue>()
                {
                    Id = key,
                    Value = value
                }, ns);
                ;
            }

        }


        #endregion

    }

}
