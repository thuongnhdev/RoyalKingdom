using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ContinuousDisplayText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private float _textFadeTime = 0.3f;
    [SerializeField]
    private float _showUnitPerSec = 10;
    public UnityEvent onFinishShowText;

    private Color32 _originColor;
    private float _fadeColorSpeed;
    private bool _forceShowAll = false;

    public void ContinuousShowText(string text)
    {
        _text.text = text;
        ContinuousShowText();
    }

    public void ContinuousShowText()
    {
        _forceShowAll = false;
        Color32 hideColor = _originColor;
        hideColor.a = 0;
        _text.color = hideColor;

        PerInterval_ShowCharByChar().Forget();
    }

    private async UniTaskVoid PerInterval_ShowCharByChar()
    {
        await UniTask.NextFrame(); // Wait for vertex update new text
        TMP_TextInfo textInfo = _text.textInfo;
        TMP_CharacterInfo[] charInfos = textInfo.characterInfo;
        TMP_MeshInfo[] meshInfos = textInfo.meshInfo;

        float interval = 1f / _showUnitPerSec;
        int charCount = textInfo.characterCount;
        for (int charIndex = 0; charIndex < charCount; charIndex++)
        {
            if (IsForceShowAll())
            {
                break;
            }

            var character = charInfos[charIndex];
            int firstVertexIndex = character.vertexIndex;
            int meshIndex = character.materialReferenceIndex;

            Color32[] vertexColor = meshInfos[meshIndex].colors32;
            ChangeCharacterColor(firstVertexIndex, vertexColor);

            await UniTask.Delay(System.TimeSpan.FromSeconds(interval));
            _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        ShowAllText();
    }

    /// <summary>
    /// We have to read this value via a method rather than directly reading in an async for loop to get it latest value
    /// </summary>
    private bool IsForceShowAll()
    {
        return _forceShowAll;
    }
    
    private void ChangeCharacterColor(int firstVertexIndex, Color32[] vertexColor)
    {
        if (vertexColor.Length <= firstVertexIndex + 4)
        {
            return;
        }

        vertexColor[firstVertexIndex] = _originColor;
        vertexColor[firstVertexIndex + 1] = _originColor;
        vertexColor[firstVertexIndex + 2] = _originColor;
        vertexColor[firstVertexIndex + 3] = _originColor;
    }
    
    public void ShowAllText()
    {
        _forceShowAll = true;
        _text.color = _originColor;
        onFinishShowText.Invoke();
    }

    public void HideAllTextThenShowNewText(string newText)
    {
        _forceShowAll = true;
        Async_HideAllTextThenShowNewText(newText).Forget();
    }
    private async UniTaskVoid Async_HideAllTextThenShowNewText(string newText)
    {
        await Perframe_HideAllText();
        ContinuousShowText(newText);
    }
    private CancellationTokenSource _hidingToken;
    private async UniTask Perframe_HideAllText()
    {
        _hidingToken?.Cancel();
        _hidingToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_hidingToken.Token))
        {
            Color currentColor = _text.color;
            if (currentColor.a <= 0f)
            {
                _hidingToken.Cancel();
                break;
            }
            currentColor.a -= _fadeColorSpeed * Time.deltaTime;
            _text.color = currentColor;
        }
    }

    private void OnEnable()
    {
        if (_text == null)
        {
            return;
        }

        _originColor = _text.color;
        if (_textFadeTime == 0f)
        {
            _fadeColorSpeed = float.MaxValue;
            return;
        }
        _fadeColorSpeed = (int)(1f / _textFadeTime);
    }

    private void OnValidate()
    {
        if (_text == null)
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
    }
}
