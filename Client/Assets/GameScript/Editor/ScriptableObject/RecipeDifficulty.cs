using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public enum DifficultyLevel:int
{
    EASY,
    NORMAL,
    HARD,
}
public class RecipeDifficulty : ScriptableObject
{
    public DifficultyLevel Level;
    public float duration;
   
}
