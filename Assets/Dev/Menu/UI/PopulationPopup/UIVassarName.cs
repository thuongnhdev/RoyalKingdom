using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIVassarName : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private TMP_InputField inputFirstName;

    [SerializeField]
    private TMP_InputField inputLastName;

 
    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    public override void Open()
    {
        base.Open();
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

    public void OnClickConfirm()
    {

    }

    public void OnClickCancel()
    {

    }


}