using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "大排档",menuName="手机/APP/打飞机")]
public class AirplaneDifficulty : ScriptableObject
{
    public float create_enemy_interval = 2.0f;
    public float enemy_speed = 0.7f;
    public float enemy_shot_interval = 2.5f;
    public int enemy_score = 1;
    public int enemy_hp = 1;
}
