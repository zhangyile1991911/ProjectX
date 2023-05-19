using UnityEngine;

[CreateAssetMenu(fileName = "大排档",menuName="大排档/烧烤/菜谱")]
public class BarbecueRecipe : Recipe
{
    public float FanAddValue;//每按一次风扇增加值
    public float Attenuation;//每秒钟衰减值
    public float AddValueLimit;//上限
    public BarbecueSet Sets;//套餐组合菜单
}