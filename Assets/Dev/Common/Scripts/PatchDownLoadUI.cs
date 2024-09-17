using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPatchUI
{
    void SetPatchInfo(float per, string info);
    void ShowUI(bool show);
}

public abstract class PatchUI : MonoBehaviour ,IPatchUI
{
    [SerializeField]
    protected Image _progress;
    [SerializeField]
    protected Text _text;


    public abstract void SetPatchInfo(float per,string info);

    private void Awake()
    {
        if (_progress != null)
            _progress.gameObject.SetActive(false);

        if (_text != null)
            _text.gameObject.SetActive(false);
    }

    public void ShowUI(bool show)
    {
        gameObject.SetActive(show);
    }
}

public class PatchDownLoadUI :  PatchUI
{


    public override void SetPatchInfo(float per, string info)
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);


        if (_progress != null)
        {
            if (_progress.gameObject.activeSelf == false)
                _progress.gameObject.SetActive(true);

            _progress.fillAmount = per;
        }

        if (_text != null)
        {
            if (_text.gameObject.activeSelf == false)
                _text.gameObject.SetActive(true);

            _text.text = info;
        }



    }
}
