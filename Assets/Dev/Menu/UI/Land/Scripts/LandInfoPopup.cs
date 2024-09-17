using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandInfoPopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private Canvas[] _contents;
    [SerializeField]
    private Button[] contentTabs;

    public void ActiveContent(int content)
    {
        _titleText.text = content == 0 ? "Land Info" : "Town List";
        for (int i = 0; i < _contents.Length; i++)
        {
            _contents[i].enabled = content == i;
            contentTabs[i].interactable = content != i;
        }
    }
}
