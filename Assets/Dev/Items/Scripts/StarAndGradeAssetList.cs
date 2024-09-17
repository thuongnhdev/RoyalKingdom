using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarAndGradeAssets", menuName = "Uniflow/Resource/StarAndGradeAssets")]
public class StarAndGradeAssetList : ScriptableObject
{
    [SerializeField]
    private List<Sprite> _stars;
    [SerializeField]
    private List<Sprite> _starBgs;
    [SerializeField]
    private List<Sprite> _grades;
    [SerializeField]
    private List<Sprite> _gradesWithSlot;
    [SerializeField]
    private Sprite _groupGrade;

    public Sprite GetStarIcon(int star)
    {
        star = Mathf.Clamp(star, 0, _stars.Count - 1);
        return _stars[star];
    }

    public Sprite GetStarBG(ItemGrade grade)
    {
        int gradeInt = (int)grade;
        gradeInt = Mathf.Clamp(gradeInt, 0, _starBgs.Count - 1);
        return _starBgs[gradeInt];
    }

    public Sprite GetItemGroupGrade()
    {
        return _groupGrade;
    }

    public Sprite GetGradeIcon(ItemGrade grade)
    {
        int gradeInt = (int)grade;
        gradeInt = Mathf.Clamp(gradeInt, 0, _grades.Count - 1);
        return _grades[gradeInt];
    }

    public Sprite GetGradeWithSlotIcon(ItemGrade grade)
    {
        int gradeInt = (int)grade;
        gradeInt = Mathf.Clamp(gradeInt, 0, _gradesWithSlot.Count - 1);
        return _gradesWithSlot[gradeInt];
    }
}
