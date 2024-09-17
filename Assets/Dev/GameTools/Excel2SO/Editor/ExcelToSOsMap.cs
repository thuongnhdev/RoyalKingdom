using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ExcelClass2SOMap", menuName = "Uniflow/Common/ExcelClass2SOMap")]
public class ExcelToSOsMap : ScriptableObject
{
    [SerializeField]
    private List<Excel2SOs> _excel2SOsList;

    // key = excel asset instance Id
    private Dictionary<int, List<ScriptableObject>> _soDict = new();
    private Dictionary<int, List<ScriptableObject>> SoDict
    {
        get
        {
            if (_soDict.Count != _excel2SOsList.Count)
            {
                _soDict.Clear();
                for (int i = 0; i < _excel2SOsList.Count; i++)
                {
                    var excel2SOs = _excel2SOsList[i];
                    _soDict[excel2SOs.excelFile.GetInstanceID()] = excel2SOs.targetSoList;
                }
            }

            return _soDict;
        }
    }

    public List<ScriptableObject> GetTargetSOs(int assetInstanceId)
    {
        SoDict.TryGetValue(assetInstanceId, out var targetSOs);

        return targetSOs;
    }

    private void OnValidate()
    {
        for (int i = 0; i < _excel2SOsList.Count; i++)
        {
            var excel2SO = _excel2SOsList[i];
            excel2SO.name = excel2SO.excelFile.name;
        }
    }
}

[System.Serializable]
public class Excel2SOs
{
    public string name;
    public DefaultAsset excelFile;
    public List<ScriptableObject> targetSoList = new();
}
