using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDisplaySwitcher : MonoBehaviour
{
    [SerializeField]
    private Button _targetButton;

    [Header("Icon")]
    [SerializeField]
    private Image _targetIcon;
    [SerializeField]
    private Sprite _activeSprite;
    [SerializeField]
    private Sprite _inactiveSprite;

    [Header("Text")]
    [SerializeField]
    private TMP_Text _buttonActiveText;
    [SerializeField]
    private TMP_Text _buttonInactiveText;

    [Header("Size")]
    [SerializeField]
    private Vector2 _inactiveSize = Vector2.one;

    private bool _buttonInteractable = false;

    private CancellationTokenSource _trackInteractableToken;

    public void SetText(string text)
    {
        _buttonActiveText.text = text;
        _buttonInactiveText.text = text;
    }

    private void OnEnable()
    {
        NextFrame_TrackButtonInteractable().Forget();
    }

    private void OnDisable()
    {
        _trackInteractableToken.Cancel();
    }

    private void OnValidate()
    {
        if (_targetButton != null) 
        {
            return;
        }
        _targetButton = GetComponent<Button>();
    }

    private async UniTaskVoid NextFrame_TrackButtonInteractable()
    {
        await UniTask.NextFrame();
        _buttonInteractable = _targetButton.interactable;
        SetUpDisplay();
        StartTrackInteractable().Forget();
    }

    private async UniTaskVoid StartTrackInteractable()
    {
        _trackInteractableToken?.Cancel();
        _trackInteractableToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_trackInteractableToken.Token))
        {
            if (_buttonInteractable == _targetButton.interactable)
            {
                continue;
            }

            _buttonInteractable = _targetButton.interactable;
            SetUpDisplay();
        }
    }

    private void SetUpDisplay()
    {
        _targetButton.transform.localScale = _buttonInteractable ? Vector3.one : _inactiveSize;

        if (_targetIcon != null)
        {
            _targetIcon.sprite = _buttonInteractable ? _activeSprite : _inactiveSprite;
        }

        if (_buttonActiveText == null || _buttonInactiveText == null)
        {
            return;
        }

        _buttonActiveText.enabled = _buttonInteractable;
        _buttonInactiveText.enabled = !_buttonInteractable;
    }
}
