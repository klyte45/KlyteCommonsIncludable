using Klyte.Commons.Utils;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{

    [XmlRoot("DataExtensor")]
    public abstract class DataExtensorBase<U> : IDataExtensor where U : DataExtensorBase<U>, new()
    {
        public abstract string SaveId { get; }

        public static U Instance
        {
            get {
                if (!ExtensorContainer.instance.Instances.TryGetValue(typeof(U), out IDataExtensor result) || result == null)
                {
                    ExtensorContainer.instance.Instances[typeof(U)] = new U();
                }
                return ExtensorContainer.instance.Instances[typeof(U)] as U;
            }
        }


        public IDataExtensor Deserialize(Type type, byte[] data)
        {
            string content = data[0] == '<' ? Encoding.UTF8.GetString(data) : ZipUtils.Unzip(data);
            if (CommonProperties.DebugMode) LogUtils.DoLog($"Deserializing {typeof(U)}:\n{content}");
            var result = XmlUtils.DefaultXmlDeserialize<U>(content);
            AfterDeserialize(result);
            return result;
        }

        public byte[] Serialize()
        {
            var xml = XmlUtils.DefaultXmlSerialize((U)this, CommonProperties.DebugMode);
            if (CommonProperties.DebugMode) LogUtils.DoLog($"Serializing  {typeof(U)}:\n{xml}");
            return ZipUtils.Zip(xml);
        }

        public virtual void OnReleased() { }

        public virtual void LoadDefaults() { }
        public virtual void AfterDeserialize(U loadedData) { }
    }
}
