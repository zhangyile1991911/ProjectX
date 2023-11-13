using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Serialization;

public class SteamEnter : MonoBehaviour
{
    private bool initYooAsset = false;

    public CookModule module;

    public SteamedRecipeDifficulty recipeDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        PickFoodAndTools foodAndTools = new PickFoodAndTools();
        foodAndTools.MenuId = 30001;
        UniModule.GetModule<UIManager>().OpenUI(UIEnum.SteamFoodWindow, (ui )=>
        {
            var sfw = ui as SteamFoodWindow;
            // sfw.XBtn_Start.OnClick.Subscribe((param) =>
            // {
            //     module.StartCook();
            // });
            sfw.ClickStart = () =>
            {
                module.StartCook();
            };
            module.Init(foodAndTools,recipeDifficulty);    
        }, null);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
