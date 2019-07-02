using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class UIRadialChartExtended : UIRadialChart
    {
        public void AddSlice(Color32 innerColor, Color32 outterColor)
        {
            SliceSettings slice = new UIRadialChart.SliceSettings
            {
                outterColor = outterColor,
                innerColor = innerColor
            };
            this.m_Slices.Add(slice);
            this.Invalidate();
        }
        public void SetValues(float offset, int[] percentages)
        {
            if (percentages.Length != this.sliceCount)
            {
                CODebugBase<InternalLogChannel>.Error(InternalLogChannel.UI, string.Concat(new object[]
                {
            "Percentage count should be ",
            sliceCount,
            " but is ",
            percentages.Length
                }), base.gameObject);
                return;
            }
            float num = offset;
            for (int i = 0; i < this.sliceCount; i++)
            {
                SliceSettings sliceSettings = this.m_Slices[i];
                sliceSettings.Setter(null);
                sliceSettings.startValue = Mathf.Max(num % 1, 0f);
                num += percentages[i] * 0.01f;
                sliceSettings.endValue = Mathf.Min(num % 1, 1f);
            }
            this.Invalidate();
        }
        public void SetValuesStarts(int[] starts)
        {
            if (starts.Length != sliceCount)
            {
                CODebugBase<InternalLogChannel>.Error(InternalLogChannel.UI, string.Concat(new object[]
                {
            "Starts count should be ",
            sliceCount,
            " but is ",
            starts.Length
                }), base.gameObject);
                return;
            }
            float num = 0;
            for (int i = 0; i < sliceCount; i++)
            {
                SliceSettings sliceSettings = this.m_Slices[i];
                sliceSettings.Setter(null);
                sliceSettings.startValue = num;
                if (i == sliceCount - 1)
                {
                    num = 1f;
                }
                else
                {
                    num = (starts[i + 1]) * 0.01f;
                }
                sliceSettings.endValue = num;
            }
            this.Invalidate();
        }
    }
}
