using ColossalFramework.Globalization;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System.Collections;
using System.IO;

namespace Klyte.Commons.Libraries
{
    public abstract class LibBaseFile<LIB, DESC> : BasicLib<LIB, DESC>
    where LIB : LibBaseFile<LIB, DESC>, new()
    where DESC : class,ILibable
    {
        private static LIB m_instance;
        public static LIB Instance
        {
            get
            {
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
        public string DefaultXmlFileBaseFullPath => $"{DefaultXmlFileBasePath}{Path.DirectorySeparatorChar}{XmlName}.xml";
        protected sealed override void Save() => EnsureFileExists();
        public void EnsureFileExists() => File.WriteAllText(DefaultXmlFileBaseFullPath, XmlUtils.DefaultXmlSerialize<LIB>((LIB)this));
        protected static LIB LoadInstance()
        {
            var newVal = new LIB();
            return File.Exists(newVal.DefaultXmlFileBaseFullPath)
                ? XmlUtils.DefaultXmlDeserialize<LIB>(File.ReadAllText(newVal.DefaultXmlFileBaseFullPath), (x, y) =>
                {
                    K45DialogControl.ShowModal(new K45DialogControl.BindProperties
                    {
                        title = Locale.Get("K45_CMNS_LIB_ANERROROCURREDWHILELOADING_TITLE"),
                        message = string.Format(Locale.Get("K45_CMNS_LIB_ANERROROCURREDWHILELOADING_MESSAGE"), newVal.XmlName, y?.Message ?? "InvalidSyntax"),
                        showButton1 = true,
                        textButton1 = Locale.Get("K45_CMNS_LIB_ANERROROCURREDWHILELOADING_OPT_DELETEFILE"),
                        showButton2 = true,
                        textButton2 = Locale.Get("K45_CMNS_LIB_ANERROROCURREDWHILELOADING_OPT_GOTOFILE"),
                        showButton3 = true,
                        textButton3 = Locale.Get("EXCEPTION_OK"),
                    }, (z) =>
                     {
                         if (z == 1)
                         {
                             File.Delete(newVal.DefaultXmlFileBaseFullPath);
                         }
                         if (z == 2)
                         {
                             ColossalFramework.Utils.OpenInFileBrowser(newVal.DefaultXmlFileBaseFullPath);
                             return false;
                         }
                         return true;
                     });
                })
                : newVal;
        }
    }
}