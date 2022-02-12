using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    public abstract class GUIWindow : MonoBehaviour, IDestroyableObject, IUIObject
    {
        private static readonly List<GUIWindow> Windows = new List<GUIWindow>();

        private static GUIWindow resizingWindow;
        private static Vector2 resizeDragHandle = Vector2.zero;

        private static GUIWindow movingWindow;
        private static Vector2 moveDragHandle = Vector2.zero;

        private static Texture2D highlightTexture;

        private static GUIStyle highlightstyle;

        private readonly UIPanel panel;
        private readonly int id;
        private readonly bool resizable;
        private readonly bool hasTitlebar;
        private GUISkin skin;
        private string cachedFontName = string.Empty;
        private int cachedFontSize;

        private Vector2 minSize = Vector2.zero;
        private Rect windowRect = new Rect(0, 0, 64, 64);

        private bool visible;
        private Texture2D cachedModIcon;

        public static GUIStyle HighlightStyle => GUIStyle.none;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = ".ctor of a Unity component")]
        protected GUIWindow(string title, Rect rect, bool resizable = true, bool hasTitlebar = true, Vector2 minSize = default)
        {
            id = UnityEngine.Random.Range(1024, int.MaxValue);
            Title = title;
            windowRect = rect;
            this.resizable = resizable;
            this.hasTitlebar = hasTitlebar;
            this.minSize = minSize == default ? new Vector2(64.0f, 64.0f) : minSize;
            Windows.Add(this);
            panel = gameObject.AddComponent<UIPanel>();
        }

        public Rect WindowRect => windowRect;
        protected GUISkin Skin => skin;

        public bool Visible
        {
            get => visible;

            set
            {
                var wasVisible = visible;
                visible = value;
                if (visible && !wasVisible)
                {
                    GUI.BringWindowToFront(id);
                    OnWindowOpened();
                }
            }
        }

        protected static Texture2D BgTexture { get; set; }

        protected static Texture2D ResizeNormalTexture { get; set; }

        protected static Texture2D ResizeHoverTexture { get; set; }

        protected static Texture2D CloseNormalTexture { get; set; }

        protected static Texture2D CloseHoverTexture { get; set; }

        protected static Texture2D MoveNormalTexture { get; set; }

        protected static Texture2D MoveHoverTexture { get; set; }

        protected string Title { get; set; }


        public void UpdateFont()
        {
            if (skin.font is null)
            {
                skin.font = Font.CreateDynamicFontFromOSFont(new string[0], 12);
                cachedFontName = null;
                cachedFontSize = 12;
            }
        }

        public void OnDestroy()
        {
            OnWindowDestroyed();
            Windows.Remove(this);
        }

        private Color bgColor;
        private Color titleBar;
        private Color titleBarHover;
        public void OnGUI()
        {
            if (skin == null)
            {
                bgColor = CommonProperties.ModColor.SetBrightness(.30f);
                titleBar = CommonProperties.ModColor.SetBrightness(.60f);
                titleBarHover = CommonProperties.ModColor.SetBrightness(1);

                BgTexture = new Texture2D(1, 1);
                BgTexture.SetPixel(0, 0, new Color(bgColor.r, bgColor.g, bgColor.b, 1));
                BgTexture.Apply();

                ResizeNormalTexture = new Texture2D(1, 1);
                ResizeNormalTexture.SetPixel(0, 0, Color.white);
                ResizeNormalTexture.Apply();

                ResizeHoverTexture = new Texture2D(1, 1);
                ResizeHoverTexture.SetPixel(0, 0, Color.blue);
                ResizeHoverTexture.Apply();

                CloseNormalTexture = new Texture2D(1, 1);
                CloseNormalTexture.SetPixel(0, 0, ColorExtensions.FromRGB("AA0000"));
                CloseNormalTexture.Apply();

                CloseHoverTexture = new Texture2D(1, 1);
                CloseHoverTexture.SetPixel(0, 0, ColorExtensions.FromRGB("FF6666"));
                CloseHoverTexture.Apply();

                MoveNormalTexture = new Texture2D(1, 1);
                MoveNormalTexture.SetPixel(0, 0, titleBar);
                MoveNormalTexture.Apply();

                MoveHoverTexture = new Texture2D(1, 1);
                MoveHoverTexture.SetPixel(0, 0, titleBarHover);
                MoveHoverTexture.Apply();

                skin = ScriptableObject.CreateInstance<GUISkin>();
                skin.box = new GUIStyle(GUI.skin.box);
                skin.button = new GUIStyle(GUI.skin.button);
                skin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
                skin.horizontalScrollbarLeftButton = new GUIStyle(GUI.skin.horizontalScrollbarLeftButton);
                skin.horizontalScrollbarRightButton = new GUIStyle(GUI.skin.horizontalScrollbarRightButton);
                skin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
                skin.horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
                skin.horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                skin.label = new GUIStyle(GUI.skin.label)
                {
                    richText = true
                };
                skin.scrollView = new GUIStyle(GUI.skin.scrollView);
                skin.textArea = new GUIStyle(GUI.skin.textArea);
                skin.textField = new GUIStyle(GUI.skin.textField);
                skin.toggle = new GUIStyle(GUI.skin.toggle);
                skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
                skin.verticalScrollbarDownButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
                skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                skin.verticalScrollbarUpButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
                skin.verticalSlider = new GUIStyle(GUI.skin.verticalSlider);
                skin.verticalSliderThumb = new GUIStyle(GUI.skin.verticalSliderThumb);
                skin.window = new GUIStyle(GUI.skin.window);
                skin.window.normal.background = BgTexture;
                skin.window.onNormal.background = BgTexture;


                skin.settings.cursorColor = GUI.skin.settings.cursorColor;
                skin.settings.cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                skin.settings.doubleClickSelectsWord = GUI.skin.settings.doubleClickSelectsWord;
                skin.settings.selectionColor = GUI.skin.settings.selectionColor;
                skin.settings.tripleClickSelectsLine = GUI.skin.settings.tripleClickSelectsLine;
                skin.label.richText = true;

                highlightstyle = new GUIStyle(GUI.skin.button)
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(0, 0, 0, 0)
                };
                highlightstyle.normal = highlightstyle.onNormal = new GUIStyleState();
                LoadHighlightTexture();
                highlightstyle.onHover = highlightstyle.hover = new GUIStyleState
                {
                    background = highlightTexture,
                };
            }

            if (!Visible)
            {
                panel.isVisible = false;
                return;
            }
            if (UIView.GetAView().panelsLibraryModalEffect.isVisible)
            {
                return;
            }

            panel.isVisible = true;

            var oldSkin = GUI.skin;
            if (skin != null)
            {
                UpdateFont();
                GUI.skin = skin;
            }

            var oldMatrix = GUI.matrix;
            try
            {
                GUI.matrix = UIScaler.ScaleMatrix;

                windowRect = GUI.Window(id, windowRect, WindowFunction, string.Empty);
                panel.absolutePosition = windowRect.position;
                panel.absolutePosition = windowRect.position;
                panel.size = windowRect.size;
                OnWindowDrawn();
            }
            finally
            {
                GUI.matrix = oldMatrix;

                GUI.skin = oldSkin;
            }
        }

        internal static Texture2D LoadHighlightTexture() => highlightTexture = KlyteResourceLoader.LoadTexture("commons.UI.Images.highlight.png");

        public void MoveResize(Rect newWindowRect) => windowRect = newWindowRect;

        protected static bool IsMouseOverWindow()
        {
            var mouse = UIScaler.MousePosition;
            return Windows.FindIndex(window => window.Visible && window.windowRect.Contains(mouse)) >= 0;
        }

        protected abstract void DrawWindow();

        protected virtual void HandleException(Exception ex)
        {
        }

        protected virtual void OnWindowDrawn()
        {
        }

        protected virtual void OnWindowOpened()
        {
        }

        protected virtual void OnWindowClosed()
        {
        }

        protected virtual void OnWindowResized(Vector2 size)
        {
        }

        protected virtual void OnWindowMoved(Vector2 position)
        {
        }

        protected virtual void OnWindowDestroyed()
        {
        }

        private void WindowFunction(int windowId)
        {
            FitScreen();
            GUILayout.Space(8.0f);

            try
            {
                DrawWindow();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            GUILayout.Space(16.0f);

            var mouse = UIScaler.MousePosition;

            DrawBorder();

            if (hasTitlebar)
            {
                DrawTitlebar(mouse);
                DrawCloseButton(mouse);
            }

            if (resizable)
            {
                DrawResizeHandle(mouse);
            }
        }

        private void DrawBorder()
        {
            var leftRect = new Rect(0.0f, 0.0f, 1.0f, windowRect.height);
            var rightRect = new Rect(windowRect.width - 1.0f, 0.0f, 1.0f, windowRect.height);
            var bottomRect = new Rect(0.0f, windowRect.height - 1.0f, windowRect.width, 1.0f);
            GUI.DrawTexture(leftRect, MoveNormalTexture);
            GUI.DrawTexture(rightRect, MoveNormalTexture);
            GUI.DrawTexture(bottomRect, MoveNormalTexture);
        }

        private void FitScreen()
        {
            windowRect.width = Mathf.Clamp(windowRect.width, minSize.x, UIScaler.MaxWidth);
            windowRect.height = Mathf.Clamp(windowRect.height, minSize.y, UIScaler.MaxHeight);
            windowRect.x = Mathf.Clamp(windowRect.x, 0, UIScaler.MaxWidth);
            windowRect.y = Mathf.Clamp(windowRect.y, 0, UIScaler.MaxHeight);
            if (windowRect.xMax > UIScaler.MaxWidth)
            {
                windowRect.x = UIScaler.MaxWidth - windowRect.width;
            }

            if (windowRect.yMax > UIScaler.MaxHeight)
            {
                windowRect.y = UIScaler.MaxHeight - windowRect.height;
            }
        }

        private void DrawTitlebar(Vector3 mouse)
        {
            var moveRect = new Rect(windowRect.x, windowRect.y, windowRect.width - 24f, 24.0f);
            var moveTex = MoveNormalTexture;

            // TODO: reduce nesting
            if (!GUIUtility.hasModalWindow)
            {
                if (movingWindow != null)
                {
                    if (movingWindow == this)
                    {
                        moveTex = MoveHoverTexture;
                        GUI.contentColor = titleBarHover.ContrastColor();

                        if (Input.GetMouseButton(0))
                        {
                            var pos = new Vector2(mouse.x, mouse.y) + moveDragHandle;
                            windowRect.x = pos.x;
                            windowRect.y = pos.y;
                            FitScreen();
                        }
                        else
                        {
                            movingWindow = null;

                            OnWindowMoved(windowRect.position);
                        }
                    }
                }
                else if (moveRect.Contains(mouse))
                {
                    moveTex = MoveHoverTexture;
                    GUI.contentColor = titleBarHover.ContrastColor();
                    if (Input.GetMouseButtonDown(0) && resizingWindow == null)
                    {
                        movingWindow = this;
                        moveDragHandle = new Vector2(windowRect.x, windowRect.y) - new Vector2(mouse.x, mouse.y);
                    }
                }
                else
                {
                    GUI.contentColor = titleBar.ContrastColor();
                }
            }
            else
            {
                GUI.contentColor = titleBar.ContrastColor();
            }

            GUI.DrawTexture(new Rect(0.0f, 0.0f, windowRect.width, 24.0f), moveTex, ScaleMode.StretchToFill);
            if (cachedModIcon is null)
            {
                cachedModIcon = UIView.GetAView().defaultAtlas.sprites.Where(x => x.name == CommonProperties.ModIcon).FirstOrDefault()?.texture;
                if (cachedModIcon is null)
                {
                    cachedModIcon = new Texture2D(1, 1);
                    cachedModIcon.SetPixel(0, 0, Color.clear);
                    cachedModIcon.Apply();
                }
            }
            GUI.DrawTexture(new Rect(3.0f, 0.0f, 24f, 24.0f), cachedModIcon, ScaleMode.ScaleToFit);
            GUI.Label(new Rect(30f, 0.0f, windowRect.width - 28, 22.0f), Title, new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft
            });
            GUI.contentColor = Color.white;
        }

        private void DrawCloseButton(Vector3 mouse)
        {
            var closeRect = new Rect(windowRect.x + windowRect.width - 24.0f, windowRect.y, 24.0f, 24.0f);
            var closeTex = CloseNormalTexture;

            if (!GUIUtility.hasModalWindow && closeRect.Contains(mouse))
            {
                closeTex = CloseHoverTexture;

                if (Input.GetMouseButton(0))
                {
                    resizingWindow = null;
                    movingWindow = null;
                    Visible = false;
                    OnWindowClosed();
                }
            }
            var oldColor = GUI.contentColor;
            GUI.contentColor = Color.white;
            GUI.DrawTexture(new Rect(windowRect.width - 24.0f, 0.0f, 24.0f, 24.0f), closeTex, ScaleMode.StretchToFill);
            GUI.Label(new Rect(windowRect.width - 24.0f, 0.0f, 24.0f, 24.0f), "X", new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            });
            GUI.contentColor = oldColor;

        }

        private void DrawResizeHandle(Vector3 mouse)
        {
            var resizeRect = new Rect(windowRect.x + windowRect.width - 12, windowRect.y + windowRect.height - 12, 12, 12);
            var resizeTex = ResizeNormalTexture;

            // TODO: reduce nesting
            if (!GUIUtility.hasModalWindow)
            {
                if (resizingWindow != null)
                {
                    if (resizingWindow == this)
                    {
                        resizeTex = ResizeHoverTexture;

                        if (Input.GetMouseButton(0))
                        {
                            var size = new Vector2(mouse.x, mouse.y)
                                + resizeDragHandle
                                - new Vector2(windowRect.x, windowRect.y);
                            windowRect.width = Mathf.Max(size.x, minSize.x);
                            windowRect.height = Mathf.Max(size.y, minSize.y);

                            // calling FitScreen() here causes gradual expansion of window when mouse is past the screen
                            // so we do like this:
                            windowRect.xMax = Mathf.Min(windowRect.xMax, UIScaler.MaxWidth);
                            windowRect.yMax = Mathf.Min(windowRect.yMax, UIScaler.MaxHeight);
                        }
                        else
                        {
                            resizingWindow = null;
                            OnWindowResized(windowRect.size);
                        }
                    }
                }
                else if (resizeRect.Contains(mouse))
                {
                    resizeTex = ResizeHoverTexture;
                    if (Input.GetMouseButtonDown(0))
                    {
                        resizingWindow = this;
                        resizeDragHandle =
                            new Vector2(windowRect.x + windowRect.width, windowRect.y + windowRect.height) -
                            new Vector2(mouse.x, mouse.y);
                    }
                }
            }

            GUI.DrawTexture(new Rect(windowRect.width - 12, windowRect.height - 12, 12, 12), resizeTex, ScaleMode.StretchToFill);
        }


    }
}