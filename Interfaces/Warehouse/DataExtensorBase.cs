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
            string content;
            if (data[0] == '<')
            {
                content = Encoding.UTF8.GetString(data);
            }
            else
            {
                content = ZipUtils.Unzip(data);
            }

            return XmlUtils.DefaultXmlDeserialize<U>(content);
        }

        public byte[] Serialize() => ZipUtils.Zip(XmlUtils.DefaultXmlSerialize((U)this, false));
        public virtual void OnReleased() { }

        public virtual void LoadDefaults() { }
    }
}
