using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemProductFormulaList", menuName = "Uniflow/Building/Production/ProductFormulaList")]
public class ProductFormulaList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<ProductFormula> _itemProductFormulasList;
    private Dictionary<int, ProductFormula> _formulaDict = new();
    private Dictionary<int, ProductFormula> FormulaDict
    {
        get
        {
            if (_itemProductFormulasList.Count != _formulaDict.Count)
            {
                _formulaDict.Clear();

                for (int i = 0; i < _itemProductFormulasList.Count; i++)
                {
                    var formula = _itemProductFormulasList[i];
                    _formulaDict[formula.productId] = formula;
                }
            }

            return _formulaDict;
        }
    }

    public void Init(Fbs.ApiGetLowData data)
    {
        int itemDataCount = data.ItemTemplateLength;
        if (itemDataCount == 0)
        {
            return;
        }

        _itemProductFormulasList.Clear();
        _formulaDict.Clear();

        for (int i = 0; i < itemDataCount; i++)
        {
            var itemData = data.ItemTemplate(i).Value;
            List<ResourcePoco> material = new();
            ResourcePoco mat1 = new() { itemId = itemData.ReqItemKey1, itemCount = itemData.ReqItemValue1 };
            if (!ResourcePoco.IsZero(mat1))
            {
                material.Add(mat1);
            }
            ResourcePoco mat2 = new() { itemId = itemData.ReqItemKey2, itemCount = itemData.ReqItemValue2 };
            if (!ResourcePoco.IsZero(mat2))
            {
                material.Add(mat2);
            }
            ResourcePoco mat3 = new() { itemId = itemData.ReqItemKey3, itemCount = itemData.ReqItemValue3 };
            if (!ResourcePoco.IsZero(mat3))
            {
                material.Add(mat3);
            }
            ResourcePoco mat4 = new() { itemId = itemData.ReqItemKey4, itemCount = itemData.ReqItemValue4 };
            if (!ResourcePoco.IsZero(mat4))
            {
                material.Add(mat4);
            }

            List<int> terrains = new();
            int terrainCount = itemData.TerrainLength;
            for (int j = 0; j < terrainCount; j++)
            {
                int terrain = itemData.Terrain(j);
                if (terrain <= 0 || terrains.Contains(terrain))
                {
                    continue;
                }
                terrains.Add(terrain);
            }

            _itemProductFormulasList.Add(new()
            {
                productId = itemData.IdItemTemplate,
                terrains = terrains,
                culture = itemData.Culture,
                religion = itemData.Relition,
                producedByBuilding = itemData.CommandBuilding,
                productCount = 1, // TODO RK update this logic if more formula ratios come
                timeCost = itemData.Workload,
                material = material
            });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public ProductFormula GetFormula(int productId)
    {
        FormulaDict.TryGetValue(productId, out var formular);
        if (formular == null)
        {
            Debug.Log($"<color=green> No formula for item [{productId}] </color>");
        }

        return formular;
    }

    public int GetProducingBuilding(int productId)
    {
        var formula = GetFormula(productId);
        if (formula == null)
        {
            return 0;
        }

        return formula.producedByBuilding;
    }

    public float GetTimeCost(int productId)
    {
        var formula = GetFormula(productId);
        if (formula == null)
        {
            return 0f;
        }

        return formula.timeCost;
    }

    public (List<int>, int, int) GetLandCondition(int productId)
    {
        var product = GetFormula(productId);
        if (product == null)
        {
            return (null, 0, 0);
        }

        (List<int> terrains, int religion, int culture) condition;
        condition.terrains = product.terrains;
        condition.religion = product.religion;
        condition.culture = product.culture;

        return condition;
    }

    public List<ResourcePoco> GetResourceCost(int productId)
    {
        var formula = GetFormula(productId);
        if (formula == null)
        {
            return null;
        }

        return formula.material;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        if (parsedSheets.Count == 0)
        {
            return;
        }

        var sheet = parsedSheets[1];
        if (sheet.Count == 0)
        {
            return;
        }

        _itemProductFormulasList.Clear();
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("Key", out int productId);
            row.GetValue("Terrain", out string terrain);
            row.GetValue("Religion", out int religion);
            row.GetValue("Culture", out int culture);
            row.GetValue("CommandBuilding", out int producedByBuilding);
            row.GetValue("Workload", out float timeCost);

            List<ResourcePoco> cost = new();

            row.GetValue("Req_ItemKey_1", out int mat1Id);
            row.GetValue("Req_ItemValue_1", out int mat1Count);
            cost.Add(new ResourcePoco { itemId = mat1Id, itemCount = mat1Count });

            row.GetValue("Req_ItemKey_2", out int mat2Id);
            row.GetValue("Req_ItemValue_2", out int mat2Count);
            cost.Add(new ResourcePoco { itemId = mat2Id, itemCount = mat2Count });

            row.GetValue("Req_ItemKey_3", out int mat3Id);
            row.GetValue("Req_ItemValue_3", out int mat3Count);
            cost.Add(new ResourcePoco { itemId = mat3Id, itemCount = mat3Count });

            row.GetValue("Req_ItemKey_4", out int mat4Id);
            row.GetValue("Req_ItemValue_4", out int mat4Count);
            cost.Add(new ResourcePoco { itemId = mat4Id, itemCount = mat4Count });

            var formula = new ProductFormula();
            formula.productId = productId;
            formula.productCount = 1;
            formula.religion = religion;
            formula.culture = culture;
            string[] terrains = terrain.Split(',');
            for (int j = 0; j < terrains.Length; j++)
            {
                if (!int.TryParse(terrains[j], out int terrainId))
                {
                    continue;
                }
                formula.terrains.Add(terrainId);
            }
            formula.producedByBuilding = producedByBuilding;
            formula.timeCost = timeCost;

            for (int j = 0; j < cost.Count; j++)
            {
                if (ResourcePoco.IsZero(cost[j]))
                {
                    continue;
                }
                formula.material.Add(cost[j]);
            }

            _itemProductFormulasList.Add(formula);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class ProductFormula
{
    public int productId;
    public int productCount;
    public int producedByBuilding;
    public List<int> terrains = new();
    public int religion;
    public int culture;
    public List<ResourcePoco> material = new();
    public float timeCost;
}
