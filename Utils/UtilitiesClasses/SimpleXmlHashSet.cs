using ColossalFramework.UI;
using System.Collections.Generic;

using System.Xml.Serialization;


namespace Klyte.Commons.Utils
{
    [XmlRoot("SimpleHashSet")]

    public class SimpleXmlHashSet<TValue> : HashSet<TValue>, IXmlSerializable
    {
        public SimpleXmlHashSet() { }
        public SimpleXmlHashSet(IEnumerable<TValue> enumerable) => enumerable.ForEach(x => Add(x));
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() => null;



        public void ReadXml(System.Xml.XmlReader reader)

        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            var valueSerializer = new XmlSerializer(typeof(TValue), "");
            reader.ReadStartElement();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.NodeType != System.Xml.XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                var value = (TValue) valueSerializer.Deserialize(reader);
                Add(value);

            }

            reader.ReadEndElement();


        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(TValue), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (TValue value in this)
            {
                valueSerializer.Serialize(writer, value, ns);
            }

        }


        #endregion

    }
}
