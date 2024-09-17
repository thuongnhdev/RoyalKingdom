using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WorldMapStrategyKit;

public class WorldMapDataExporter : MonoBehaviour
{
    public void ExportLandsData()
    {
        WMSK map = WMSK.instance;

        StringBuilder sb = new();
        Province[] provinces = map.provinces;
        for (int i = 0; i < provinces.Length; i++)
        {
            var p = provinces[i];
            sb.Append(p.uniqueId).Append(',').AppendLine(p.name);
        }

        TextFileIO.WriteText("Lands.csv", sb.ToString());
    }
}
