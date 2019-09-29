
using Klyte.Commons.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;


namespace Klyte.Commons.Utils
{
    [XmlRoot("TimeableList")]

    public class TimeableList<TValue> : SortedDictionary<long, TValue>, IXmlSerializable where TValue : ITimeable
    {

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() => null;

        private bool m_readingXml = false;

        public void ReadXml(System.Xml.XmlReader reader)

        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            m_readingXml = true;
            try
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
            finally
            {
                m_readingXml = false;
            }
        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            var valueSerializer = new XmlSerializer(typeof(TValue), "");

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (long key in Keys)
            {
                TValue value = this[key];
                PrepareValue(key, value);
                valueSerializer.Serialize(writer, value, ns);
            }

        }

        public new TValue this[long key]
        {
            get => base[key];
            set {
                Remove(key);
                PrepareValue(key, value);
                base[value.TimeOfDay.Ticks] = value;
            }
        }

        private void PrepareValue(long key, TValue value)
        {
            if (value.TimeOfDay == null)
            {
                value.TimeOfDay = new TimeSpan(key % TimeSpan.TicksPerDay);
            }
            if (!m_readingXml && value.TimeOfDay.Ticks < Keys.Min())
            {
                value.TimeOfDay = new TimeSpan(0);
            }
        }

        public new void Add(long key, TValue value)
        {
            Remove(key);
            PrepareValue(key, value);
            base.Add(value.TimeOfDay.Ticks, value);
        }


        public TValue GetAtHour(float hour) => TryGetValue(this.Where(x => x.Key < hour * TimeSpan.TicksPerHour).Max(x => x.Key), out TValue result) ? result : default;


        #endregion

    }
}
