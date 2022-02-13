using ColossalFramework.Globalization;
using Klyte.Commons.LiteUI;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.UI
{
    internal class GUIBasicListingTabsContainer<T>
    {
        private int m_tabSel = 0;
        private int m_listSel = -1;
        private readonly Action m_onAdd;
        private readonly Func<string[]> m_listGetter;
        private readonly Func<int, T> m_currentItemGetter;
        internal event Action<int> EventListItemChanged;
        public int ListSel
        {
            get => m_listSel;
            private set
            {
                if (m_listSel != value)
                {
                    m_listSel = value;
                    EventListItemChanged?.Invoke(m_listSel);
                    foreach (var x in m_tabsImages)
                    {
                        x.Second.Reset();
                    }
                    m_tabSel = 0;
                }
            }
        }
        private GUIStyle m_greenButton;
        private GUIStyle GreenButton
        {
            get
            {
                if (m_greenButton is null)
                {
                    m_greenButton = new GUIStyle(GUI.skin.button)
                    {
                        normal = new GUIStyleState()
                        {
                            background = GUIKlyteCommons.darkGreenTexture,
                            textColor = Color.white
                        },
                        hover = new GUIStyleState()
                        {
                            background = GUIKlyteCommons.greenTexture,
                            textColor = Color.black
                        },
                    };
                }
                return m_greenButton;
            }
        }

        private Vector2 m_scrollPosition;

        private readonly Tuple<Texture, IGUITab<T>>[] m_tabsImages;

        public GUIBasicListingTabsContainer(Tuple<Texture, IGUITab<T>>[] tabsImages, Action onAdd, Func<string[]> listGetter, Func<int, T> currentItemGetter)
        {
            m_onAdd = onAdd;
            m_listGetter = listGetter;
            m_tabsImages = tabsImages;
            m_currentItemGetter = currentItemGetter;
        }

        public static bool LockSelection { get; internal set; } = true;

        public void DrawListTabs(Rect area)
        {
            using (new GUILayout.AreaScope(area))
            {
                var sideListArea = new Rect(0, 0, 120, area.height);
                var sideList = m_listGetter();
                var addItemText = Locale.Get("K45_WTS_SEGMENT_ADDITEMLIST");
                if (GUIKlyteCommons.CreateItemVerticalList(sideListArea, ref m_scrollPosition, ListSel, sideList, addItemText, GreenButton, out int newSel))
                {
                    m_onAdd();
                }
                ListSel = newSel;

                if (ListSel >= 0 && ListSel < sideList.Length)
                {
                    var usedHeight = 0f;

                    using (new GUILayout.AreaScope(new Rect(125, 0, area.width - 135, usedHeight += 40)))
                    {
                        m_tabSel = GUILayout.SelectionGrid(m_tabSel, m_tabsImages.Select(x => x.First).ToArray(), m_tabsImages.Length, new GUIStyle(GUI.skin.button)
                        {
                            fixedWidth = 32,
                            fixedHeight = 32,
                        });
                    }

                    var tabAreaRect = new Rect(125, usedHeight, area.width - 130, area.height - usedHeight);
                    using (new GUILayout.AreaScope(tabAreaRect))
                    {
                        if (m_tabSel >= 0 && m_tabSel < m_tabsImages.Length)
                        {
                            m_tabsImages[m_tabSel].Second.DrawArea(new Rect(0, 0, tabAreaRect.width, tabAreaRect.height), m_currentItemGetter(ListSel), ListSel);
                        }
                    };
                }
            }
        }

        internal void Reset() => ListSel = -1;
    }
    public interface IGUITab<T>
    {
        void DrawArea(Rect tabAreaRect, T currentItem, int currentItemIdx);
        void Reset();
    }
}
