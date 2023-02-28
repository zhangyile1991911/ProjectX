using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "大排档",menuName="大排档/菜谱")]
public class Recipe : ScriptableObject
{
    public Vector2 temperatureArea;
    public float maxTemperature;
    public float finishValue;
    public float addValue;

}
