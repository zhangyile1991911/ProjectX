using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "大排档",menuName="大排档/炒菜/QTE")]
public class FriedFoodQTE : ScriptableObject
{
    public enum QTEAction
    {
        Salt,Oil,Vinegar
    }
    public float progress;
    public QTEAction action;
    public KeyCode pressKey;
    public string desc;
}