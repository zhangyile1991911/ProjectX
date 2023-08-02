using System;
using System.Collections;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using YooAsset;

public class CookModule : MonoBehaviour
{
    protected List<AssetOperationHandle> _cacheHandles;
    protected CompositeDisposable _handler;
    protected PickFoodAndTools _recipe;
    //QTE动画相关
    protected List<Animation> _qteAnimations;
    protected List<qte_info> _tbQteInfos;
    //配置表
    protected MenuInfo _tbMenuInfo;
    //结果
    protected CookResult _result;
    

    public bool IsCooking { get; protected set; }
    public Action<CookResult> FinishCook { get; set; }

    public virtual void Init(PickFoodAndTools foodAndTools, RecipeDifficulty difficulty)
    {
        _cacheHandles?.Clear();
        _cacheHandles ??= new List<AssetOperationHandle>(20);
        
        _handler?.Clear();
        _handler ??= new CompositeDisposable(20);
        
        _result = new CookResult();
        _result.menuId = foodAndTools.MenuId;
        _result.Tags = new HashSet<flavorTag>(5);
        _result.QTEResult = new Dictionary<int, bool>();
    }

    public virtual void StartCook()
    {
        
    }


    public virtual async void LoadQTEConfig(List<qte_info> selectQTE, Transform qteRoot)
    {
        var groupId = DataProviderModule.Instance.GetQTEGroupId();
        Debug.Log($"loadQTEConfig = {groupId}");
        _tbQteInfos = selectQTE;
        // _tbQteInfos.Clear();
        //
        // var tmpTb = DataProviderModule.Instance.GetQTEGroupInfo(groupId);
        // for (int i = 0; i < tmpTb.Count; i++)
        // {
        //     if (qtes.Contains(tmpTb[i].QteId))
        //     {
        //         _tbQteInfos.Add(tmpTb[i]);
        //     }
        // }

        //QTE动画
        _qteAnimations ??= new List<Animation>(5);
        _qteAnimations?.Clear();

        foreach (var one in selectQTE)
        {
            var qteTB = DataProviderModule.Instance.GetQTEInfo(one.QteId);
            var loadHandle = YooAssets.LoadAssetAsync<GameObject>(qteTB.AnimResPath);
            _cacheHandles.Add(loadHandle);

            await loadHandle.ToUniTask();

            var prefb = loadHandle.AssetObject as GameObject;
            var go = Instantiate(prefb, qteRoot);
            go.gameObject.SetActive(false);
            var ani = go.GetComponent<Animation>();
            ani.clip.legacy = true;
            _qteAnimations.Add(ani);
            _result.QTEResult.Add(one.QteId, false);
        }
    }

    public virtual void UnloadRes()
    {
        //清理qte
        _tbQteInfos?.Clear();
        for (int i = 0; i < _qteAnimations.Count; i++)
        {
            var one = _qteAnimations[i];
            Destroy(one.gameObject);
        }

        _qteAnimations?.Clear();

        //清理加载资源
        foreach (var handler in _cacheHandles)
        {
            handler.Release();
        }
        
        _handler?.Clear();
    }

}