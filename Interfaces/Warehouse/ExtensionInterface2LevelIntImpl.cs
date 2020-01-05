using Klyte.Commons.Utils;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{
    public abstract class ExtensionInterface2LevelIntImpl<U, D> : DataExtensorBase<U> where U : ExtensionInterface2LevelIntImpl<U, D>, new()
    {

        [XmlElement("DictData")]
        public SimpleNonSequentialList<SimpleNonSequentialList<D>> m_cachedDictDataSaved = new SimpleNonSequentialList<SimpleNonSequentialList<D>>();


        public event Action<uint, uint, D> eventOnValueChanged;

        #region Utils R/W
        protected D SafeGet(uint idx, uint key)
        {

            if (!m_cachedDictDataSaved.ContainsKey(idx) || !m_cachedDictDataSaved[idx].ContainsKey(key))
            {
                return default;
            }

            return m_cachedDictDataSaved[idx][key];
        }
        protected void SafeSet(uint idx, uint key, D value)
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

        public void SafeCleanEntry(uint idx)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved.Remove(idx);
            }
            eventOnValueChanged?.Invoke(idx, default, default);
        }

        public void SafeCleanProperty(uint idx, uint key)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                if (m_cachedDictDataSaved[idx].ContainsKey(key))
                {
                    m_cachedDictDataSaved[idx].Remove(key);
                    eventOnValueChanged?.Invoke(idx, key, default);
                }
            }
        }
        protected SimpleNonSequentialList<D> SafeGetEntry(uint idx)
        {

            if (!m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved[idx] = new SimpleNonSequentialList<D>();
            }

            return m_cachedDictDataSaved[idx];
        }
        #endregion
    }
}
