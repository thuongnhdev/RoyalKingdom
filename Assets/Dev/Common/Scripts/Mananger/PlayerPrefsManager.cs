using UnityEngine;
using Network.Common.Static;

public class PlayerPrefsManager : Singleton<PlayerPrefsManager>
{
    bool IS_INIT = false;
    public void LoadPlayerPrefs()
    {
        if (!IS_INIT)
        {
            IS_INIT = true;

            if (PlayerPrefs.HasKey(ConstPlayerPrefs._OPT_LANGUAGE))
            {
                StatesGlobal.OPT_LANGUAGE = PlayerPrefs.GetInt(ConstPlayerPrefs._OPT_LANGUAGE, ConstLanguage._EN);
            }
            else
            {
                LoadDefaultLanguage();
            }
            StatesGlobal.OPT_GAME_QUALITY = PlayerPrefs.GetInt(ConstPlayerPrefs._OPT_GAME_QUALITY, ConstGlobal._QUALITY_HIGH);

            StatesGlobal.OPT_BGM_VOLUME = PlayerPrefs.GetFloat(ConstPlayerPrefs._OPT_BGM_VOLUME, 1f);
            StatesGlobal.OPT_EFFECT_VOLUME = PlayerPrefs.GetFloat(ConstPlayerPrefs._OPT_EFFECT_VOLUME, 1f);

            StatesGlobal.OPT_BGM = GlobalMethod.GetIntToBool(PlayerPrefs.GetInt(ConstPlayerPrefs._OPT_BGM, ConstGlobal._TRUE));
            StatesGlobal.OPT_EFFECT = GlobalMethod.GetIntToBool(PlayerPrefs.GetInt(ConstPlayerPrefs._OPT_EFFECT, ConstGlobal._TRUE));

        }
    }
    private void LoadDefaultLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                StatesGlobal.OPT_LANGUAGE = ConstLanguage._KO;
                break;
            default:
                StatesGlobal.OPT_LANGUAGE = ConstLanguage._EN;
                break;
        }
        Save_Language();
    }

    public void Save_Language()
    {
        PlayerPrefs.SetInt(ConstPlayerPrefs._OPT_LANGUAGE, StatesGlobal.OPT_LANGUAGE);
        PlayerPrefs.Save();
    }
    public void Save_Quality()
    {
        PlayerPrefs.SetInt(ConstPlayerPrefs._OPT_GAME_QUALITY, StatesGlobal.OPT_GAME_QUALITY);
        PlayerPrefs.Save();
    }
    public void Save_SoundBgmVolume()
    {
        PlayerPrefs.SetFloat(ConstPlayerPrefs._OPT_BGM_VOLUME, StatesGlobal.OPT_BGM_VOLUME);
        PlayerPrefs.Save();
    }
    public void Save_SoundEffectVolume()
    {
        PlayerPrefs.SetFloat(ConstPlayerPrefs._OPT_EFFECT_VOLUME, StatesGlobal.OPT_EFFECT_VOLUME);
        PlayerPrefs.Save();
    }

    public void Save_SoundBgm()
    {
        PlayerPrefs.SetInt(ConstPlayerPrefs._OPT_BGM, GlobalMethod.GetBoolToInt(StatesGlobal.OPT_BGM));
        PlayerPrefs.Save();
    }
    public void Save_SoundEffect()
    {
        PlayerPrefs.SetInt(ConstPlayerPrefs._OPT_EFFECT, GlobalMethod.GetBoolToInt(StatesGlobal.OPT_EFFECT));
        PlayerPrefs.Save();
    }
}
