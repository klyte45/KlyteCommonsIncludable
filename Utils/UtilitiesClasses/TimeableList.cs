
using Klyte.Commons.Interfaces;
using System;
using System.Collections.Generic;

using System.Xml.Serialization;


namespace Klyte.Commons.Utils
{
    [XmlRoot("TimeableList")]

    public class TimeableList<TValue> : SortedDictionary<long, TValue>, IXmlSerializable where TValue : ITimeable
    {

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() => null;



        public void ReadXml(System.Xml.XmlReader reader)

        {
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
                if (value.TimeOfDay == null)
                {
                    continue;
                }
                Add(value.TimeOfDay.Ticks, value);

            }

            reader.ReadEndElement();


        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(TValue), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (var key in Keys)
            {
                TValue value = this[key];
                if (value.TimeOfDay == null)
                {
                    value.TimeOfDay = new TimeSpan(key);
                }
                valueSerializer.Serialize(writer, value, ns);
            }

        }

        public new TValue this[long key]
        {
            get => base[key];
            set {
                Remove(key);
                if(value.TimeOfDay == null)
                {
                    value.TimeOfDay = new TimeSpan(key);
                }
                base[value.TimeOfDay.Ticks] = value;
            }
        }

        public new void Add(long key, TValue value)
        {
            Remove(key);
            if (value.TimeOfDay == null)
            {
                value.TimeOfDay = new TimeSpan(key);
            }
            base.Add(value.TimeOfDay.Ticks, value);
        }



        #endregion

    }
}
