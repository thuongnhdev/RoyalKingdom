using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _fpsText;

    private void Update()
    {
        if (Time.deltaTime == 0)
        {
            return;
        }
        _fpsText.text = $"{1 / Time.deltaTime} FPS";
    }
}
