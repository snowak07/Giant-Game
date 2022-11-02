using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    private static float score = 0;

    public static void Add(float scoreToAdd = 1)
    {
        score += scoreToAdd;
    }

    public static float Get()
    {
        return score;
    }

    public static void Reset()
    {
        score = 0;
    }
}
