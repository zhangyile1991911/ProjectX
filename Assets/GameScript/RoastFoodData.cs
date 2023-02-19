using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "大排档",menuName="大排档/烧烤/烤物")]
public class RoastFoodData : ScriptableObject
{
    public Color CookingProgressColor;
    public Color OverCookedProgressColor;
    
    public float CookedHeatCapacity;
    public float MediumHeatCapactiy;
    public float SpecificHeatCapacity;

    
}
