using Klyte.Commons.Utils;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{
    public abstract class ExtensionInterfaceDictionaryByUintImpl<T, U, D> : DataExtensorBase<U> where T : Enum, IConvertible where U : ExtensionInterfaceDictionaryByUintImpl<T, U, D>, new() where D : class
    {

        [XmlElement("DictData")]
        public SimpleNonSequentialList<SimpleEnumerableList<T, D>> m_cachedDictDataSaved = new SimpleNonSequentialList<SimpleEnumerableList<T, D>>();


        public event Action<uint, T, D> eventOnValueChanged;

        #region Utils R/W
        protected D SafeGet(uint idx, T key)
        {

            if (!m_cachedDictDataSaved.ContainsKey(idx) || !m_cachedDictDataSaved[idx].ContainsKey(key))
            {
                return null;
            }

            return m_cachedDictDataSaved[idx][key];
        }
        protected void SafeSet(uint idx, T key, D value)
        {
            if (!m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved[idx] = new SimpleEnumerableList<T, D>();
            }
            if (value == null)
            {
                m_cachedDictDataSaved[idx].Remove(key);
            }
            else
            {
                m_cachedDictDataSaved[idx][key] = value;
            }
            eventOnValueChanged?.Invoke(idx, key, value);
        }

        public void SafeCleanEntry(uint idx)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved.Remove(idx);
            }
            eventOnValueChanged?.Invoke(idx, default, null);
        }

        public void SafeCleanProperty(uint idx, T key)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                if (m_cachedDictDataSaved[idx].ContainsKey(key))
                {
                    m_cachedDictDataSaved[idx].Remove(key);
                    eventOnValueChanged?.Invoke(idx, key, null);
                }
            }
        }
        #endregion
    }
}
