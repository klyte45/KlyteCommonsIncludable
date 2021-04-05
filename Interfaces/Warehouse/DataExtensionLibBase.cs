using Klyte.Commons.Libraries;
using Klyte.Commons.Utils;
using System;
using System.Text;

namespace Klyte.Commons.Interfaces
{
    public abstract class DataExtensionLibBase<LIB, DESC> : BasicLib<LIB, DESC>, IDataExtension
        where LIB : DataExtensionLibBase<LIB, DESC>, new()
        where DESC : ILibable
    {
        public abstract string SaveId { get; }
        public static LIB Instance
        {
            get
            {
                if (!DataContainer.instance.Instances.TryGetValue(typeof(LIB), out IDataExtension result) || result is null)
                {
                    var newItem = new LIB();
                    newItem.AfterDeserialize(newItem);
                    DataContainer.instance.Instances[typeof(LIB)] = newItem;
                }
                return DataContainer.instance.Instances[typeof(LIB)] as LIB;
            }
        }


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

            var result = XmlUtils.DefaultXmlDeserialize<LIB>(content);
            AfterDeserialize(result);
            return result;
        }

        public byte[] Serialize() => ZipUtils.Zip(XmlUtils.DefaultXmlSerialize((LIB)this, false));
        public virtual void OnReleased() { }
        public virtual void AfterDeserialize(LIB instance) { }
        public virtual void LoadDefaults() { }

        public event Action EventDataChanged;

        protected override void Save() => EventDataChanged?.Invoke();
    }
}
