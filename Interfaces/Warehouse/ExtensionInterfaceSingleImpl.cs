using Klyte.Commons.Utils.UtilitiesClasses;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces.Warehouse
{
    public abstract class ExtensionInterfaceSingleImpl<T, R, U> : DataExtensionBase<U> where T : Enum, IConvertible where R : class, new() where U : ExtensionInterfaceSingleImpl<T, R, U>, new()
    {
        [XmlElement("Data")]
        public SimpleEnumerableList<T, R> m_cachedListString = new SimpleEnumerableList<T, R>();

        public event Action<T, R> EventOnValueChanged;

        protected virtual bool HasNullValue { get; } = false;

        #region Utils R/W
        protected R SafeGet(T key)
        {

            if (!m_cachedListString.ContainsKey(key) || (m_cachedListString[key] == default && !HasNullValue))
            {

                m_cachedListString[key] = HasNullValue ? default : new R();
            }

            return m_cachedListString[key];
        }
        protected void SafeSet(T key, R value)
        {

            if (value == default)
            {
                m_cachedListString.Remove(key);
            }
            else
            {
                m_cachedListString[key] = value;
            }
            EventOnValueChanged?.Invoke(key, value);
        }

        public void SafeCleanProperty(T key)
        {

            if (m_cachedListString.ContainsKey(key))
            {
                m_cachedListString.Remove(key);
                EventOnValueChanged?.Invoke(key, default);
            }
        }
        #endregion
    }
}
