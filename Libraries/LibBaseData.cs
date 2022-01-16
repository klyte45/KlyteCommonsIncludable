using ICities;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System;
using System.Text;

namespace Klyte.Commons.Libraries
{
    public abstract class LibBaseData<LIB, DESC> : BasicLib<LIB, DESC>, IDataExtension
        where LIB : LibBaseData<LIB, DESC>, new()
        where DESC : class,ILibable
    {
        public abstract string SaveId { get; }
        public static LIB Instance
        {
            get {
                if (!DataContainer.instance.Instances.TryGetValue(typeof(LIB), out IDataExtension result) || result == null)
                {
                    DataContainer.instance.Instances[typeof(LIB)] = new LIB();
                }
                return DataContainer.instance.Instances[typeof(LIB)] as LIB;
            }
        }

        public bool IsLegacyCompatOnly { get; } = false;

        public IDataExtension Deserialize(Type type, byte[] data)
        {
            string content;
            if (data[0] == '<')
            {
                content = Encoding.UTF8.GetString(data);
            }
            else
            {
                content = ZipUtils.Unzip(data);
            }

            return XmlUtils.DefaultXmlDeserialize<LIB>(content);
        }

        public byte[] Serialize() => ZipUtils.Zip(XmlUtils.DefaultXmlSerialize((LIB)this, false));
        public virtual void OnReleased() { }

        public virtual void LoadDefaults(ISerializableData serializableData) { }

        protected override void Save() { }
    }
}
