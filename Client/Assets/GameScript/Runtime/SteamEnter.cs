using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class SteamEnter : MonoBehaviour
{
    public Button startBtn;
    private bool initYooAsset = false;

    public SteamedModule module;

    public SteamedRecipe recipe;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GlobalFunctions.InitYooAssets(waitYooAssets));
        startBtn.OnClickAsObservable()
            .Where(_=>initYooAsset)
            .Subscribe(_ =>
            {
                module.StartGame();
                startBtn.gameObject.SetActive(false);
            });
    }

    private void waitYooAssets(bool success)
    {
        initYooAsset = success;
        module.Init();
        module.SetRecipe(recipe);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
