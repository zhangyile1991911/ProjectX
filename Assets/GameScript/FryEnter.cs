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

    IEnumerator InitYooAssets()
    {
        //todo 这部分初始化 之后移动一个专门地方做
        YooAssets.Initialize();
        var package = YooAssets.CreateAssetsPackage("DefaultPackage");
        YooAssets.SetDefaultAssetsPackage(package);
        var initParameters = new EditorSimulateModeParameters();
        initParameters.SimulatePatchManifestPath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        yield return package.InitializeAsync(initParameters);
        Debug.Log($"YooAssets初始化完成");
        
        module.SetFryRecipe(recipe);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitYooAssets());
        
        startButton.OnClickAsObservable().Subscribe(_ =>
        {
            module.StartFry();
            startButton.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
