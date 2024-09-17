using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;

public class UICustomFlag : BasePanel
{
    private BasePanel previousPanel;

    private MasterDataStoreGlobal _masterDataStoreGlobal;

    [SerializeField]
    private GameObject _prefabBackground;

    [SerializeField]
    private RectTransform _parentBackground;

    [SerializeField]
    private Sprite[] _sprBackground;

    [SerializeField]
    private GameObject _prefabMainIcon;

    [SerializeField]
    private RectTransform _parentMainIcon;

    [SerializeField]
    private Sprite[] _sprMainIcon;

    [SerializeField]
    private GameObject _prefabSubIcon;

    [SerializeField]
    private RectTransform _parentSubIcon;

    [SerializeField]
    private Sprite[] _sprSubIcon;

    [SerializeField]
    private GameObject _prefabLayout;

    [SerializeField]
    private RectTransform _parentLayout;

    [SerializeField]
    private Sprite[] _sprLayout;

    [SerializeField]
    private GameObject _objColorBackground;

    [SerializeField]
    private Scrollbar _colorBgScrollBarTop;

    [SerializeField]
    private Scrollbar _colorBgScrollBarBottom;

    [SerializeField]
    private GameObject _objColorMainIcon;

    [SerializeField]
    private Scrollbar _colorMainIconScrollBar;

    private List<Color> _colorTableBg = new List<Color>();

    private List<Color> _colorTableMain = new List<Color>();

    private List<Color> _colorTableSub = new List<Color>();

    [SerializeField]
    private Image[] _imageColorBackground;

    [SerializeField]
    private Image[] _imageColorMainIcon;

    [SerializeField]
    private LayoutInfo[] _layoutInfo;

    private List<RandomBadge> RandomBadges = new List<RandomBadge>();

    private int _idBackground;
    private int _idMainIcon;
    private int _idSubIcon;
    private int _idLayout;

    private int _backgroundTopColor;
    private int _backgroundBottomColor;
    private int _mainIconColor;
    private int _subIconColor;
    private LayoutInfo _layoutInfoCurrent;
    private List<ItembackgroundFlag> ItembackgroundFlag = new List<ItembackgroundFlag>();
    private List<ItemMainIconFlag> ItemMainIconFlag = new List<ItemMainIconFlag>();
    private List<ItemLayoutFlag> ItemLayoutFlag = new List<ItemLayoutFlag>();
    private List<ItemSubIconFlag> ItemSubIconFlag = new List<ItemSubIconFlag>();
    private void OnEnable()
    {
        _colorBgScrollBarTop.onValueChanged.AddListener((float val) => ScrollbarBackgroundTopCallback(val));
        _colorBgScrollBarBottom.onValueChanged.AddListener((float val) => ScrollbarBackgroundBottomCallback(val));
        _colorMainIconScrollBar.onValueChanged.AddListener((float val) => ScrollbarMainIconCallback(val));
    }
    private void OnDisable()
    {
        //Un-Reigister to event
        _colorBgScrollBarTop.onValueChanged.RemoveListener((float val) => ScrollbarBackgroundTopCallback(val));
        _colorBgScrollBarBottom.onValueChanged.RemoveListener((float val) => ScrollbarBackgroundBottomCallback(val));
        _colorMainIconScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarMainIconCallback(val));
    }

  
    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    public override void Open()
    {
        base.Open();

        _masterDataStoreGlobal = MasterDataStoreGlobal.Instance;
        InitContent();
    }
   
    private void DestroyContent()
    {
        foreach (Transform child in _parentBackground)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItembackgroundFlag.Clear();

        foreach (Transform child in _parentLayout)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemLayoutFlag.Clear();

        foreach (Transform child in _parentSubIcon)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemSubIconFlag.Clear();

        foreach (Transform child in _parentMainIcon)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < _layoutInfo.Length; i++)
        {
            _layoutInfo[i].gameObject.SetActive(false);
        }
        _colorBgScrollBarTop.value = 0;
        _colorBgScrollBarBottom.value = 0;
        _colorMainIconScrollBar.value = 0;
        _objColorBackground.SetActive(false);
        _objColorMainIcon.SetActive(false);
        ItemMainIconFlag.Clear();
    }

    private void InitContent()
    {
        DestroyContent();
      
        for (int i = 0; i < _masterDataStoreGlobal.BaseDataSymbolBgs.Count; i++)
        {
            GameObject item = Instantiate(_prefabBackground, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentBackground;

            ItembackgroundFlag itembackgroundFlag = item.GetComponent<ItembackgroundFlag>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itembackgroundFlag.InitData(i, _sprBackground[i], _masterDataStoreGlobal.BaseDataSymbolBgs[i], (index, baseSymbolIconBG) => { OnSelectBackground(index, baseSymbolIconBG); });
            ItembackgroundFlag.Add(itembackgroundFlag);
        }
    
        for (int i = 0; i < _masterDataStoreGlobal.BaseDataSymbolIconMains.Count; i++)
        {
            GameObject item = Instantiate(_prefabMainIcon, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentMainIcon;

            ItemMainIconFlag itemMainIconFlag = item.GetComponent<ItemMainIconFlag>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itemMainIconFlag.InitData(i, _sprMainIcon[i], _masterDataStoreGlobal.BaseDataSymbolIconMains[i], (index, baseSymbolIconMain) => { OnSelectMainIcon(index, baseSymbolIconMain); });
            ItemMainIconFlag.Add(itemMainIconFlag);
        }
       
        for (int i = 0; i < _masterDataStoreGlobal.BaseDataSymbolIconSubs.Count; i++)
        {
            GameObject item = Instantiate(_prefabSubIcon, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentSubIcon;

            ItemSubIconFlag itemSubIconFlag = item.GetComponent<ItemSubIconFlag>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itemSubIconFlag.InitData(i, _sprSubIcon[i], _masterDataStoreGlobal.BaseDataSymbolIconSubs[i],(index,baseSymbolIconSub)=> { OnSelectSubIcon(index, baseSymbolIconSub); });
            ItemSubIconFlag.Add(itemSubIconFlag);
        }

        for (int i = 0; i < _masterDataStoreGlobal.BaseDataSymbolLayouts.Count; i++)
        {
            GameObject item = Instantiate(_prefabLayout, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentLayout;

            ItemLayoutFlag itemLayoutFlag = item.GetComponent<ItemLayoutFlag>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itemLayoutFlag.InitData(i,_sprLayout[i], _masterDataStoreGlobal.BaseDataSymbolLayouts[i], (index, baseSymbolLayout) => { OnSelectLayout(index, baseSymbolLayout); });
            ItemLayoutFlag.Add(itemLayoutFlag);
        }

        _colorTableBg.Clear();
        _colorTableMain.Clear();
        _colorTableSub.Clear();
        RandomBadges.Clear();
        for (int i = 0;i< _masterDataStoreGlobal.BaseDataSymbolColors.Count;i++)
        {
            var item = _masterDataStoreGlobal.BaseDataSymbolColors[i];
            Color bgColor,mainColor,subColor;
            mainColor = hexToColor(item.Icon);
            _imageColorMainIcon[i].color = mainColor;
            _colorTableMain.Add(mainColor);

            subColor = hexToColor(item.IconDark);
            _colorTableSub.Add(subColor);

            bgColor = hexToColor(item.Background);
            _imageColorBackground[i].color = bgColor;
            _colorTableBg.Add(bgColor);

        }

        for (int i = 0; i < _masterDataStoreGlobal.BaseDataSymbolColors.Count - 4; i++)
        {
            var item_1 = _masterDataStoreGlobal.BaseDataSymbolColors[i];
            int indexNext = i + 4;
            var item_2 = _masterDataStoreGlobal.BaseDataSymbolColors[indexNext];
            Color mainColor_1,mainColor_2;
            mainColor_1 = hexToColor(item_1.Icon);
            mainColor_2 = hexToColor(item_2.Icon);
            RandomBadges.Add(new RandomBadge(i, indexNext, mainColor_1, mainColor_2));
        }
        SetSizeParent();
        SetColorDefault();
    }

    private void SetColorDefault()
    {
        int idLayout = GameData.Instance.SavedPack.SaveData.FlagData.LayoutKey - 1;
        if (idLayout < 0) idLayout = 0;
        OnSelectLayout(idLayout, _masterDataStoreGlobal.BaseDataSymbolLayouts[idLayout]);

        int idBackground = GameData.Instance.SavedPack.SaveData.FlagData.BackgroundKey - 1;
        if (idBackground < 0) idBackground = 0;
        Color stepBg = new(255, 255, 255, 255);
        int indexTopColor = GameData.Instance.SavedPack.SaveData.FlagData.BackgroundTopColor;
        if (indexTopColor < 0)
            indexTopColor = 0;
        stepBg = _colorTableBg[indexTopColor];
        Color newBg = new Color(stepBg.r, stepBg.g, stepBg.b, 255);
        _layoutInfoCurrent.ImgBackgroundEmpty.color = newBg;

        Color stepBottomBg = new(255, 255, 255, 255);
        int indexBottomColor = GameData.Instance.SavedPack.SaveData.FlagData.BackgroundBottomColor;
        if (indexBottomColor < 0)
            indexBottomColor = 0;
        stepBottomBg = _colorTableBg[indexBottomColor];
        Color newBottomBg = new Color(stepBottomBg.r, stepBottomBg.g, stepBottomBg.b, 255);
        _layoutInfoCurrent.ImgBackground.color = newBottomBg;

        _colorBgScrollBarTop.value = ConvertIndexToValueScroll(GameData.Instance.SavedPack.SaveData.FlagData.BackgroundTopColor);
        OnSelectBackground(idBackground, _masterDataStoreGlobal.BaseDataSymbolBgs[idBackground]);

        int idMainIcon = GameData.Instance.SavedPack.SaveData.FlagData.MainIconKey - 1;
        if (idMainIcon < 0) idMainIcon = 0;
        for (int i = 0; i < _layoutInfoCurrent.ImgMainIcon.Length; i++)
        {
            Color stepM = new(255, 255, 255, 255);
            int indexMainColor = GameData.Instance.SavedPack.SaveData.FlagData.MainIconColor;
            if (indexMainColor < 0) indexMainColor = 0;
            stepM = _colorTableMain[indexMainColor];
            Color newM = new Color(stepM.r, stepM.g, stepM.b, 255);
            _layoutInfoCurrent.ImgMainIcon[i].color = newM;
        }
        _colorMainIconScrollBar.value = ConvertIndexToValueScroll(GameData.Instance.SavedPack.SaveData.FlagData.MainIconColor);
        OnSelectMainIcon(idMainIcon, _masterDataStoreGlobal.BaseDataSymbolIconMains[idMainIcon]);

        int idSubIcon = GameData.Instance.SavedPack.SaveData.FlagData.SubIconKey - 1;
        if (idSubIcon < 0) idSubIcon = 0;
        for (int i = 0; i < _layoutInfoCurrent.ImgSubIcon.Length; i++)
        {
            Color stepM = new(255, 255, 255, 255);
            int indexSubColor = GameData.Instance.SavedPack.SaveData.FlagData.SubIconColor;
            if (indexSubColor < 0) indexSubColor = 0;
            stepM = _colorTableSub[indexSubColor];
            Color newM = new Color(stepM.r, stepM.g, stepM.b, 255);
            _layoutInfoCurrent.ImgSubIcon[i].color = newM;
        }
        OnSelectSubIcon(idSubIcon, _masterDataStoreGlobal.BaseDataSymbolIconSubs[idSubIcon]);
        ItembackgroundFlag[idBackground].OnSelect();
        ItemSubIconFlag[idSubIcon].OnSelect();
        ItemMainIconFlag[idMainIcon].OnSelect();
        ItemLayoutFlag[idLayout].OnSelect();
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    private void OnSelectBackground(int index, BaseDataSymbolBg baseSymbolIconBG)
    {
        if (baseSymbolIconBG == null)
        {
            return;
        }
        for (var i = 0; i < ItembackgroundFlag.Count; i++)
        {
            if (ItembackgroundFlag[i].BaseSymbolIconBG.Key != baseSymbolIconBG.Key)
                ItembackgroundFlag[i].OnReset();
        }
        _layoutInfoCurrent.SetImgBackground(_sprBackground[index]);
        _idBackground = baseSymbolIconBG.Key;
    }

    void ScrollbarBackgroundTopCallback(float color)
    {
        int indexColor = ValueScrollInCrease(color);
        if(indexColor != _backgroundTopColor)
        {
            Color step = _colorTableBg[indexColor];
            Color newC = new Color(step.r, step.g, step.b, 255);
            _layoutInfoCurrent.ImgBackgroundEmpty.color = newC;
            _backgroundTopColor = indexColor;
        }
        OnEndScrollTop();
    }

    private float _bgColorTopValue;
    private float _bgColorBottomValue;
    public void OnBeginScrollTop()
    {
        _bgColorTopValue = _colorBgScrollBarTop.value;
    }

    public void OnEndScrollTop()
    {
        float value = _colorBgScrollBarTop.value;
        int indexColor = ValueScrollInCrease(value);
        if(indexColor == _backgroundBottomColor)
        {
            if (value > _bgColorTopValue)
            {
                indexColor++;
                _colorBgScrollBarTop.value = ConvertIndexToValueScroll(indexColor);
            }
            else
            {
                indexColor--;
                _colorBgScrollBarTop.value = ConvertIndexToValueScroll(indexColor);
            }
        }
    
    }

    public void OnBeginScrollBottom()
    {
        _bgColorBottomValue = _colorBgScrollBarBottom.value;
    }

    public void OnEndScrollBottom()
    {
        float value = _colorBgScrollBarBottom.value;
        int indexColor = ValueScrollInCrease(value);
        if (indexColor == _backgroundTopColor)
        {
            if (value > _bgColorBottomValue)
            {
                indexColor++;
                _colorBgScrollBarBottom.value = ConvertIndexToValueScroll(indexColor);
            }
            else
            {
                indexColor--;
                _colorBgScrollBarBottom.value = ConvertIndexToValueScroll(indexColor);
            }
        }

    }

    void ScrollbarBackgroundBottomCallback(float color)
    {
        int indexColor = ValueScrollInCrease(color);
        if(indexColor != _backgroundTopColor)
        {
            Color step = _colorTableBg[indexColor];
            Color newC = new Color(step.r, step.g, step.b, 255);
            _layoutInfoCurrent.ImgBackground.color = newC;
            _backgroundBottomColor = indexColor;
        }
        OnEndScrollBottom();
    }

    private void OnSelectMainIcon(int index, BaseDataSymbolIconMain baseSymbolIconMain)
    {
        if (baseSymbolIconMain == null)
        {
            return;
        }
        for (var i = 0; i < ItemMainIconFlag.Count; i++)
        {
            if (ItemMainIconFlag[i].BaseSymbolIconMain.Key != baseSymbolIconMain.Key)
                ItemMainIconFlag[i].OnReset();
        }
        for(int  i = 0; i < _layoutInfoCurrent.ImgMainIcon.Length;i++)
        {
            _layoutInfoCurrent.ImgMainIcon[i].sprite = _sprMainIcon[index];
        }
        _idMainIcon = baseSymbolIconMain.Key;
    }

    void ScrollbarMainIconCallback(float color)
    {
        int indexColor = ValueScrollInCrease(color);
        Color step = _colorTableMain[indexColor];
        _mainIconColor = indexColor;
        for(int i = 0; i< _layoutInfoCurrent.ImgMainIcon.Length;i++)
        {
            Color newC = new Color(step.r, step.g, step.b, 255);
            _layoutInfoCurrent.ImgMainIcon[i].color = newC;
        }

        SubIconCallback(indexColor);
    }

    private void OnSelectSubIcon(int index, BaseDataSymbolIconSub baseSymbolIconSub)
    {
        if (baseSymbolIconSub == null)
        {
            return;
        }
        for (var i = 0; i < ItemSubIconFlag.Count; i++)
        {
            if (ItemSubIconFlag[i].BaseSymbolIconSub.Key != baseSymbolIconSub.Key)
                ItemSubIconFlag[i].OnReset();
        }
        for (int i = 0; i < _layoutInfoCurrent.ImgSubIcon.Length; i++)
        {
            _layoutInfoCurrent.ImgSubIcon[i].sprite = _sprSubIcon[index];
        }
       _idSubIcon = baseSymbolIconSub.Key;

    }

    private void SubIconCallback(int indexColor)
    {
        Color step = _colorTableSub[indexColor];
        _subIconColor = indexColor;
        for (int i = 0; i < _layoutInfoCurrent.ImgSubIcon.Length; i++)
        {
            Color newC = new Color(step.r, step.g, step.b, 255);
            _layoutInfoCurrent.ImgSubIcon[i].color = newC;
        }
    }

    private void OnSelectLayout(int index, BaseDataSymbolLayout baseSymbolLayout)
    {
        if (baseSymbolLayout == null)
        {
            return;
        }
        for (var i = 0; i < ItemLayoutFlag.Count; i++)
        {
            _layoutInfo[i].gameObject.SetActive(false);
            if (ItemLayoutFlag[i].BaseSymbolLayout.Key != baseSymbolLayout.Key)
                ItemLayoutFlag[i].OnReset();
        }
        _layoutInfoCurrent = _layoutInfo[index];
        _layoutInfo[index].gameObject.SetActive(true);
        _idLayout = baseSymbolLayout.Key;

        // set color
        Color step = _colorTableBg[_backgroundTopColor];
        Color newC = new Color(step.r, step.g, step.b, 255);
        _layoutInfoCurrent.ImgBackgroundEmpty.color = newC;

        Color stepBottom = _colorTableBg[_backgroundBottomColor];
        Color newCBottom = new Color(stepBottom.r, stepBottom.g, stepBottom.b, 255);
        _layoutInfoCurrent.ImgBackground.color = newCBottom;

        Color stepMain = _colorTableMain[_mainIconColor];
        for (int i = 0; i < _layoutInfoCurrent.ImgMainIcon.Length; i++)
        {
            Color newCMain = new Color(stepMain.r, stepMain.g, stepMain.b, 255);
            _layoutInfoCurrent.ImgMainIcon[i].color = newCMain;
        }

        Color stepSub = _colorTableSub[_subIconColor];
        for (int i = 0; i < _layoutInfoCurrent.ImgSubIcon.Length; i++)
        {
            Color newCSub = new Color(stepSub.r, stepSub.g, stepSub.b, 255);
            _layoutInfoCurrent.ImgSubIcon[i].color = newCSub;
        }
    }

    private int ValueScrollInCrease(float value)
    {
        int moodKey = 0;
        if (value > 0 && value < 0.072f) moodKey = 0;
        else if (value > 0.072f && value < 0.215f) moodKey = 1;
        else if (value > 0.215f && value < 0.357f) moodKey = 2;
        else if (value > 0.357f && value < 0.499f) moodKey = 3;
        else if (value > 0.499f && value < 0.641f) moodKey = 4;
        else if (value > 0.641f && value < 0.783f) moodKey = 5;
        else if (value > 0.783f && value < 0.925f) moodKey = 6;
        else if (value > 0.925f) moodKey = 7;
        return moodKey;
    }

    private int ValueScrollDeCrease(float value)
    {
        int moodKey = 0;
        if (value > 0 && value < 0.072f) moodKey = 0;
        else if (value > 0.072f && value < 0.215f) moodKey = 0;
        else if (value > 0.215f && value < 0.357f) moodKey = 1;
        else if (value > 0.357f && value < 0.499f) moodKey = 2;
        else if (value > 0.499f && value < 0.641f) moodKey = 3;
        else if (value > 0.641f && value < 0.783f) moodKey = 4;
        else if (value > 0.783f && value < 0.925f) moodKey = 5;
        else if (value > 0.925f) moodKey = 6;
        return moodKey;
    }

    private float ConvertIndexToValueScroll(int index)
    {
        switch(index)
        {
            case 0:
                return 0.07f;
            case 1:
                return 0.2f;
            case 2:
                return 0.3f;
            case 3:
                return 0.45f;
            case 4:
                return 0.6f;
            case 5:
                return 0.75f;
            case 6:
                return 0.9f;
            case -1:
            case 7:
                return 0.95f;
        }
        return 0;
    }

    public override void Close()
    {
        base.Close();
        if (previousPanel != null)
        {
            previousPanel.Open();
            previousPanel = null;
        }
    }

    public void OnClickColorBackground()
    {
        if (_objColorBackground.activeInHierarchy)
            _objColorBackground.SetActive(false);
        else
            _objColorBackground.SetActive(true);
    }

    public void OnClickColorMainIcon()
    {
        if (_objColorMainIcon.activeInHierarchy)
            _objColorMainIcon.SetActive(false);
        else
            _objColorMainIcon.SetActive(true);
    }

    public void OnClickColorLayout()
    {

    }

    public void OnClickUseRandom()
    {
        int indexBg = UnityEngine.Random.Range(0, _masterDataStoreGlobal.BaseDataSymbolBgs.Count - 1);
        int indexMainIcon = UnityEngine.Random.Range(0, _masterDataStoreGlobal.BaseDataSymbolIconMains.Count - 1);
        int indexSubIcon = UnityEngine.Random.Range(0, _masterDataStoreGlobal.BaseDataSymbolIconSubs.Count - 1);
        int indexLayout = UnityEngine.Random.Range(0, _masterDataStoreGlobal.BaseDataSymbolLayouts.Count - 1);
        OnSelectLayout(indexLayout, _masterDataStoreGlobal.BaseDataSymbolLayouts[indexLayout]);
        OnSelectBackground(indexBg, _masterDataStoreGlobal.BaseDataSymbolBgs[indexBg]);
        OnSelectMainIcon(indexMainIcon, _masterDataStoreGlobal.BaseDataSymbolIconMains[indexMainIcon]);
        OnSelectSubIcon(indexSubIcon, _masterDataStoreGlobal.BaseDataSymbolIconSubs[indexSubIcon]);
        ItemLayoutFlag[indexLayout].OnSelect();
        ItembackgroundFlag[indexBg].OnSelect();
        ItemMainIconFlag[indexMainIcon].OnSelect();
        ItemSubIconFlag[indexSubIcon].OnSelect();

        int randomTypeColor = UnityEngine.Random.Range(0, 2);
        int colorMain, colorBackgroundTop, colorBackgroundBottom;
        if(randomTypeColor == 0)
        {
            colorMain = UnityEngine.Random.Range(0, _colorTableMain.Count - 1);
            int radomPosBackground = UnityEngine.Random.Range(0, 2);
            if (radomPosBackground == 0)
            {
                colorBackgroundTop = colorMain - 1;
                colorBackgroundBottom = colorMain + 1;
                if (colorBackgroundTop == -1)
                    colorBackgroundTop = _colorTableBg.Count -1;
                if (colorBackgroundBottom == _colorTableBg.Count)
                    colorBackgroundBottom = 0;
            }
            else
            {
                colorBackgroundTop = colorMain + 1;
                colorBackgroundBottom = colorMain - 1;
                if (colorBackgroundBottom == -1)
                    colorBackgroundBottom = _colorTableBg.Count - 1;
                if (colorBackgroundTop == _colorTableBg.Count)
                    colorBackgroundTop = 0;
            }
        }
        else
        {
            colorMain = UnityEngine.Random.Range(0, _colorTableMain.Count - 1);
            var item = RandomBadges.Find(t => t.Id_1 == colorMain || t.Id_2 == colorMain);
            int idColorConfrontation = item.Id_2;
            if (item.Id_2 == colorMain)
                idColorConfrontation = item.Id_1;
            int radomPosBackground = UnityEngine.Random.Range(0, 2);
            if(radomPosBackground == 0)
            {
                colorBackgroundTop = idColorConfrontation - 1;
                colorBackgroundBottom = idColorConfrontation + 1;
                if (colorBackgroundTop == -1)
                    colorBackgroundTop = _colorTableBg.Count - 1;
                if (colorBackgroundBottom == _colorTableBg.Count)
                    colorBackgroundBottom = 0;
            }
            else
            {
                colorBackgroundTop = idColorConfrontation + 1;
                colorBackgroundBottom = idColorConfrontation - 1;
                if (colorBackgroundBottom == -1)
                    colorBackgroundBottom = _colorTableBg.Count - 1;
                if (colorBackgroundTop == _colorTableBg.Count)
                    colorBackgroundTop = 0;
            }
        }

        Color step = _colorTableBg[colorBackgroundTop];
        Color newC = new Color(step.r, step.g, step.b, 255);
        _layoutInfoCurrent.ImgBackgroundEmpty.color = newC;
        _backgroundTopColor = colorBackgroundTop;

        Color stepBottom = _colorTableBg[colorBackgroundBottom];
        Color newCBottom = new Color(stepBottom.r, stepBottom.g, stepBottom.b, 255);
        _layoutInfoCurrent.ImgBackground.color = newCBottom;
        _backgroundBottomColor = colorBackgroundBottom;

        Color stepMain = _colorTableMain[colorMain];
        _mainIconColor = colorMain;
        for (int i = 0; i < _layoutInfoCurrent.ImgMainIcon.Length; i++)
        {
            Color newCMain = new Color(stepMain.r, stepMain.g, stepMain.b, 255);
            _layoutInfoCurrent.ImgMainIcon[i].color = newCMain;
        }

        Color stepSub = _colorTableSub[colorMain];
        _subIconColor = colorMain;
        for (int i = 0; i < _layoutInfoCurrent.ImgSubIcon.Length; i++)
        {
            Color newCSub = new Color(stepSub.r, stepSub.g, stepSub.b, 255);
            _layoutInfoCurrent.ImgSubIcon[i].color = newCSub;
        }
        _colorBgScrollBarTop.value = ConvertIndexToValueScroll(colorBackgroundTop);
        _colorBgScrollBarBottom.value = ConvertIndexToValueScroll(colorBackgroundBottom);
        _colorMainIconScrollBar.value = ConvertIndexToValueScroll(colorMain);
        GameData.Instance.RequestSaveGame();

        SetLeft(_parentBackground, (indexBg * -152f));
        SetLeft(_parentLayout, (indexLayout * -165f));
        SetLeft(_parentMainIcon, (indexMainIcon * -107f));
        SetLeft(_parentSubIcon, (indexSubIcon * -215f));
        SetSizeParent();
    }

    private void SetSizeParent()
    {
        _parentBackground.sizeDelta = new Vector2(ItembackgroundFlag.Count * 152, _parentBackground.sizeDelta.y);
        _parentLayout.sizeDelta = new Vector2(ItemLayoutFlag.Count * 165, _parentLayout.sizeDelta.y);
        _parentMainIcon.sizeDelta = new Vector2(ItemMainIconFlag.Count * 107, _parentMainIcon.sizeDelta.y);
        _parentSubIcon.sizeDelta = new Vector2(ItemSubIconFlag.Count * 215, _parentSubIcon.sizeDelta.y);
    }

    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
    public void OnClickColorConfirm()
    {
        UIManagerWorld.Instance.ShowUISymbolTicket((isSelect)=> { OnCompleteSelect(isSelect); });
    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }

    public void OnClickBgColorTop()
    {

    }

    public void OnClickBgColorMain()
    {

    }

    private void OnCompleteSelect(bool isSelect)
    {
        if(isSelect)
        {
            GameData.Instance.SavedPack.SaveData.FlagData.LayoutKey = _idLayout;
            GameData.Instance.SavedPack.SaveData.FlagData.BackgroundKey = _idBackground;
            GameData.Instance.SavedPack.SaveData.FlagData.MainIconKey = _idMainIcon;
            GameData.Instance.SavedPack.SaveData.FlagData.SubIconKey = _idSubIcon;
            GameData.Instance.SavedPack.SaveData.FlagData.BackgroundTopColor = _backgroundTopColor;
            GameData.Instance.SavedPack.SaveData.FlagData.BackgroundBottomColor = _backgroundBottomColor;
            GameData.Instance.SavedPack.SaveData.FlagData.MainIconColor = _mainIconColor;
            GameData.Instance.SavedPack.SaveData.FlagData.SubIconColor = _subIconColor;
            GameData.Instance.RequestSaveGame();
        }
    }
}
public class RandomBadge
{
    public int Id_1;
    public int Id_2;
    public Color Color_1;
    public Color Color_2;
    public RandomBadge(int id1, int id2,Color color1, Color color2)
    {
        Id_1 = id1;
        Id_2 = id2;
        Color_1 = color1;
        Color_2 = color2;
    }
}