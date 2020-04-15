﻿using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

namespace Klyte.Commons.Utils
{

    public static class TabCommons
    {

        public static UIButton CreateTabTemplate(out UISprite logo, Vector2 size, UITextureAtlas textureAtlas = null)
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "UVMTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.size = size;
            KlyteMonoUtils.CreateUIElement(out logo, tabTemplate.transform, "TabIcon", new Vector4(0, 0, size.x, size.y));
            logo.atlas = textureAtlas ?? tabTemplate.atlas;

            return tabTemplate;
        }
        public static UIButton CreateTextTabTemplate(Vector2 size)
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "UVMTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.size = size;

            return tabTemplate;
        }

        public static void CreateTabLocalized(this UITabstrip stripMain, string sprite, string localeKey, string objectName, bool scroll = true, UITextureAtlas textureAtlas = null, Vector2? nullableSize = null) => CreateTabInternal<UICustomControl>(stripMain, sprite, Locale.Get(localeKey), objectName, scroll, textureAtlas, nullableSize, true);
        public static T CreateTabLocalized<T>(this UITabstrip stripMain, string sprite, string localeKey, string objectName, bool scroll = true, UITextureAtlas textureAtlas = null, Vector2? nullableSize = null) where T : UICustomControl => CreateTabInternal<T>(stripMain, sprite, Locale.Get(localeKey), objectName, scroll, textureAtlas, nullableSize, true);

        public static T CreateTabInternal<T>(this UITabstrip stripMain, string sprite, string text, string objectName, bool scroll = true, UITextureAtlas textureAtlas = null, Vector2? nullableSize = null, bool isHorizontal = true) where T : UICustomControl
        {
            Vector2 size = nullableSize ?? (isHorizontal ? new Vector2(sprite.IsNullOrWhiteSpace() ? 100 : stripMain.height, stripMain.height) : new Vector2(stripMain.width, 40));
            UIButton tab = CreateTabTemplate(out UISprite logo, size, textureAtlas);
            if (sprite.IsNullOrWhiteSpace())
            {
                tab.text = text;
            }
            else
            {
                logo.spriteName = sprite;
                tab.tooltip = text;
            }

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
