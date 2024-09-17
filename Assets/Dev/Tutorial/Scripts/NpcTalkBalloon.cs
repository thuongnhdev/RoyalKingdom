using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NpcTalkBalloon : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private LocalizationStringList _stringList;

    public UnityEvent onNoTalkContent;
    [Header("Configs")]
    [SerializeField]
    private string[] _tutorialTextIds;
    [SerializeField]
    private ContinuousDisplayText _textDisplayer;
    [SerializeField]
    private Button _showNextContentButton;
    
    [Header("Inspec")]
    [SerializeField]
    private string[] _editOnlyContents;
    [SerializeField]
    private int _currentContentIndex = 0;

    [SerializeField] private GameEvent _onFinishShowText;
    [SerializeField] private GameEvent _onClickShowNextTepTutorial;
    public void ShowFirstContent()
    {
        _currentContentIndex = 0;
        ShowContent(0);
    }

    public void ShowNextContent()
    {
        _currentContentIndex++;
        if (_tutorialTextIds.Length <= _currentContentIndex)
        {
            onNoTalkContent.Invoke();
            return;
        }
        ShowContent(_currentContentIndex);
    }

    public void ShowContent(int contentIndex)
    {
        if (_tutorialTextIds.Length <= contentIndex)
        {
            return;
        }
        _currentContentIndex = contentIndex;
        _showNextContentButton.gameObject.SetActive(false);

        string content = _stringList.GetText(_tutorialTextIds[contentIndex]);
        if (contentIndex == 0)
        {
            _textDisplayer.ContinuousShowText(content);
            return;
        }
        _textDisplayer.HideAllTextThenShowNewText(content);
    }

    public void ActiveNextContentButton(bool active)
    {
        _showNextContentButton.enabled = active;
    }

    private void OnValidate()
    {
        _editOnlyContents = new string[_tutorialTextIds.Length];
        for (int i = 0; i < _tutorialTextIds.Length; i++)
        {
            _editOnlyContents[i] = _stringList.GetText(_tutorialTextIds[i]);
        }
    }

    public async void OnFinishShowTextDelay()
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        _onFinishShowText?.Raise();
    }

    public void OnFinishShowText()
    {
        _onFinishShowText?.Raise();
    }

    public void OnClickShowNextTepTutorial()
    {
        _onClickShowNextTepTutorial?.Raise();
    }
}
