using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GlobalFunctions.InitYooAssets(WaitInit));
    }

    void WaitInit(bool success)
    {
        UIManager.Instance.OpenUI(UIEnum.RestaurantWindow,null,null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
