using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorScrollSpeed : MonoSingleton<EditorScrollSpeed>
{
    [SerializeField]
    private Scrollbar _workerScrollBar;

    [SerializeField]
    private Image imgFillWorkScroll;

    [SerializeField]
    private GameEvent _onSpeedWorkedTest;
    private void OnEnable()
    {
        _workerScrollBar.onValueChanged.AddListener((float val) => ScrollbarWorkerCallback(val));
    }
    private void OnDisable()
    {
        //Un-Reigister to event
        _workerScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarWorkerCallback(val));
    }

    void ScrollbarWorkerCallback(float newValue)
    {
        _workerScrollBar.value = newValue;
        imgFillWorkScroll.fillAmount = newValue;
        ShareUIManager.Instance.Config.DEFAULT_SPEED_WORKER = newValue * 20;
        _onSpeedWorkedTest?.Raise();
    }
}
