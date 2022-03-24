namespace Klyte.Commons.Utils
{
    public static class SceneUtils
    {

        public static bool IsAssetEditor => ToolManager.instance?.m_properties?.m_mode == ItemClass.Availability.AssetEditor;
    }
}
