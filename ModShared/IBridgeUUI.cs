using System;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons.ModShared
{
    internal abstract class IBridgeUUI : MonoBehaviour
    {

        public abstract bool IsUuiAvailable { get; }

        public abstract void RegisterMod<U, C, T>(BasicIUserMod<U, C, T> modInstance)
            where U : BasicIUserMod<U, C, T>, new()
            where C : BaseController<U, C>
            where T : BasicKPanel<U, C, T>;

        protected virtual UIPanel CreateContainer<U, C, T>(BasicIUserMod<U, C, T> modInstance, float width, float height)
            where U : BasicIUserMod<U, C, T>, new()
            where C : BaseController<U, C>
            where T : BasicKPanel<U, C, T>
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel content, null);
            content.name = "Container";
            content.size = new Vector4(modInstance.TabWidth ?? width, height);
            content.eventVisibilityChanged += (x, y) => { if (y) { modInstance.ShowVersionInfoPopup(); } };
            return content;
        }

        protected static UIButton CreateTabTemplate()
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "KCTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }

        internal abstract void ResetPosition();

        internal abstract void ApplyButtonColor(bool useLowSaturationButton);
        internal abstract void Close();
        internal abstract void Open();
        internal abstract void SelectModTab();
        internal abstract void UnselectTab();
    }
}