using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static float Remap(float inMin, float inMax, float outMin, float outMax, float input)
    {
        return outMin + (input - inMin) * (outMax - outMin) / (inMax - inMin);
    }

    /// <summary>
    /// return current Progress percent and remaining time (seconds)
    /// </summary>
    public static (float, float) CurrentRatioAndRemain(float current, float destination, float speed)
    {
        if (destination < current)
        {
            return (1f, 0f);
        }

        (float current, float remain) result;
        result.current = current / destination;

        if (speed <= float.Epsilon)
        {
            result.remain = float.PositiveInfinity;
            return result;
        }
        result.remain = (destination - current) / speed;

        return result;
    }

    public static int RandomChooseFrom(int[] ratios)
    {
        int[] ratioChunk = new int[ratios.Length];

        int randomTo = 0;
        for (int i = 0; i < ratios.Length; i++)
        {
            randomTo += ratios[i];
            ratioChunk[i] = ratios[i];
            int j = i - 1;
            if (0 <= j)
            {
                ratioChunk[i] += ratioChunk[j];
            }
        }

        int roll = Random.Range(0, randomTo);
        for (int i = 0; i < ratioChunk.Length; i++)
        {
            if (roll < ratioChunk[i])
            {
                return i;
            }
        }

        return -1;
    }

    public static int RandomChooseFrom(float[] ratios)
    {
        float randomTo = 0f;
        float[] ratioChunk = new float[ratios.Length];
        for (int i = 0; i < ratios.Length; i++)
        {
            randomTo += ratios[i];
            ratioChunk[i] = ratios[i];
            int j = i - 1;
            if (0 <= j)
            {
                ratioChunk[i] += ratioChunk[j];
            }
        }

        float roll = Random.Range(0f, randomTo);
        for (int i = 0; i < ratioChunk.Length; i++)
        {
            if (roll <= ratioChunk[i])
            {
                return i;
            }
        }

        return -1;
    }
}
