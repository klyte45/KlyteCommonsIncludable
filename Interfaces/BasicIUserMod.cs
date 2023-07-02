using ColossalFramework;
using ICities;
using Klyte.Commons.Extensions.UI;
using System.Collections;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public abstract class BasicIUserMod<U, C, T> : BasicIUserModSimplified<U, C>
        where U : BasicIUserMod<U, C, T>, new()
        where C : BaseController<U, C>
        where T : BasicKPanel<U, C, T>
    {
        internal virtual float? TabWidth => null;

        protected virtual bool LoadUI => true;

        protected sealed override void OnLevelLoadedInherit(LoadMode mode)
        {
            base.OnLevelLoadedInherit(mode);
            if (LoadUI && IsValidLoadMode(mode))
            {
                try
                {
                    Controller.BridgeUUI.RegisterMod(this);
                }
                catch (Exception e)
                {
                    LogUtils.DoWarnLog($"Exception occurred while registering UUI. Falling back to traditional K button!\n {e}");
                    Controller.FallbackToKButton();
                    Controller.BridgeUUI.RegisterMod(this);
                }
            }
        }

        public override void Group9SettingsUI(UIHelperExtension group9)
        {
            base.Group9SettingsUI(group9);
            group9.AddButton("Reset <K> Button position", () =>
            {
                ButtonPosX.value = 5;
                ButtonPosY.value = 60;
                Controller.BridgeUUI.ResetPosition();
            });
        }

        protected override void CreateGroup9(UIHelperExtension helper)
        {
            base.CreateGroup9(helper);
            var chk_lowSat = helper.AddCheckboxLocale("K45_CMNS_USE_LOW_BRIGHT_BUTTON", UseLowSaturationButton);
            var chk_uui = helper.AddCheckboxLocale("K45_CMNS_USE_UUI_IF_AVAILABLE", UseUuiIfAvailable);
            helper.Self.eventVisibilityChanged += (x, y) =>
            {
                if (y)
                {
                    chk_lowSat.isChecked = UseLowSaturationButton;
                    chk_uui.isChecked = UseUuiIfAvailable;
                }
            };
            chk_lowSat.eventClicked += (x, y) =>
            {
                UseLowSaturationButton.value = chk_lowSat.isChecked;
                Controller.BridgeUUI.ApplyButtonColor(UseLowSaturationButton);
            };
            chk_uui.eventClicked += (x, y) =>
            {
                UseUuiIfAvailable.value = chk_uui.isChecked;
            };
        }



        public void ClosePanel() => Controller.BridgeUUI.Close();

        public void OpenPanel() => Controller.BridgeUUI.Open();

        public void OpenPanelAtModTab()
        {
            OpenPanel();
            Controller.StartCoroutine(ShowTab());
        }

        private IEnumerator ShowTab()
        {
            yield return 0;
            yield return 0;
            Controller.BridgeUUI.SelectModTab();
        }

        internal bool IsUui() => Controller.BridgeUUI.IsUuiAvailable;
        public void UnselectTab() => Controller.BridgeUUI.UnselectTab();

        public static SavedFloat ButtonPosX { get; } = new SavedFloat("K45_ButtonPosX_v2", Settings.gameSettingsFile, 5, true);
        public static SavedFloat ButtonPosY { get; } = new SavedFloat("K45_ButtonPosY_v2", Settings.gameSettingsFile, 60, true);
        public static SavedBool UseLowSaturationButton { get; } = new SavedBool("K45_UseLowSaturationButton", Settings.gameSettingsFile, true, true);

        protected override void ExtraUnloadBinds()
        {
            base.ExtraUnloadBinds();
            ClosePanel();
        }
    }

}
