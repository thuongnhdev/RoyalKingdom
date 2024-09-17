using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    [System.Serializable]
    public class FlagData
    {
        public int BackgroundKey;
        public int BackgroundTopColor;
        public int BackgroundBottomColor;
        public int MainIconKey;
        public int MainIconColor;
        public int SubIconKey;
        public int SubIconColor;
        public int LayoutKey;
        public FlagData(int bgKey, int bgTopColor, int bgBottomColor, int mainKey, int mainColor, int subKey, int subColor, int layoutKey)
        {
            this.BackgroundKey = bgKey;
            this.BackgroundTopColor = bgTopColor;
            this.BackgroundBottomColor = bgBottomColor;
            this.MainIconKey = mainKey;
            this.MainIconColor = mainColor;
            this.SubIconKey = subKey;
            this.SubIconColor = subColor;
            this.LayoutKey = layoutKey;
        }
    }
}
