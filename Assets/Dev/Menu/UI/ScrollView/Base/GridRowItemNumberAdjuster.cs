using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRowItemNumberAdjuster : MonoBehaviour
{
    [SerializeField]
    private BaseScrollController _scroller;
    [SerializeField]
    private int _itemCountFor43Screen;

    private void OnEnable()
    {
        if (_itemCountFor43Screen == 0)
        {
            return;
        }

        float ratio = (float)Screen.width / Screen.height;
        if (ratio - 4f / 3f < 0.1f)
        {
            _scroller.AdjustItemPerRow(_itemCountFor43Screen);
        }
    }
}
