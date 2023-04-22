using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEnter : MonoBehaviour
{
    private UIManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = UniModule.GetModule<UIManager>();
        _uiManager.OpenUI(UIEnum.RestaurantWindow,null,null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
