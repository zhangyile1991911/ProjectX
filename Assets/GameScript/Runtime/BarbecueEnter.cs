using System;
using System.Collections;
using System.Collections.Generic;
using GameScript.CookPlay;
using PlasticPipe.Tube;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Random = UnityEngine.Random;

public class BarbecueEnter : MonoBehaviour
{
    public Button startBtn;
    public BarbecueModule module;
    public BarbecueRecipe recipe;
    private bool initYooAsset = false;
    private void Start()
    {
        StartCoroutine(GlobalFunctions.InitYooAssets(waitYooAssets));
        startBtn.OnClickAsObservable()
            .Where(_=>initYooAsset)
            .Subscribe(_ =>
        {
            module.StartBarbecue();
            startBtn.gameObject.SetActive(false);
        });
    }

    private void waitYooAssets(bool success)
    {
        initYooAsset = success;
        module.SetBarbecueFood(recipe);
        
        
            
        
    }
}
