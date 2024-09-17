using Fbs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(fileName = "LocalizationStringList", menuName = "Uniflow/Common/LocalizationStringList")]
public class LocalizationStringList : ScriptableObject
{
    [SerializeField]
    private StringTableAndFont[] _tableAndFontList;

    [Header("Inspec")]
    [SerializeField]
    private StringTableAndFont _activeTableAndFont;
    public StringTableAndFont ActiveTableAndFont
    {
        get
        {
            if (_activeTableAndFont == null)
            {
                _activeTableAndFont = _tableAndFontList[0];
            }

            return _activeTableAndFont;
        }
    }

    public void Init(ApiGetLowData serverData)
    {
        int textEntryCount = serverData.DataLanguageTextLength;
        if (textEntryCount == 0)
        {
            return;
        }

        ResetAllTables();
        for (int i = 0; i < textEntryCount; i++)
        {
            var textEntry = serverData.DataLanguageText(i).Value;
            MapLineToTable(textEntry);
        }

        SetDirtyAllTables();
    }

    public string GetText(string id)
    {
        var activeTable = ActiveTableAndFont.table;
        var entry = activeTable.GetEntry(id);
        if (entry == null)
        {
            Logger.Log($"No text id [{id}], return textId as default");
            return id;
        }

        return entry.Value;
    }

    private void MapLineToTable(LanguageText line)
    {
        var properties = line.GetType().GetProperties();
        long id = line.TextId;
        for (int i = 0; i < _tableAndFontList.Length; i++)
        {
            var table = _tableAndFontList[i].table;
            var sharedTable = table.SharedData;
            string tlCode = table.LocaleIdentifier.CultureInfo.TwoLetterISOLanguageName;
            for (int j = 0; j < properties.Length; j++)
            {
                var property = properties[j];
                if (!property.Name.ToLower().Contains(tlCode))
                {
                    continue;
                }

                var shareEntry = sharedTable.GetEntry(line.TextKey);
                if (shareEntry == null)
                {
                    shareEntry = sharedTable.AddKey(line.TextKey, id);
                }

                var tableEntry = table.GetEntry(shareEntry.Id);
                if (tableEntry == null)
                {
                    tableEntry = table.CreateTableEntry();
                    tableEntry.Key = line.TextKey;
                    table.Add(id, tableEntry);
                }
                tableEntry.Value = (string)property.GetValue(line);
            }
        }
    }

    private void ResetAllTables()
    {
        for (int i = 0; i < _tableAndFontList.Length; i++)
        {
            _tableAndFontList[i].table.Clear();
            _tableAndFontList[i].table.SharedData.Clear();
        }
    }

    private void SetDirtyAllTables()
    {
#if UNITY_EDITOR
        for (int i = 0; i < _tableAndFontList.Length; i++)
        {
            UnityEditor.EditorUtility.SetDirty(_tableAndFontList[i].table);
            UnityEditor.EditorUtility.SetDirty(_tableAndFontList[i].table.SharedData);
        }
#endif
    }

    private void TrackLocaleChange(Locale newLocale)
    {
        string newTlId = newLocale.Identifier.CultureInfo.TwoLetterISOLanguageName;
        for (int i = 0; i < _tableAndFontList.Length; i++)
        {
            if (_tableAndFontList[i].table.LocaleIdentifier.CultureInfo.TwoLetterISOLanguageName == newTlId)
            {
                _activeTableAndFont = _tableAndFontList[i];
                break;
            }
        }
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += TrackLocaleChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= TrackLocaleChange;
    }

    [System.Serializable]
    public class StringTableAndFont
    {
        public StringTable table;
        public TMP_FontAsset font;
    }

#if UNITY_EDITOR
    public void Editor_ClearAllTablesAndSharedTables()
    {
        for (int i = 0; i < _tableAndFontList.Length; i++)
        {
            _tableAndFontList[i].table.Clear();
            UnityEditor.EditorUtility.SetDirty(_tableAndFontList[i].table);
            _tableAndFontList[i].table.SharedData.Clear();
            UnityEditor.EditorUtility.SetDirty(_tableAndFontList[i].table.SharedData);
        }
    }
#endif
}
