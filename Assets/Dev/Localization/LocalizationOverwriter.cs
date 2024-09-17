using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class LocalizationOverwriter : MonoSingleton<LocalizationOverwriter>
{
    [SerializeField]
    private LocalizationStringList _localizedStrings;
    protected override void DoOnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(LocalizeTextIfNeeded);
    }

    protected override void DoOnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(LocalizeTextIfNeeded);
    }

    private void LocalizeTextIfNeeded(object textObject)
    {
        TMP_Text text = textObject as TMP_Text;
        if (text == null)
        {
            return;
        }
        if (text.GetComponent<LocalizeStringEvent>() != null)
        {
            return;
        }

        Delay_LocalizeText(text).Forget();
    }

    private async UniTaskVoid Delay_LocalizeText(TMP_Text text)
    {
        await UniTask.NextFrame();
        var table = _localizedStrings.ActiveTableAndFont.table;
        var entry = table.GetEntry(text.text);
        if (entry == null)
        {
            return;
        }

        text.text = entry.Value;
        var targetFont = _localizedStrings.ActiveTableAndFont.font;
        if (text.font.GetInstanceID() == targetFont.GetInstanceID())
        {
            return;
        }

        text.font = _localizedStrings.ActiveTableAndFont.font;
    }
}
