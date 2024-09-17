using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBtn : UIMenu
{
    public GameObject topMenuObj;
    public Text[] menuText;

    IEnumerator CoShowMainMenu(E_MenuName eMenu)
    {
        yield return UIManager.Instance.StartCoroutine(UIManager.Instance.ShowMenu(eMenu));

        if (eMenu == E_MenuName.TempMenu_1)
        {
            topMenuObj.SetActive(false);
        }
        else
        {
            topMenuObj.SetActive(true);
        }
    }

    public void Click_CampaignMenu()
    {
        menuText[0].color = Color.white;
        menuText[1].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[2].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[3].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[4].color = ConstColor._UI_COLOR_TEMP_1;

        SoundManager.Instance.PlayOneShot_Effect(ConstSound._SFX_UI_TEMP);
        StartCoroutine(CoShowMainMenu(E_MenuName.TempMenu_1));
    }

    public void Click_HeroListMenu()
    {
        menuText[0].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[1].color = Color.white;
        menuText[2].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[3].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[4].color = ConstColor._UI_COLOR_TEMP_1;

        SoundManager.Instance.PlayOneShot_Effect(ConstSound._SFX_UI_TEMP);
        StartCoroutine(CoShowMainMenu(E_MenuName.TempMenu_2));
    }

    public void Click_Challenge()
    {
        menuText[0].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[1].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[2].color = ConstColor._UI_COLOR_TEMP_1;
        menuText[3].color = Color.white;
        menuText[4].color = ConstColor._UI_COLOR_TEMP_1;

        SoundManager.Instance.PlayOneShot_Effect(ConstSound._SFX_UI_TEMP);
        StartCoroutine(CoShowMainMenu(E_MenuName.TempMenu_3));
    }

    public void CommingSoon()
    {
        SoundManager.Instance.PlayOneShot_Effect(ConstSound._SFX_UI_TEMP);
        BaseManager.Instance.ShowToast(ConstGlobal._STR_TEMP);
    }

    public void OnClick_InventoryMenu()
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.ShowMenu(E_MenuName.TempMenu_4));
    }

    public void OnClick_GachaMenu()
    {
        UIManager.Instance.StartCoroutine(UIManager.Instance.ShowMenu(E_MenuName.TempMenu_4));
    }
}
