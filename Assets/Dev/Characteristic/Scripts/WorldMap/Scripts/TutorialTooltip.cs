using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TutorialTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmpNotice;

    private float timeDisable = 3f;
    public async void SetData(Vector3 position, string msg)
    {
        tmpNotice.text = msg;
        await Task.Delay(TimeSpan.FromSeconds(timeDisable));
       Destroy(this.gameObject);
    }
}
