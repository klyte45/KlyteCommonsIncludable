using Klyte.Commons.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using static Klyte.Commons.Utils.XmlUtils;

namespace Klyte.Commons.Libraries
{
    public abstract class BasicLib<LIB, DESC>
        where LIB : BasicLib<LIB, DESC>, new()
        where DESC : class, ILibable
    {

        [XmlElement("descriptorsData")]
        public virtual ListWrapper<DESC> SavedDescriptorsSerialized
        {
            get => new ListWrapper<DESC>() { listVal = m_savedDescriptorsSerialized.Values.ToList() };
            set => m_savedDescriptorsSerialized = value.listVal.ToDictionary(x => x.SaveName, x => x);
        }

        [XmlIgnore]
        protected Dictionary<string, DESC> m_savedDescriptorsSerialized = new Dictionary<string, DESC>();


        public void Add(string indexName, ref DESC descriptor)
        {
            descriptor.SaveName = indexName;
            m_savedDescriptorsSerialized[indexName] = descriptor;


            Save();
        }


        public DESC Get(string indexName)
        {
            return m_savedDescriptorsSerialized.TryGetValue(indexName ?? "", out DESC val) ? val : null;
        }

        public IEnumerable<string> List() => m_savedDescriptorsSerialized.Keys;
        public IEnumerable<string> ListWhere(Func<DESC, bool> filter) => m_savedDescriptorsSerialized.Where(x => filter(x.Value)).Select(x => x.Key);

        public void Remove(string indexName)
        {
            if (indexName != null)
            {
                if (m_savedDescriptorsSerialized.ContainsKey(indexName))
                {
                    m_savedDescriptorsSerialized.Remove(indexName);
                    Save();
                }
            }
        }
        protected abstract void Save();
    }
}