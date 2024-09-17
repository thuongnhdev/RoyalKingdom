using UnityEngine;
using Network.Common.Static;

public class BaseTextInfo : MonoBehaviour
{
    public TextInfo[] ObjTextList;
    private int CurrentLanguage = -1;
    private Font CurrentFont;

    private void OnEnable()
    {
        InvokeRepeating("InitText", 0f, 0.3f);   
    }


    public void InitText()
    {
        SetFont();

        for(int i = 0; i < ObjTextList.Length; i++)
        {
            if (ObjTextList[i].TxObj == null)
            {
                Debug.LogError("InitText Error [" + gameObject.name + "][" + i + "]");
            }
            else
            {
                string strError = "";
                ObjTextList[i].TxObj.font = CurrentFont;
                if (ObjTextList[i].TxType != E_GameTextDataType.none)
                {
                    //ObjTextList[i].TxObj.text = LanguageFileLoadManager.Instance.GetTranslationString(ObjTextList[i].TxType, ObjTextList[i].Key, out strError);
                    if (strError.Length > 0)
                    {
                        Debug.LogError("InitText Error [" + gameObject.name + "][" + strError + "]");
                    }
                }                
            }            
        }

        CancelInvoke("InitText");
    }

    private void SetFont()
    {
        //Debug.Log("TextData::InitText [" + CURRENT_LANGUAGE + "][" + StatesGlobal.OPT_LANGUAGE + "]");

        if (CurrentLanguage != StatesGlobal.OPT_LANGUAGE)
        {
            CurrentLanguage = StatesGlobal.OPT_LANGUAGE;
            CurrentFont = (Font)Resources.Load("Fonts/" + ConstLanguage._LANG_FONT_NAME[CurrentLanguage]);
        }
    }

   
}





