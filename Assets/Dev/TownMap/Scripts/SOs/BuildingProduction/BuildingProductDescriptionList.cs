using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingProductionDescriptionList", menuName = "Uniflow/Building/Production/ProductionDescription")]
public class BuildingProductDescriptionList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<BuildingProductionDescription> _productionDescriptionList;
    private Dictionary<int, BuildingProductionDescription> _prodDesDict = new();
    private Dictionary<int, BuildingProductionDescription> ProdDesDict
    {
        get
        {
            if (_productionDescriptionList.Count != _prodDesDict.Count)
            {
                _prodDesDict.Clear();

                for (int i = 0; i < _productionDescriptionList.Count; i++)
                {
                    var prodDes = _productionDescriptionList[i];
                    _prodDesDict[prodDes.buildingId] = prodDes;
                }
            }

            return _prodDesDict;
        }
    }

    public void Init(Fbs.ApiGetLowData data)
    {
        int itemDataCount = data.ItemTemplateLength;
        if (itemDataCount == 0)
        {
            return;
        }
        _productionDescriptionList.Clear();
        _prodDesDict.Clear();
        for (int i = 0; i < itemDataCount; i++)
        {
            var itemData = data.ItemTemplate(i).Value;
            int commandBuilding = itemData.CommandBuilding;
            if (commandBuilding == 0)
            {
                continue;
            }

            _prodDesDict.TryGetValue(commandBuilding, out var prodDes);
            if (prodDes == null)
            {
                prodDes = new BuildingProductionDescription
                {
                    buildingId = itemData.CommandBuilding,
                    productionType = ProductionType.Resource, //TODO RK update this logic when product types come
                    productsId = new()
                };
                _productionDescriptionList.Add(prodDes);
                _prodDesDict.Add(prodDes.buildingId, prodDes);
            }
            prodDes.productsId.Add(itemData.IdItemTemplate);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public BuildingProductionDescription GetProductionDescription(int buildingId)
    {
        ProdDesDict.TryGetValue(buildingId, out var prodDes);
        if (prodDes == null)
        {
            Debug.Log($"building with Id [{buildingId}] has no production");
        }

        return prodDes;
    }

    public List<int> GetBuildingProducts(int buildingId)
    {
        var production = GetProductionDescription(buildingId);
        if (production == null)
        {
            return null;
        }

        return production.productsId;
    }

    public bool HasProduct(int buildingId)
    {
        var products = GetBuildingProducts(buildingId);
        if (products == null || products.Count == 0)
        {
            return false;
        }

        return true;
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

        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("Key", out int productId);
            row.GetValue("CommandBuilding", out int buildingId);

            _prodDesDict.TryGetValue(buildingId, out var prodDes);
            if (prodDes == null)
            {
                prodDes = new BuildingProductionDescription();
                prodDes.buildingId = buildingId;
                prodDes.productionType = ProductionType.Resource;

                _productionDescriptionList.Add(prodDes);
                _prodDesDict[buildingId] = prodDes;
            }

            if (prodDes.productsId.Contains(productId))
            {
                continue;
            }

            prodDes.productsId.Add(productId);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class BuildingProductionDescription
{
    public int buildingId;
    public ProductionType productionType;
    public List<int> productsId = new();
}

public enum ProductionType
{
    None = 0,
    Resource = 1,
    Military = 2,
    Technology = 3
}
