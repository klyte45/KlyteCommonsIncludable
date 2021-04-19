using Klyte.Commons.Utils;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{
    public abstract class ExtensionInterfaceIndexableImpl<R, U> : DataExtensionBase<U> where R : IIdentifiable, new() where U : ExtensionInterfaceIndexableImpl<R, U>, new()
    {
        protected NonSequentialList<R> m_cachedList = new NonSequentialList<R>();


        [XmlElement("Data")]
        [Obsolete("XML Serialization only!", true)]
        public NonSequentialList<R> ItemList { get => m_cachedList; set => m_cachedList = value; }


        public event Action<long, R> EventOnValueChanged;

        protected virtual bool HasNullValue { get; } = false;

        #region Utils R/W
        public R SafeGet(long key)
        {
            if (!m_cachedList.TryGetValue(key, out R result))
            {
                result = m_cachedList[key] = new R();
            }
            return result;
        }
        public void SafeSet(R value)
        {
            if (value.Id == default)
            {
                return;
            }
            else
            {
                m_cachedList[value.Id ?? 0] = value;
            }
            EventOnValueChanged?.Invoke(value.Id ?? 0, value);
        }

        public void SafeCleanProperty(long key)
        {

            if (m_cachedList.ContainsKey(key))
            {
                m_cachedList.Remove(key);
                EventOnValueChanged?.Invoke(key, default);
            }
        }
        #endregion
    }
}
