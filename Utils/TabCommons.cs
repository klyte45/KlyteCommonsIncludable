using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

namespace Klyte.Commons.Utils
{

    public static class TabCommons
    {

        public static UIButton CreateTabTemplate(out UISprite logo, float size, UITextureAtlas textureAtlas = null)
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "UVMTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = size;
            tabTemplate.height = size;
            KlyteMonoUtils.CreateUIElement(out logo, tabTemplate.transform, "TabIcon", new Vector4(0, 0, size, size));
            logo.atlas = textureAtlas ?? tabTemplate.atlas;

            return tabTemplate;
        }

        public static void CreateTab(this UITabstrip stripMain, string sprite, string localeKey, string objectName, bool scroll = true, UITextureAtlas textureAtlas = null, float? nullableSize = null) => CreateTab<UICustomControl>(stripMain, sprite, localeKey, objectName, scroll, textureAtlas, nullableSize);


        public static T CreateTab<T>(this UITabstrip stripMain, string sprite, string localeKey, string objectName, bool scroll = true, UITextureAtlas textureAtlas = null, float? nullableSize = null) where T : UICustomControl
        {
            float size = nullableSize ?? stripMain.height;
            UIButton tab = CreateTabTemplate(out UISprite logo, size, textureAtlas);
            logo.spriteName = sprite;
            tab.tooltip = Locale.Get(localeKey);

            KlyteMonoUtils.CreateUIElement(out UIPanel contentContainer, null);
            contentContainer.name = "Container";
            contentContainer.size = new Vector4(stripMain.tabContainer.width, stripMain.tabContainer.height);
            stripMain.AddTab(objectName, tab.gameObject, contentContainer.gameObject);
            GameObject go;
            if (scroll)
            {
                go = KlyteMonoUtils.CreateScrollPanel(contentContainer, out UIScrollablePanel scrollablePanel, out _, contentContainer.width - 20, contentContainer.height - 5, new Vector3()).Self.gameObject;
                scrollablePanel.scrollPadding = new RectOffset(10, 10, 10, 10);
            }
            else
            {
                go = contentContainer.gameObject;
            }
            if (typeof(T) != typeof(UICustomControl))
            {
                return go.AddComponent<T>();
            }
            return null;
        }
    }

}
