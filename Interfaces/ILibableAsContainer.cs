using System.Linq;
using System.Xml.Serialization;
using static Klyte.Commons.Utils.XmlUtils;

namespace Klyte.Commons.Interfaces
{
    public abstract class ILibableAsContainer<D> : ILibable
    {
        [XmlIgnore]
        internal D[] m_dataArray = new D[0];

        public ListWrapper<D> Data
        {
            get => new ListWrapper<D> { listVal = m_dataArray.ToList() };
            set => m_dataArray = value.listVal.ToArray();
        }
        public abstract string SaveName { get; set; }
    }
}