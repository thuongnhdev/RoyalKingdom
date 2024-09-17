using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemLog : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpTime;

    [SerializeField]
    private TextMeshProUGUI _tmpContent;

    public void InitData(string time ,string content)
    {
        _tmpTime.text = time.ToString();
        _tmpContent.text = content.ToString();
    }

}
