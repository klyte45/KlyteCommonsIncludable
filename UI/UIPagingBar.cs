using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class UIPagingBar : UICustomControl
    {
        private UIPanel m_pagePanel;
        private UILabel m_infoLabel;
        private UIButton m_firstPage;
        private UIButton m_backPage;
        private UITextField m_pageInput;
        private UILabel m_totalPage;
        private UIButton m_nextPage;
        private UIButton m_lastPage;

        private int m_itemsPerPage = 5;
        private int m_totalItems = 0;
        private int m_currentPage = 1;

        private int[] m_allowedItemCount = new int[] { 5, 10, 20, 50 };
        private UIDropDown m_itemsPerPageDD;

        private int TotalPages => Mathf.CeilToInt(m_totalItems / (float)m_itemsPerPage);

        public event Action<int> OnGoToPage;

        private void Awake()
        {
            m_pagePanel = GetComponent<UIPanel>();
            m_pagePanel.padding = new RectOffset(0, 0, 0, 0);
            m_pagePanel.autoLayout = true;
            m_pagePanel.autoLayoutDirection = LayoutDirection.Horizontal;
            m_pagePanel.autoLayoutPadding = new RectOffset(0, 0, 0, 3);
            m_pagePanel.clipChildren = true;
            float height = m_pagePanel.height;

            KlyteMonoUtils.CreateUIElement(out m_infoLabel, m_pagePanel.transform, "infoLabel", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .4f, height));
            m_infoLabel.verticalAlignment = UIVerticalAlignment.Middle;
            m_infoLabel.minimumSize = new Vector2(m_pagePanel.width * .4f, height);
            KlyteMonoUtils.LimitWidthAndBox(m_infoLabel, m_pagePanel.width * .4f);

            KlyteMonoUtils.CreateUIElement(out UILabel itemsPerPageLbl, m_pagePanel.transform, "itemsPerPageLbl", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .2f, height));
            itemsPerPageLbl.verticalAlignment = UIVerticalAlignment.Middle;
            itemsPerPageLbl.text = Locale.Get("K45_CMNS_ITEMSPERPAGE");
            itemsPerPageLbl.minimumSize = new Vector2(m_pagePanel.width * .2f, height);
            itemsPerPageLbl.textAlignment = UIHorizontalAlignment.Right;
            KlyteMonoUtils.LimitWidthAndBox(itemsPerPageLbl, m_pagePanel.width * .2f);

            m_itemsPerPageDD = UIHelperExtension.CloneBasicDropDownNoLabel(
                m_allowedItemCount.Select(x => x.ToString("0")).ToArray(),
                (idx) =>
                    {
                        if (idx > 0)
                        {
                            SetItemsPerpage(m_allowedItemCount[idx]);
                        }
                    },
                m_pagePanel);
            m_itemsPerPageDD.area = new Vector4(0, 0, m_pagePanel.width * .1f, height);
       

            KlyteMonoUtils.CreateUIElement(out m_firstPage, m_pagePanel.transform, "firstPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            KlyteMonoUtils.InitButton(m_firstPage, false, "ButtonMenu");
            m_firstPage.text = "<<";
            m_firstPage.eventClicked += (x, y) => GoToPage(1);
            KlyteMonoUtils.CreateUIElement(out m_backPage, m_pagePanel.transform, "backPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            KlyteMonoUtils.InitButton(m_backPage, false, "ButtonMenu");
            m_backPage.text = "<";
            m_backPage.eventClicked += (x, y) => GoToPage(m_currentPage - 1);

            KlyteMonoUtils.CreateUIElement(out m_pageInput, m_pagePanel.transform, "firstPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            KlyteMonoUtils.UiTextFieldDefaults(m_pageInput);
            m_pageInput.horizontalAlignment = UIHorizontalAlignment.Right;
            m_pageInput.numericalOnly = true;
            m_pageInput.allowNegative = false;
            m_pageInput.allowFloats = false;
            m_pageInput.verticalAlignment = UIVerticalAlignment.Top;
            m_pageInput.padding.top = (int)((height - m_pageInput.font.size) / 2f);
            m_pageInput.submitOnFocusLost = true;
            m_pageInput.eventTextSubmitted += (x, y) =>
            {
                if (int.TryParse(y, out int page))
                {
                    GoToPage(page);
                }
                else
                {
                    m_pageInput.text = m_currentPage.ToString("0");
                }
            };


            KlyteMonoUtils.CreateUIElement(out m_totalPage, m_pagePanel.transform, "firstPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            m_totalPage.prefix = "/";
            m_totalPage.verticalAlignment = UIVerticalAlignment.Middle;
            m_totalPage.minimumSize = new Vector2(m_pagePanel.width * .05f, height);
            KlyteMonoUtils.LimitWidthAndBox(m_totalPage, m_pagePanel.width * .05f);

            KlyteMonoUtils.CreateUIElement(out m_nextPage, m_pagePanel.transform, "nextPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            KlyteMonoUtils.InitButton(m_nextPage, false, "ButtonMenu");
            m_nextPage.text = ">";
            m_nextPage.eventClicked += (x, y) => GoToPage(m_currentPage + 1);
            KlyteMonoUtils.CreateUIElement(out m_lastPage, m_pagePanel.transform, "lastPage", new UnityEngine.Vector4(0, 0, m_pagePanel.width * .05f, height));
            KlyteMonoUtils.InitButton(m_lastPage, false, "ButtonMenu");
            m_lastPage.text = ">>";
            m_lastPage.eventClicked += (x, y) => GoToPage(TotalPages);

            SetNewLength(0);
            GoToPage(1);
        }

        public void SetItemsPerPageOptions(int[] newOptions)
        {
            var targetArray = newOptions.Where(x => x > 0).GroupBy(x => x).Select(x => x.Key).OrderBy(x => x).ToArray();
            if (targetArray.Length > 0)
            {
                m_allowedItemCount = newOptions.Where(x => x > 0).GroupBy(x => x).Select(x => x.Key).OrderBy(x => x).ToArray();
                m_itemsPerPageDD.items = m_allowedItemCount.Select(x => x.ToString("0")).ToArray();
                m_itemsPerPageDD.selectedIndex = 0;
            }
        }
        private void SetItemsPerpage(int newVal)
        {
            var oldItemsPerpage = m_itemsPerPage;
            m_itemsPerPage = newVal;
            m_totalPage.text = TotalPages.ToString("0");
            GoToPage(Mathf.CeilToInt(m_currentPage * (float)oldItemsPerpage / newVal));
        }

        public void SetNewLength(int length)
        {
            m_totalItems = length;
            m_totalPage.text = TotalPages.ToString("0");
            GoToPage(m_currentPage);
        }

        public void GoToPage(int pageNum)
        {
            if (pageNum > TotalPages)
            {
                pageNum = TotalPages;
            }
            if (pageNum < 1)
            {
                pageNum = 1;
            }
            m_currentPage = pageNum;
            if (m_currentPage > 1)
            {
                m_firstPage.Enable();
            }
            else
            {
                m_firstPage.Disable();
            }
            if (m_currentPage - 1 >= 1)
            {
                m_backPage.Enable();
            }
            else
            {
                m_backPage.Disable();
            }
            if (m_currentPage + 1 <= TotalPages)
            {
                m_nextPage.Enable();
            }
            else
            {
                m_nextPage.Disable();
            }
            if (m_currentPage < TotalPages)
            {
                m_lastPage.Enable();
            }
            else
            {
                m_lastPage.Disable();
            }

            m_pageInput.text = m_currentPage.ToString("0");
            m_infoLabel.text = string.Format(Locale.Get("K45_CMNS_PAGING_SHOWINGMESSAGE_FMT"), 1 + ((m_currentPage - 1) * m_itemsPerPage), Mathf.Min(m_totalItems, m_currentPage * m_itemsPerPage), m_totalItems);
            OnGoToPage?.Invoke(m_currentPage);
        }

    }
}
