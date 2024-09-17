using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
[CanEditMultipleObjects]
public class ExcelFileInspector : Editor
{
    private string _filePath;
    private ExcelToSOsMap _name2SOMap;

    private void OnEnable()
    {
        _filePath = AssetDatabase.GetAssetPath(target);
        _name2SOMap = AssetDatabase.LoadAssetAtPath<ExcelToSOsMap>("Assets/Dev/GameTools/Excel2SO/Editor/ExcelClass2SOMap.asset");
    }

    public override void OnInspectorGUI()
    {
        if (_filePath.EndsWith(".xlsx"))
        {
            GUI.enabled = true;
            DrawExcelInspector();
            return;
        }

        DrawDefaultInspector();
    }

    private void DrawExcelInspector()
    {
        if (GUILayout.Button("Generate!"))
        {
            GenerateSOFromExcel();
        }
    }

    private void GenerateSOFromExcel()
    {
        var so = _name2SOMap.GetTargetSOs(target.GetInstanceID());
        if (so == null)
        {
            ExcelImporterMaker.ExportExcelToAssetbundle();
            return;
        }

        FileInfo file = new(_filePath);
        FileInfo tempFile = file.CopyTo(_filePath.Replace(".xlsx", "temp.xlsx"));

        FileStream readStream = tempFile.OpenRead();
        ExcelPackage ep = new(readStream);
        Excel xls = new(ep.Workbook);

        GenerateCustomSO(xls, so);

        readStream.Close();
        tempFile.Delete();
    }

    private void GenerateGenericExcelClass(Excel xls)
    {
        var soRows = ParseASheet(xls.Tables[0]);

        var excelSO = (ExcelGenericClass)CreateInstance(typeof(ExcelGenericClass));
        excelSO.InitRows(soRows);

        AssetDatabase.CreateAsset(excelSO, _filePath.Replace(".xlsx", ".asset"));
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = excelSO;
        AssetDatabase.SaveAssets();
    }


    private void GenerateCustomSO(Excel xls, List<ScriptableObject> targetSOs)
    {
        if (_name2SOMap == null)
        {
            Debug.LogError("Cannot find ClassName2SOMap");
            return;
        }

        if (targetSOs == null || targetSOs.Count == 0)
        {
            Debug.LogError("target SOs is not created or assigned to ClassName2SOMap yet");
            return;
        }

        for (int i = 0; i < targetSOs.Count; i++)
        {
            var targetSO = targetSOs[i];
            if (!(targetSO is IExcel2SO))
            {
                Debug.LogError("target SO class does not implement IExcel2SO yet");
                continue;
            }

            var parsedSheets = ParseExcel(xls);

            if (parsedSheets == null || parsedSheets.Count == 0)
            {
                continue;
            }

            ((IExcel2SO)targetSO).FromExcelToSO(parsedSheets);
        }

    }

    private List<List<ExcelGenericRow>> ParseExcel(Excel xls)
    {
        List<List<ExcelGenericRow>> parsedSheets = new();
        var excelSheets = xls.Tables;
        for (int i = 0; i < excelSheets.Count; i++)
        {
            var parsedSheet = ParseASheet(excelSheets[i]);
            if (parsedSheet == null)
            {
                Debug.LogError($"sheet at index [{i}] cannot be parsed!");
                continue;
            }

            parsedSheets.Add(parsedSheet);
        }

        return parsedSheets;
    }

    private List<ExcelGenericRow> ParseASheet(ExcelTable sheet)
    {
        int rowNum = sheet.NumberOfRows;
        int colNum = sheet.NumberOfColumns;

        List<string> colNames = new();
        for (int i = 1; i <= colNum; i++)
        {
            colNames.Add(sheet.GetCell(1, i).Value);
        }

        List<string> typeStrings = new();
        for (int i = 1; i <= colNum; i++)
        {
            typeStrings.Add(sheet.GetCell(2, i).Value);
        }

        List<Type> types = CreateTypeList(typeStrings);

        List<ExcelGenericRow>  rows = new();
        for (int row = 3; row <= rowNum; row++)
        {
            var soRow = new ExcelGenericRow();
            for (int col = 1; col <= colNum; col++)
            {
                var cell = sheet.GetCell(row, col);
                soRow.values.Add(ToSOCellValue(types[col - 1], colNames[col - 1], cell.Value));
            }

            rows.Add(soRow);
        }

        return rows;
    }

    private List<Type> CreateTypeList(List<string> types)
    {
        List<Type> typesList = new();
        for (int i = 0; i < types.Count; i++)
        {
            switch (types[i].ToLower().Trim())
            {
                case "int":
                    {
                        typesList.Add(typeof(int));
                        break;
                    }
                case "float":
                    {
                        typesList.Add(typeof(float));
                        break;
                    }
                default:
                    {
                        typesList.Add(typeof(string));
                        break;
                    }
            }
        }

        return typesList;
    }

    private RootValueCell ToSOCellValue(Type type, string colName, string value)
    {
        RootValueCell soCell;
        if (type.Equals(typeof(int)))
        {
            int.TryParse(value, out var intVal);
            soCell = new IntValueCell(intVal);
        }
        else if (type.Equals(typeof(float)))
        {
            float.TryParse(value, out var floatVal);
            soCell = new FloatValueCell(floatVal);
        }
        else // default is string
        {
            soCell = new StringValueCell(value);
        }

        soCell.name = colName;
        return soCell;
    }
}
