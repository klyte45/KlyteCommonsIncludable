using Klyte.Commons.Utils;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{
    public abstract class ExtensionInterfaceDictionaryByEnumImpl<T, U, D> : DataExtensorBase<U> where T : Enum, IConvertible where U : ExtensionInterfaceDictionaryByEnumImpl<T, U, D>, new() where D : class
    {

        [XmlElement("DictData")]
        public SimpleEnumerableList<T, SimpleNonSequentialList<D>> m_cachedDictDataSaved = new SimpleEnumerableList<T, SimpleNonSequentialList<D>>();


        public event Action<T, uint, D> eventOnValueChanged;

        #region Utils R/W
        protected D SafeGet(T idx, uint key)
        {

            if (!m_cachedDictDataSaved.ContainsKey(idx) || !m_cachedDictDataSaved[idx].ContainsKey(key))
            {
                return null;
            }

            return m_cachedDictDataSaved[idx][key];
        }
        protected void SafeSet(T idx, uint key, D value)
        {
            if (!m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved[idx] = new SimpleNonSequentialList<D>();
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

        public void SafeCleanEntry(T idx)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved.Remove(idx);
            }
            eventOnValueChanged?.Invoke(idx, default, null);
        }

        public void SafeCleanProperty(T idx, uint key)
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
