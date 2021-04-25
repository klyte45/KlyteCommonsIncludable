using ICities;
using Klyte.Commons.Utils;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{

    [XmlRoot("DataExtension")]
    public abstract class DataExtensionBase<U> : IDataExtension where U : DataExtensionBase<U>, new()
    {
        public abstract string SaveId { get; }

        public static U Instance
        {
            get {
                if (!DataContainer.instance.Instances.TryGetValue(typeof(U), out IDataExtension result) || result == null)
                {
                    DataContainer.instance.Instances[typeof(U)] = new U();
                }
                return DataContainer.instance.Instances[typeof(U)] as U;
            }
            protected set
            {
                DataContainer.instance.Instances[typeof(U)] = XmlUtils.CloneViaXml(value);
            }
        }


        public IDataExtension Deserialize(Type type, byte[] data)
        {
            string content = data[0] == '<' ? Encoding.UTF8.GetString(data) : ZipUtils.Unzip(data);
            if (CommonProperties.DebugMode) LogUtils.DoLog($"Deserializing {typeof(U)}:\n{content}");
            var result = XmlUtils.DefaultXmlDeserialize<U>(content);
            AfterDeserialize(result);
            return result;
        }

        public byte[] Serialize()
        {
            BeforeSerialize();
            var xml = XmlUtils.DefaultXmlSerialize((U)this, CommonProperties.DebugMode);
            if (CommonProperties.DebugMode) LogUtils.DoLog($"Serializing  {typeof(U)}:\n{xml}");
            return ZipUtils.Zip(xml);
        }

        public virtual void OnReleased() { }

        public virtual void LoadDefaults(ISerializableData serializableData) { }
        public virtual void BeforeSerialize() { }
        public virtual void AfterDeserialize(U loadedData) { }
    }
}
