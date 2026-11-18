using UnityEngine;
// Gives access to Action
using System;

public static class Score
{
    //var used to store score for whole game ONLY one instance can exist 
    private static int points;

    // Announces the new score whenever it changes; anyone can subscribe
    public static event Action<int> OnPointsChanged;

    //method to reset points called from game manager
    public static void ResetPoints()
    {
        points = 0;
        // Tell every subscriber the score is now 0
        OnPointsChanged?.Invoke(points);
    }

    //method to add points can be called in the following format Score.AddPoints(100);
    public static void AddPoints(int amount)
    {
        points += amount;
        Debug.Log("points added");
        // Tell every subscriber the new score
        OnPointsChanged?.Invoke(points);
    }
}