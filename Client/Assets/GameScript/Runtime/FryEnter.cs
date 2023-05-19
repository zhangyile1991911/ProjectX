using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using YooAsset;

public class FryEnter : MonoBehaviour
{
    public Button startButton;
    public FryModule module;

    public  FriedFoodRecipe recipe;

    private bool initYooAsset = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GlobalFunctions.InitYooAssets(waitYooAssets));
        
        startButton.OnClickAsObservable()
            .Where(_=>initYooAsset)
            .Subscribe(_ =>
        {
            module.StartFry();
            startButton.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void waitYooAssets(bool success)
    {
        initYooAsset = success;
        module.SetFryRecipe(recipe);
    }
}
