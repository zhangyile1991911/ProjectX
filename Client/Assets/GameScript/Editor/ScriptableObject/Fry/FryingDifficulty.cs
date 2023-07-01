using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "大排档",menuName="大排档/炒菜/菜谱")]
public class FryingDifficulty : RecipeDifficulty
{
    public Vector2 temperatureArea;
    public float maxTemperature;
    public float finishValue;
    public float addValue;
}