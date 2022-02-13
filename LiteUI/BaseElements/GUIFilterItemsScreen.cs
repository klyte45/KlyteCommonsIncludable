using ColossalFramework.Globalization;
using System;
using System.Collections;
using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    public class GUIFilterItemsScreen<S> where S : Enum
    {
        public delegate IEnumerator DoFilter(string input, Action<string[]> setOptions);
        public delegate float ExtraFiltersFunc(out bool hasChanged);

        public GUIFilterItemsScreen(string title, MonoBehaviour targetObjCoroutines, DoFilter filter, Action<int, string> onSelect, Action<S> setState, S normalState, S activeState, ExtraFiltersFunc otherFilters = null, bool acceptsNull = false, bool acceptsEmpty = false)
        {
            m_filterAction = filter;
            m_onSelect = onSelect;
            m_normalState = normalState;
            m_activeState = activeState;
            m_targetObjCoroutines = targetObjCoroutines;
            m_acceptsNull = acceptsNull;
            m_acceptsEmpty = acceptsEmpty;
            m_otherFilters = otherFilters;
            m_title = title;
            m_setState = setState;
        }

        private readonly DoFilter m_filterAction;
        private readonly Action<int, string> m_onSelect;
        private readonly S m_normalState;
        private readonly S m_activeState;
        private readonly Action<S> m_setState;
        private readonly MonoBehaviour m_targetObjCoroutines;
        private readonly bool m_acceptsNull;
        private readonly bool m_acceptsEmpty;
        private readonly ExtraFiltersFunc m_otherFilters;
        private readonly string m_title;
        private GUIStyle m_titleLabelStyle;

        private string m_searchText;
        private Coroutine m_searchCoroutine;
        private Vector2 m_searchResultPanelScroll;
        private void RestartFilterCoroutine()
        {
            if (m_searchCoroutine != null)
            {
                m_targetObjCoroutines.StopCoroutine(m_searchCoroutine);
            }
            m_searchCoroutine = m_targetObjCoroutines.StartCoroutine(OnFilterParam());
        }
        private string[] m_stringCachedResultList = new string[0];
        private IEnumerator OnFilterParam()
        {
            yield return 0;
            yield return m_filterAction(m_searchText, (x) => m_stringCachedResultList = x);
        }
        private void GotFocus()
        {
            m_searchText = "";
            RestartFilterCoroutine();
        }
        public void DrawButton(float width, string value) => GUIKlyteCommons.ButtonSelector(width, m_title, value, () => SetFocus(true));
        public void DrawSelectorView(float height)
        {
            bool dirtyInput = false;
            using (new GUILayout.HorizontalScope(GUILayout.Height(8)))
            {
                if (m_titleLabelStyle is null)
                {
                    m_titleLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                GUILayout.Label(m_title + $"<{height}>", m_titleLabelStyle);
            }
            using (new GUILayout.HorizontalScope())
            {
                var newInput = GUILayout.TextField(m_searchText);
                dirtyInput = newInput != m_searchText;
                if (dirtyInput)
                {
                    m_searchText = newInput;
                }
            }
            bool hasChanged = false;
            var usedHeight = m_otherFilters?.Invoke(out hasChanged) ?? 0;
            if (dirtyInput || hasChanged)
            {

                RestartFilterCoroutine();
            }
            using (var scroll = new GUILayout.ScrollViewScope(m_searchResultPanelScroll, GUILayout.Height(height - 72 - usedHeight)))
            {
                var selectFont = GUILayout.SelectionGrid(-1, m_stringCachedResultList, 1, new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft
                });
                if (selectFont >= 0)
                {
                    m_onSelect(selectFont, m_stringCachedResultList[selectFont]);
                    SetFocus(false);
                }
                m_searchResultPanelScroll = scroll.scrollPosition;
            };
            using (new GUILayout.HorizontalScope(GUILayout.Height(12)))
            {
                if (m_acceptsNull && GUILayout.Button(GUIKlyteCommons.v_null))
                {
                    m_onSelect(-2, null);
                    SetFocus(false);
                }
                if (m_acceptsEmpty && GUILayout.Button(GUIKlyteCommons.v_empty))
                {
                    m_onSelect(-3, "");
                    SetFocus(false);
                }
                if (GUILayout.Button(Locale.Get("CANCEL")))
                {
                    SetFocus(false);
                }
            }
        }

        private void SetFocus(bool focus)
        {
            m_setState(focus ? m_activeState : m_normalState);
            if (focus)
            {
                GotFocus();
            }
        }
    }
}