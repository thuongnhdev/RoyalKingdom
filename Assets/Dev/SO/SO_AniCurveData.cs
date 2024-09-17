using UnityEngine;

[CreateAssetMenu(menuName = "SO_Data/AniCurveData")]
public class SO_AniCurveData : ScriptableObject
{
    public UnityDictionary<int, AnimationCurve> AniCurveDataList_Int;
    public UnityDictionary<string, AnimationCurve> AniCurveDataList_Str;
}