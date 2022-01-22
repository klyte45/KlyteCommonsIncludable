using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    public abstract class GUIRootWindowBase : GUIWindow
    {
        protected GUIRootWindowBase(string title, Rect rect, bool resizable = true, bool hasTitlebar = true, Vector2 minSize = default) : base(title, rect, resizable, hasTitlebar, minSize)
        {

        }

    }
}