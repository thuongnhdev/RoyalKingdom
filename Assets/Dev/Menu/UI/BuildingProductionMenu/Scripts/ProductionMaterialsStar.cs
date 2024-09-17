using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductionMaterialsStars", menuName = "Uniflow/Resource/ProductionMaterialsStars")]
public class ProductionMaterialsStar : ScriptableObject
{
    public List<ResourcePoco> materials = new();
    public List<ItemWithStar> materialsStars = new();

    public void Reset()
    {
        if (materials.Count != 0)
        {
            materials = new();
        }
        if (materialsStars.Count != 0)
        {
            materialsStars = new();
        }
    }

    public int[] GetStars(int itemId)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            var matStars = materialsStars[i];
            if (matStars.itemId == itemId)
            {
                return matStars.stars;
            }
        }

        return new int[0];
    }

    public int GetStar(int itemId, int star)
    {
        int[] stars = GetStars(itemId);
        if (stars.Length == 0)
        {
            return 0;
        }

        return stars[star - 1];
    }

    public void UpdateMaterial(ResourcePoco material, int star, int starCount)
    {
        if (star <= 0)
        { 
            return;
        }

        star--;
        if (!ContainsMaterial(material.itemId))
        {
            materials.Add(material);

            ItemWithStar iws = new();

            iws.itemId = material.itemId;
            iws.stars = new int[] { 0, 0, 0, 0 };
            int[] stars = iws.stars;

            stars[star] = starCount;
            if (star != 0)
            {
                stars[0] = material.itemCount - stars[star];
            }

            materialsStars.Add(iws);
            return;
        }

        UpdateStars(material, star, starCount);
    }

    private void UpdateStars(ResourcePoco material, int star, int starCount)
    {
        var matStar = FindMaterialStarOption(material.itemId);
        if (matStar == null)
        {
            return;
        }

        int totalStarBeforeChange = 0;
        int[] stars = matStar.stars;
        for (int i = 1; i < stars.Length; i++) // skip star0;
        {
            totalStarBeforeChange += stars[i];
        }

        int beforeChange = stars[star];
        if (totalStarBeforeChange == material.itemCount && beforeChange < starCount)
        {
            return;
        }

        int maxStarIncrease = material.itemCount - totalStarBeforeChange;
        stars[star] = Mathf.Clamp(starCount, 0, maxStarIncrease + beforeChange);
        stars[0] = material.itemCount - totalStarBeforeChange - (stars[star] - beforeChange);
    }

    private bool ContainsMaterial(int itemId)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].itemId == itemId)
            {
                return true;
            }
        }

        return false;
    }

    private ItemWithStar FindMaterialStarOption(int itemId)
    {
        for (int i = 0; i < materialsStars.Count; i++)
        {
            var matStars = materialsStars[i];
            if (matStars.itemId == itemId)
            {
                return matStars;
            }
        }

        return null;
    }
}
