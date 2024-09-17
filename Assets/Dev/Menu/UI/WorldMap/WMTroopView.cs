using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WMTroopView : MonoBehaviour
{
    public Transform progress;
    public Image bgSelect;
    [Header("Vassal")]
    [SerializeField]
    private Image _imgAvatarVassal_1;

    [SerializeField]
    private Image _imgAvatarVassal_2;

    [SerializeField]
    private Image _imgAvatarVassal_3;

    [SerializeField]
    private TextMeshProUGUI _tmpTroopName;

    [SerializeField]
    private TextMeshProUGUI txtInfantry;
    [SerializeField]
    private TextMeshProUGUI txtCavalry;
    [SerializeField]
    private TextMeshProUGUI txtArcher;

    [SerializeField]
    private TextMeshProUGUI txtMood;

    public CityList cityList;
    public VassalSpriteSOList spriteList;

    public WorldMapTroop data { get; set; }

    private void OnValidate() {

        //bgSelect = transform.Find("content/BGTop")?.GetComponent<Image>();
        //_imgAvatarVassal_1 = transform.Find("content/vassals/AvatarVassal_1")?.GetComponent<Image>();
        //_imgAvatarVassal_2 = transform.Find("content/vassals/AvatarVassal_2")?.GetComponent<Image>();
        //_imgAvatarVassal_3 = transform.Find("content/vassals/AvatarVassal_3")?.GetComponent<Image>();

        //txtInfantry = transform.Find("content/troops/infantry/txt")?.GetComponent<TextMeshProUGUI>();
        //txtCavalry = transform.Find("content/troops/cavalry/txt")?.GetComponent<TextMeshProUGUI>();
        //txtArcher = transform.Find("content/troops/archer/txt")?.GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetModel(WorldMapTroop troop)
    {
        data = troop;
        UpdateView();
    }

    bool _selected;
    public bool Selected
    {
        get {
            return _selected;
        }
        set {
            _selected = value;
            bgSelect.color = _selected ? Color.red : Color.white;
        }
    }

    public void toggleSelect() => Selected = !Selected && !isMoving(); 

    bool isMoving() {
        return data.TargetPosition > 0 && data.TargetPosition != data.Position;
    }
    void UpdateView()
    {
        _imgAvatarVassal_1.gameObject.SetActive(data.Vassal_1 > 0);
        if (data.Vassal_1 > 0) {
            _imgAvatarVassal_1.sprite = spriteList.GetHexaIcon(data.Vassal_1);
        }

        _imgAvatarVassal_2.gameObject.SetActive(data.Vassal_2 > 0);
        if (data.Vassal_2 > 0) {
            _imgAvatarVassal_2.sprite = spriteList.GetHexaIcon(data.Vassal_2);
        }
        _imgAvatarVassal_3.gameObject.SetActive(data.Vassal_3 > 0);
        if (data.Vassal_3 > 0) {
            _imgAvatarVassal_3.sprite = spriteList.GetHexaIcon(data.Vassal_3);
        }

        txtInfantry.text = data.Value_Infantry.ToString();
        txtCavalry.text = data.Value_Cavalry.ToString();
        txtArcher.text = data.Value_Archer.ToString();

        bool inProgress = isMoving();
        progress.gameObject.SetActive(inProgress);
        if (inProgress) {
            CityPoco city = cityList.GetCity(data.TargetPosition);
            progress.GetComponentInChildren<TextMeshProUGUI>().text = $"Is moving to { city.cityName }";
        }
    }
}
