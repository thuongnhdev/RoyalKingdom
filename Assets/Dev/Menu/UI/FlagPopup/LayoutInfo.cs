using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutInfo : MonoBehaviour
{
    [SerializeField]
    private Image _imgBackgroundEmpty;
    public Image ImgBackgroundEmpty => _imgBackgroundEmpty;

    [SerializeField]
    private Image _imgBackground;
    public Image ImgBackground => _imgBackground;

    public void SetImgBackground(Sprite spr)
    {
        _imgBackground.sprite = spr;
        _imgBackground.SetNativeSize();
    }

    [SerializeField]
    private Image[] _imgMainIcon;

    public Image[] ImgMainIcon => _imgMainIcon;

    public void SetImgMainIcon(Sprite spr)
    {
      for(int i = 0;i< _imgMainIcon.Length;i++)
        {
            _imgMainIcon[i].sprite = spr;
        }
    }

    [SerializeField]
    private Image[] _imgSubIcon;

    public Image[] ImgSubIcon => _imgSubIcon;

    public void SetImgSubIcon(Sprite spr)
    {
        for (int i = 0; i < _imgSubIcon.Length; i++)
        {
            _imgSubIcon[i].sprite = spr;
        }
    }

    private int _index;

    public void InitData(int index,Sprite background, Sprite mainIcon, Sprite subIcon)
    {
        _index = index;
        _imgBackground.sprite = background;
        for(int i = 0; i< _imgMainIcon.Length;i++)
        {
            _imgMainIcon[i].sprite = mainIcon;
        }
        for (int i = 0; i < _imgSubIcon.Length; i++)
        {
            _imgSubIcon[i].sprite = subIcon;
        }
    }

}
