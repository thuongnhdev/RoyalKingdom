using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExcelSO", menuName = "Uniflow/Common/ExcelSO")]
public class ExcelGenericClass : ScriptableObject
{
    [SerializeField]
    private List<ExcelGenericRow> _rows = new();
    public int RowCount => _rows.Count;

    public void InitRows(List<ExcelGenericRow> rows)
    {
        if (rows == null || rows.Count == 0)
        {
            return;
        }

        _rows.Clear();
        for (int i = 0; i < rows.Count; i++)
        {
            _rows.Add(rows[i]);
        }
    }

    public ExcelGenericRow GetRow(int rowIndex)
    {
        if (_rows.Count <= rowIndex)
        {
            Debug.LogError($"Invalid row index [{rowIndex}], last row index is [{_rows.Count - 1}]");
            return null;
        }

        return _rows[rowIndex];
    }
}

[System.Serializable]
public class ExcelGenericRow
{
    [SerializeReference]
    public List<RootValueCell> values = new();

    private Dictionary<string, RootValueCell> _valueDict = new();
    private Dictionary<string, RootValueCell> ValueDict
    {
        get
        {
            if (values.Count != _valueDict.Count)
            {
                _valueDict.Clear();
                for (int i = 0; i < values.Count; i++)
                {
                    _valueDict[values[i].name] = values[i];
                }
            }

            return _valueDict;
        }
    }

    public bool GetValue(string columnName, out int value)
    {
        value = 0;
        ValueDict.TryGetValue(columnName, out var cell);
        if (cell == null)
        {
            Debug.LogError($"Invalid columnName [{columnName}]");
            return false;
        }

        if (!(cell is IntValueCell))
        {
            Debug.LogError($"Mismatch value type! [{columnName}] is not saved as int");
            return false;
        }

        value = ((IntValueCell)cell).value;
        return true;
    }

    public bool GetValue(string columnName, out float value)
    {
        value = 0;
        ValueDict.TryGetValue(columnName, out var cell);
        if (cell == null)
        {
            Debug.LogError($"Invalid columnName [{columnName}]");
            return false;
        }

        if (!(cell is FloatValueCell))
        {
            Debug.LogError($"Mismatch value type! [{columnName}] is not saved as float");
            return false;
        }

        value = ((FloatValueCell)cell).value;
        return true;
    }

    public bool GetValue(string columnName, out string value)
    {
        value = string.Empty;
        ValueDict.TryGetValue(columnName, out var cell);
        if (cell == null)
        {
            Debug.LogError($"Invalid columnName [{columnName}]");
            return false;
        }

        if (!(cell is StringValueCell))
        {
            Debug.LogError($"Mismatch value type! [{columnName}] is not saved as string");
            return false;
        }

        value = ((StringValueCell)cell).value;
        return true;
    }
}

[System.Serializable]
public class RootValueCell
{
    public string name;
}

[System.Serializable]
public class IntValueCell : RootValueCell
{
    [SerializeReference]
    public int value;

    public IntValueCell(int value)
    {
        this.value = value;
    }
}

[System.Serializable]
public class FloatValueCell : RootValueCell
{
    [SerializeReference]
    public float value;
    public FloatValueCell(float value)
    {
        this.value = value;
    }
}

[System.Serializable]
public class StringValueCell : RootValueCell
{
    [SerializeReference]
    public string value;
    public StringValueCell(string value)
    {
        this.value = value;
    }
}

