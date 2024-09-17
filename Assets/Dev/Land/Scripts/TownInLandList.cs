using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TownsInLand", menuName = "Uniflow/World/TownsInLand")]
public class TownInLandList : ScriptableObject
{
    [SerializeField]
    private List<TownInLandInfo> _towns;
    public List<TownInLandInfo> Towns => _towns;

    public void Init()
    {
        //_towns.Clear();
        //for (int i = 0; i < 20; i++)
        //{
        //    _towns.Add(new()
        //    {
        //        id = (i + 1),
        //        townName = $"Dummy {i + 1}",
        //        population = Random.Range(1000, 10000),
        //        reputation = Random.Range(100, 1000),
        //        prosperity = Random.Range(100, 1000),
        //        totalTroops = Random.Range(1000, 10000),
        //        gatheredResource = Random.Range(10000, 100000),
        //        consecutiveLogin = Random.Range(1, 1000),
        //        login = Random.Range(100, 1000),
        //        builtBuildings = Random.Range(10, 100)
        //    }); ;
        //}

        //UnityEditor.EditorUtility.SetDirty(this);
    }

    private void OnEnable()
    {
        Init();
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
