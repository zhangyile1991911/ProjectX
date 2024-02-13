using System;
using System.Collections.Generic;
using System.Linq;
using cfg.food;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Pool;
using YooAsset;
using Random = UnityEngine.Random;

public class FryModule : CookModule
{
    public bool IsDebugMode = false;
    // public Material sliderBgMat;
    public PanSimulator pan;
    public Transform firePointTransform;

    [InspectorName("距离热度比例")]
    public AnimationCurve heatCurve;

    [InspectorName("移动速度降温比例")]
    public AnimationCurve lowerCurve;

    private FryingDifficulty _currentRecipeDifficulty;
    
    
    private ReactiveProperty<float> _curProgress;
    private ReactiveProperty<float> _curTemperature;
    
    private Subject<bool> _finishTopic;

    private List<oil_steaming_effect> oil_steaming_pools;

    private List<oil_jump_effect> oil_jump_pools;
    // private Subject<bool> timeUpTopic;
    // private List<bool> qteRecords;
    // private List<int> qteKeys;
        
    // private async void loadQTEConfig(HashSet<int> qtes)
    // {
    //     var groupId = DataProviderModule.Instance.GetQTEGroupId();
    //     Debug.Log($"loadQTEConfig = {groupId}");   
    //     _tbQteInfos ??= new List<qte_info>(10);
    //     _tbQteInfos.Clear();
    //     
    //     var tmpTb = DataProviderModule.Instance.GetQTEGroupInfo(groupId);
    //     for (int i = 0; i < tmpTb.Count; i++)
    //     {
    //         if (qtes.Contains(tmpTb[i].QteId))
    //         {
    //             _tbQteInfos.Add(tmpTb[i]);
    //         }
    //     }
    //     
    //     _uiWindow.LoadQTEConfigTips(_tbQteInfos);
    //     
    //     //QTE动画
    //     _qteAnimations?.Clear();
    //     _qteAnimations ??= new List<Animation>(5);
    //
    //     foreach (var qteId in qtes)
    //     {
    //         var qteTB = DataProviderModule.Instance.GetQTEInfo(qteId);
    //         var loadHandle = YooAssets.LoadAssetAsync<GameObject>(qteTB.AnimResPath);
    //         _cacheHandles.Add(loadHandle);
    //         
    //         await loadHandle.ToUniTask();
    //         
    //         var prefb = loadHandle.AssetObject as GameObject;
    //         var go = Instantiate(prefb,pan.animNode);
    //         go.gameObject.SetActive(false);
    //         var ani = go.GetComponent<Animation>();
    //         ani.clip.legacy = true;
    //         _qteAnimations.Add(ani);
    //         _result.QTEResult.Add(qteId,false);
    //     }
    //
    // }

    private void loadRawFood(List<ItemTableData> foods,string resRootPath)
    {
        
        //炒菜原材料
        foreach (var food in foods)
        {
            // var itemTB = DataProviderModule.Instance.GetItemBaseInfo(food.Id);
            var handle = YooAssets.LoadAssetSync<GameObject>($"{resRootPath}/{food.Id}.prefab");
            _cacheHandles.Add(handle);
            var num = 5;
            for (int i = 0; i < num; i++)
            {
                var go = Instantiate(handle.AssetObject as GameObject,pan.transform);
                Vector3 pos = Random.insideUnitCircle*3f;
                pos.z = 0f;
                go.transform.localPosition = pos;
                var fs = go.GetComponent<FoodSimulator>();
                var materialTb = DataProviderModule.Instance.GetFoodMaterial(food.Id);
                fs.CollisionLayer = materialTb.CollisionLayer;
                pan.AddFood(fs);
            }
        }
    }

    private FryingFoodWindow _uiWindow;
    
    void SetFryRecipe(PickFoodAndTools recipe)
    {
        _tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(recipe.MenuId);
        if (_tbMenuInfo == null)
        {
            Debug.LogError($"recipe.MenuId {recipe.MenuId} == null");
        }
        
        loadRawFood(recipe.CookFoods,_tbMenuInfo.SceneResPath);
        LoadQTEConfig(recipe.QTEConfigs,pan.animNode);

        _uiWindow = UIManager.Instance.Get(UIEnum.FryingFoodWindow) as FryingFoodWindow;
    }

    private async void load_oil_steaming_effect()
    {
        if (oil_steaming_pools.Count > 0) return;
        for (int i = 0;i < 10;i++)
        {
            var handle = YooAssets.LoadAssetAsync<GameObject>("Assets/GameRes/Prefabs/AllFood/oil_steaming.prefab");
            await handle.ToUniTask(this);
            var go = Instantiate(handle.AssetObject as GameObject,transform);
            go.SetActive(false);
            var effect = go.GetComponent<oil_steaming_effect>();
            oil_steaming_pools.Add(effect);
        }
        Debug.Log($"load_oil_steaming_effect oil_steaming_pools.Count = {oil_steaming_pools.Count}");
    }

    public void StartFry()
    {
        _curTemperature.Value = 0;
        _curProgress.Value = 0;
        IsCooking = true;

        var min = _currentRecipeDifficulty.temperatureArea.x * _currentRecipeDifficulty.maxTemperature;
        var max = _currentRecipeDifficulty.temperatureArea.y * _currentRecipeDifficulty.maxTemperature;
        // Debug.Log($"min = {min} max = {max} _currentRecipeDifficulty.temperatureArea = {_currentRecipeDifficulty.temperatureArea} _currentRecipeDifficulty.maxTemperature = {_currentRecipeDifficulty.maxTemperature}");
        // Debug.Log($"_handler = {_handler.Count} {_handler.GetHashCode()}");
        this.UpdateAsObservable()
            .Where(_=>IsCooking&&(_curTemperature.Value >= min && _curTemperature.Value <= max))
            .Subscribe(HeatFood).AddTo(_handler);


        var TimeCounter = Observable.
            Timer(TimeSpan.FromSeconds(_currentRecipeDifficulty.duration))
            .Select(_ => false);

        _uiWindow.SetProgressListener(_curProgress);
        _uiWindow.SetTemperatureListener(_curTemperature,IsDebugMode);

        
        Observable.Amb(
            _finishTopic,
            TimeCounter
            ).Subscribe(GameOver).AddTo(_handler);
        
        this.UpdateAsObservable()
            .Where(_ => IsCooking && Input.anyKeyDown)
            .Subscribe(ListenQteInput)
            .AddTo(_handler);
        
        pan.GameStart(_handler);

        this.UpdateAsObservable()
            .Where(_ => IsCooking)
            .Subscribe(CalculateHeat)
            .AddTo(_handler);
    }

    // private void LateUpdate()
    // {
    //     Debug.Log("Update++++++");
    // }
    private float oil_steaming_interval;
    private void show_oil_steaming()
    {
        if (_curTemperature.Value < _currentRecipeDifficulty.maxTemperature*0.5f)
        {
            return;
        }
        if (oil_steaming_pools.Count <= 0)
        {
            return;
        }

        oil_steaming_interval -= Time.deltaTime;
        if (oil_steaming_interval > 0)
        {
            return;
        }

        oil_steaming_interval = 0.8f;
        var obj = oil_steaming_pools.Last();
        oil_steaming_pools.RemoveAt(oil_steaming_pools.Count - 1);
        obj.Reset();
        obj.transform.position = pan.transform.position;
        obj.PlayEffect(oil_steaming_pools);
        // Debug.Log($"show_oil_steaming remain = {oil_steaming_pools.Count}");
    }
    private void CalculateHeat(Unit param)
    {
        show_oil_steaming();
        show_oil_jump();
        // Debug.Log("=========Start CalculateHeat=========");
        var distance = Vector2.Distance(pan.transform.position, firePointTransform.position);
        var add = heatCurve.Evaluate(distance)*Time.deltaTime;
        // Debug.Log($"Calculateheat deltaTime = {Time.deltaTime}");
        // Debug.Log($"distance = {distance} add = {add}");
        // var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        
        // Debug.Log($"velocity = {pan.velocity} add = {add} sub = {sub}");
        
        var tmp = _curTemperature.Value;
        tmp += (add+sub) * _currentRecipeDifficulty.maxTemperature;
        // Debug.Log($"_curTemperature.Value = {_curTemperature.Value}");      
        // Debug.Log($"distance = {distance} result = {add+sub} ");
        // Debug.Log($"mul = {(add+sub)*_currentRecipeDifficulty.maxTemperature}");
        
        // Debug.Log($"_curTemperature.Value = {_curTemperature.Value}");
        // tmp *= _currentRecipeDifficulty.maxTemperature;
        
        // var result = add + sub;
        // result = result > 0 ? result : 0;
        // var tmp = _curTemperature.Value;
        // tmp += result;
        // Debug.Log($"before temperature = {_curTemperature}");
        _curTemperature.Value = Mathf.Clamp(tmp, 0, _currentRecipeDifficulty.maxTemperature);
        // Debug.Log($"after temperature = {_curTemperature}");
        // Debug.Log("=========Finish CalculateHeat=========");
    }
    
    private void HeatFood(Unit param)
    {
        var temp = _curProgress.Value;
        temp += _currentRecipeDifficulty.addValue * Time.deltaTime;
        temp = Mathf.Clamp(temp, 0, _currentRecipeDifficulty.finishValue);
        _curProgress.Value = temp;
        // Debug.Log($"HeatFood add value = {_currentRecipeDifficulty.addValue} progress {_curProgress.Value}");
        //判断是否结束
        if (_curProgress.Value >= _currentRecipeDifficulty.finishValue)
        {
            _finishTopic.OnNext(true);
        }

        ListenProgress(_curProgress.Value);
    }

    private void GameOver(bool param)
    {
        Debug.Log($"GameOver IsSuccess = {param}");
        IsCooking = false;

        _result.MenuId = _tbMenuInfo.Id;
        _result.Score = _curProgress.Value;
        _result.MaxScore = _currentRecipeDifficulty.finishValue;
        //计算标签
        for (int i = 0;i < _recipe.CookFoods.Count;i++)
        {
            var tb = DataProviderModule.Instance.GetFoodBaseInfo(_recipe.CookFoods[i].Id);
            foreach (var one in tb.Tag)
            {
                _result.Tags.Add(one);    
            }

            foreach (var one in tb.OppositeTag)
            {
                _result.Tags.Add(one);    
            }
        }
        _result.create_timestamp = Clocker.Instance.NowDateTime.Timestamp;
        
        _curTemperature.Value = 0;
        _curProgress.Value = 0;
        _currentRecipeDifficulty = null;

        if (!IsDebugMode)
        {
            //消耗时间
            var clocker = UniModule.GetModule<Clocker>();
            clocker.AddMinute(_tbMenuInfo.CostTime);    
        }
        pan.transform.localPosition = Vector3.zero;
        
        FinishCook?.Invoke(_result);
        UnloadRes();
    }

    public override void UnloadRes()
    {
        base.UnloadRes();
        pan.RemoveAllFood();
        
        // foreach (var oil in oil_steaming_pools)
        // {
        //     Destroy(oil.gameObject);
        // }
        // oil_steaming_pools.Clear();
    }
    
    private void ListenProgress(float param)
    { 
        for (int i = 0;i < _tbQteInfos.Count;i++)
        {
            var qteInfo = _tbQteInfos[i];  
            var percent = _curProgress.Value / _currentRecipeDifficulty.finishValue;
            if (percent >= qteInfo.StartArea && percent < qteInfo.EndArea)
            {
                _uiWindow.ShowQTETip(qteInfo.QteId);
                break;
            }
        }
    }

    private void ListenQteInput(Unit param)
    {
        for (int i = 0;i < _tbQteInfos.Count;i++)
        {
            var one = _tbQteInfos[i];
            var tbQte = DataProviderModule.Instance.GetQTEInfo(one.QteId);
            var percent = _curProgress.Value / _currentRecipeDifficulty.finishValue;
            // Debug.Log($" ListenQteInput {percent} one.StartArea = {one.StartArea} one.EndArea = {one.EndArea}");
            if (percent >= one.StartArea && percent <= one.EndArea)
            {
                var keyDown = Input.GetKeyDown((KeyCode)tbQte.KeyCode);
                var clicked = _result.QTEResult[one.QteId]; 
                // Debug.Log($" ListenQteInput {percent} keyDown = {keyDown} clicked = {clicked}");
                if (keyDown&&clicked==false)
                {
                    // Debug.Log($"ListenQteInput 播放QTE动画");
                    _qteAnimations[i].gameObject.SetActive(true);
                    _qteAnimations[i].Play();
                    _result.QTEResult[one.QteId] = true;
                    _result.Tags.Add(tbQte.Tag);
                    break; 
                }
            }
        }
    }

    public override void Init(PickFoodAndTools foodAndTools,RecipeDifficulty difficulty)
    {
        base.Init(foodAndTools,difficulty);

        pan.Init();

        IsCooking = false;
        
        _finishTopic ??= new Subject<bool>();
        
        _qteAnimations?.Clear();
        _qteAnimations ??= new List<Animation>(5);

        _curProgress ??= new ReactiveProperty<float>();
        _curTemperature ??= new ReactiveProperty<float>();
        

        // _result = new CookResult();
        // _result.Tags = new HashSet<flavorTag>(5);
        // _result.QTEResult = new Dictionary<int, bool>();
        
        _currentRecipeDifficulty = difficulty as FryingDifficulty;
        pan.transform.localPosition = Vector3.zero;
        
        oil_steaming_pools ??= new List<oil_steaming_effect>(10);
        load_oil_steaming_effect();
        
        oil_jump_pools ??= new List<oil_jump_effect>(10);
        load_oil_jump_effect();
        
        SetFryRecipe(foodAndTools);
    }
    
    public override void StartCook()
    {
        StartFry();
    }

    private async void load_oil_jump_effect()
    {
        if (oil_jump_pools.Count > 0) return;
        for (int i = 0; i < 10; i++)
        {
            var prefab = YooAssets.LoadAssetAsync<GameObject>("Assets/GameRes/Prefabs/AllFood/oil_jump_effect.prefab");
            await prefab.ToUniTask(this);
            var go  = Instantiate(prefab.AssetObject as GameObject,transform);
            var obj = go.GetComponent<oil_jump_effect>();
            go.SetActive(false);
            oil_jump_pools.Add(obj);
        }
    }
    
    private float oil_jump_interval;
    private void show_oil_jump()
    {
        // if (_curTemperature.Value > _currentRecipeDifficulty.maxTemperature*0.7f)
        // {
        //     return;
        // }
        if (oil_jump_pools.Count <= 0)
        {
            return;
        }

        oil_jump_interval -= Time.deltaTime;
        if (oil_jump_interval > 0)
        {
            return;
        }

        oil_jump_interval = 0.25f;
        var obj = oil_jump_pools.Last();
        oil_jump_pools.RemoveAt(oil_jump_pools.Count - 1);
        obj.transform.position = pan.transform.position;
        obj.PlayEffect(oil_jump_pools);
        // Debug.Log($"oil_jump_pools remain = {oil_jump_pools.Count}");
    }
    
    

}
