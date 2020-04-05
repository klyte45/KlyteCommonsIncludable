using Klyte.Commons.Utils;
using System;
using System.Xml.Serialization;

namespace Klyte.Commons.Interfaces
{
    public abstract class ExtensionInterfaceDictionaryStructValSimplImpl<U, T, D> : DataExtensorBase<U>
        where U : ExtensionInterfaceDictionaryStructValSimplImpl<U, T, D>, new()
        where T : class
        where D : struct
    {

        [XmlElement("DictData")]
        public SimpleXmlDictionaryStructVal<T, D> m_cachedDictDataSaved = new SimpleXmlDictionaryStructVal<T, D>();


        public event Action<T, D?> eventOnValueChanged;

        #region Utils R/W
        protected D? SafeGet(T key)
        {

            if (!m_cachedDictDataSaved.ContainsKey(key))
            {
                return null;
            }

            return m_cachedDictDataSaved[key];
        }
        protected void SafeSet(T key, D? value)
        {
            if (value == null)
            {
                m_cachedDictDataSaved.Remove(key);
            }
            else
            {
                m_cachedDictDataSaved[key] = value ?? default;
            }
            eventOnValueChanged?.Invoke(key, value);
        }


        public void SafeCleanProperty(T key)
        {
            if (m_cachedDictDataSaved.ContainsKey(key))
            {
                m_cachedDictDataSaved.Remove(key);
                eventOnValueChanged?.Invoke(key, null);
            }

        }
        #endregion
    }
}
