using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentNpcController : MonoBehaviour
{
    [SerializeField]
    private UIController _uiController;
    [SerializeField]
    private Image _persistentNpc;
    [SerializeField]
    private bool _usePersistentNpc = false;

    public void StartUsingPersistentNpc()
    {
        _usePersistentNpc = true;
        UsePersistentNpcIfNeeded();
    }

    public void StopUsingPersistentNpc()
    {
        _persistentNpc.enabled = false;
        _usePersistentNpc = false;

        ActiveNpcOfPanel(true);
    }

    private void UsePersistentNpcIfNeeded()
    {
        if (!_usePersistentNpc)
        {
            return;
        }

        ActiveNpcOfPanel(false);
        _persistentNpc.enabled = true;
    }

    private void ActiveNpcOfPanel(bool active)
    {
        var tutorialPanel = _uiController.TopPanel;
        if (tutorialPanel == null)
        {
            return;
        }

        var npc = tutorialPanel.GetElement(1);
        if (npc == null || !npc.name.Contains("NPCContent")) // refactor this
        {
            return;
        }

        npc.gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        _uiController.OnOpenAPanel += UsePersistentNpcIfNeeded;
    }

    private void OnDisable()
    {
        _uiController.OnOpenAPanel -= UsePersistentNpcIfNeeded;
    }
}
