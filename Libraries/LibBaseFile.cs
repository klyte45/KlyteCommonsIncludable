using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.IO;

namespace Klyte.Commons.Libraries
{
    public abstract class LibBaseFile<LIB, DESC> : BasicLib<LIB, DESC>
    where LIB : LibBaseFile<LIB, DESC>, new()
    where DESC : ILibable
    {
        private static LIB m_instance;
        public static LIB Instance
        {
            get {
                if (m_instance == null)
                {
                    m_instance = LoadInstance();
                }
                return m_instance;
            }
        }
        protected abstract string XmlName { get; }

        public static void Reload() => m_instance = null;
        private static string DefaultXmlFileBasePath => CommonProperties.ModRootFolder;
        public string DefaultXmlFileBaseFullPath => $"{DefaultXmlFileBasePath}{XmlName}.xml";
        protected sealed override void Save() => EnsureFileExists();
        public void EnsureFileExists() => File.WriteAllText(DefaultXmlFileBaseFullPath, XmlUtils.DefaultXmlSerialize<LIB>((LIB)this));
        protected static LIB LoadInstance()
        {
            var newVal = new LIB();
            if (File.Exists(newVal.DefaultXmlFileBaseFullPath))
            {
                return XmlUtils.DefaultXmlDeserialize<LIB>(File.ReadAllText(newVal.DefaultXmlFileBaseFullPath));
            }
            return newVal;
        }
    }
}