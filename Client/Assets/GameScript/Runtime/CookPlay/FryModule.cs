using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using cfg.food;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using Random = UnityEngine.Random;

public class FryModule : MonoBehaviour
{
    // public Material sliderBgMat;
    public PanSimulator pan;
    public Transform firePointTransform;

    [InspectorName("距离热度比例")]
    public AnimationCurve heatCurve;

    [InspectorName("移动速度降温比例")]
    public AnimationCurve lowerCurve;

    private FryingDifficulty _currentRecipeDifficulty;
    
    
    private bool _start;
    
    private ReactiveProperty<float> _curProgress;
    private ReactiveProperty<float> _curTemperature;
    
    private Subject<bool> _finishTopic;
    // private Subject<bool> timeUpTopic;

    private CompositeDisposable _handler;
    private List<AssetOperationHandle> _cacheHandles;
    private void Init()
    {
        pan.Init();
        
        _handler?.Clear();
        _handler ??= new CompositeDisposable();
        
        _start = false;
        
        _finishTopic = new Subject<bool>();

        _cacheHandles?.Clear();
        _cacheHandles ??= new List<AssetOperationHandle>(20);
        
        _qteAnimations?.Clear();
        _qteAnimations ??= new List<Animation>(5);

        _curProgress = new ReactiveProperty<float>();
        _curTemperature = new ReactiveProperty<float>();
        
        UIManager.Instance.OpenUI(UIEnum.FryingFoodWindow,null,null);

        EventModule.Instance.CookGameStartSub.Subscribe(StartFry);

        _result = new CookResult();
        _result.Tags = new HashSet<flavorTag>(5);
        _result.QTEResult = new Dictionary<int, bool>();
    }

    private void loadDifficultyConfig(cookDifficulty difficulty)
    {
        AssetOperationHandle handler = null;
        switch (difficulty)
        {
            case cookDifficulty.easy:
                handler = YooAssets.LoadAssetSync<FryingDifficulty>("Assets/GameRes/SOConfigs/Menu/FryMenu/FryLow.asset");
                break;
            case cookDifficulty.normal:
                handler = YooAssets.LoadAssetSync<FryingDifficulty>("Assets/GameRes/SOConfigs/Menu/FryMenu/FryMiddle.asset");
                break;
            case cookDifficulty.hard:
                handler = YooAssets.LoadAssetSync<FryingDifficulty>("Assets/GameRes/SOConfigs/Menu/FryMenu/FryHigh.asset");
                break;
            default:
                Debug.LogError($"handler == null");
                break;
        }
        _currentRecipeDifficulty = handler.AssetObject as FryingDifficulty;
    }

    // private List<bool> qteRecords;
    // private List<int> qteKeys;
    private List<Animation> _qteAnimations;
    private MenuInfo _tbMenuInfo;
    private CookResult _result;
    private List<qte_info> _tbQteInfos;    
    private async void loadQTEConfig(HashSet<int> qtes)
    {
        var groupId = DataProviderModule.Instance.GetQTEGroupId();
        Debug.Log($"loadQTEConfig = {groupId}");   
        _tbQteInfos ??= new List<qte_info>(10);
        _tbQteInfos.Clear();
        
        var tmpTb = DataProviderModule.Instance.GetQTEGroupInfo(groupId);
        for (int i = 0; i < tmpTb.Count; i++)
        {
            if (qtes.Contains(tmpTb[i].QteId))
            {
                _tbQteInfos.Add(tmpTb[i]);
            }
        }
        
        _uiWindow.LoadQTEConfigTips(_tbQteInfos);
        
        //QTE动画
        _qteAnimations?.Clear();
        _qteAnimations ??= new List<Animation>(5);

        foreach (var qteId in qtes)
        {
            var qteTB = DataProviderModule.Instance.GetQTEInfo(qteId);
            var loadHandle = YooAssets.LoadAssetAsync<GameObject>(qteTB.AnimResPath);
            _cacheHandles.Add(loadHandle);
            
            await loadHandle.ToUniTask();
            
            var prefb = loadHandle.AssetObject as GameObject;
            var go = Instantiate(prefb,pan.animNode);
            go.gameObject.SetActive(false);
            var ani = go.GetComponent<Animation>();
            ani.clip.legacy = true;
            _qteAnimations.Add(ani);
            _result.QTEResult.Add(qteId,false);
        }

    }

    private void loadRawFood(List<ItemTableData> foods)
    {
        //炒菜原材料
        foreach (var food in foods)
        {
            var tb = DataProviderModule.Instance.GetItemBaseInfo(food.Id);
            var handle = YooAssets.LoadAssetSync<GameObject>(tb.SceneResPath);
            _cacheHandles.Add(handle);
            var num = 5;
            for (int i = 0; i < num; i++)
            {
                var go = Instantiate(handle.AssetObject as GameObject,pan.transform);
                Vector3 pos = Random.insideUnitCircle*0.4f;
                pos.z = 0f;
                go.transform.localPosition = pos;
                var fs = go.GetComponent<FoodSimulator>();
                pan.AddFood(fs);
            }
        }
    }

    private FryingFoodWindow _uiWindow;
    private PickFoodAndTools _recipe;
    public void SetFryRecipe(PickFoodAndTools recipe)
    {
        Init();
        _tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(recipe.MenuId);
        if (_tbMenuInfo == null)
        {
            Debug.LogError($"recipe.MenuId {recipe.MenuId} == null");
        }
        
        loadDifficultyConfig(_tbMenuInfo.Difficulty);
        loadRawFood(recipe.CookFoods);
        
        _uiWindow = UIManager.Instance.Get(UIEnum.FryingFoodWindow) as FryingFoodWindow;
        if (_uiWindow == null)
        {
            Debug.LogError("打不开FringFoodWindow");
        }
        _uiWindow.SetDifficulty(_currentRecipeDifficulty);
        loadQTEConfig(recipe.QTESets);
        _recipe = recipe;
    }

    public void StartFry(bool param)
    {
        _handler?.Clear();
        
        _curTemperature.Value = 0;
        _curProgress.Value = 0;
        _start = param;

        var min = _currentRecipeDifficulty.temperatureArea.x * _currentRecipeDifficulty.maxTemperature;
        var max = _currentRecipeDifficulty.temperatureArea.y * _currentRecipeDifficulty.maxTemperature;

        this.UpdateAsObservable()
            .Where(_=>_start&&(_curTemperature.Value >= min && _curTemperature.Value <= max))
            .Subscribe(HeatFood).AddTo(_handler);


        var TimeCounter = Observable.
            Timer(TimeSpan.FromSeconds(_currentRecipeDifficulty.duration))
            .Select(_ => false);

        _uiWindow.SetProgressListener(_curProgress);
        _uiWindow.SetTemperatureListener(_curTemperature);
        
        Observable.Amb(
            _finishTopic,
            TimeCounter
            ).Subscribe(GameOver).AddTo(_handler);
        
        this.UpdateAsObservable()
            .Where(_ => _start && Input.anyKeyDown)
            .Subscribe(ListenQteInput)
            .AddTo(_handler);
        
        pan.GameStart(_handler);

        this.UpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(CalculateHeat)
            .AddTo(_handler);
    }

    private void CalculateHeat(Unit param)
    {
        var distance = Vector2.Distance(pan.transform.position, firePointTransform.position);
        var add = heatCurve.Evaluate(distance)*Time.deltaTime;
        // Debug.Log($"distance = {distance} add = {add}");
        // var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        // Debug.Log($"velocity = {pan.velocity} sub = {sub}");
        var tmp = _curTemperature.Value;
        tmp += (add+sub);
        // Debug.Log($"distance = {distance} add = {add} sub = {sub} _curTemperature = {_curTemperature}");
        
        _curTemperature.Value = Mathf.Clamp(tmp, 0, _currentRecipeDifficulty.maxTemperature);
    }
    
    private void HeatFood(Unit param)
    {
        var temp = _curProgress.Value;
        temp += _currentRecipeDifficulty.addValue * Time.deltaTime;
        temp = Mathf.Clamp(temp, 0, _currentRecipeDifficulty.finishValue);
        _curProgress.Value = temp;
        
        //判断是否结束
        if (_curProgress.Value >= _currentRecipeDifficulty.finishValue)
        {
            _finishTopic.OnNext(true);
            _finishTopic.OnCompleted();
        }

        ListenProgress(_curProgress.Value);
    }

    private void GameOver(bool param)
    {
        Debug.Log($"GameOver IsSuccess = {param}");
        _start = false;
        _handler.Clear();
        
        UnloadRes();
        
        _result.menuId = _tbMenuInfo.Id;
        _result.CompletePercent = _curProgress.Value;
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

        //消耗时间
        var clocker = UniModule.GetModule<Clocker>();
        clocker.AddMinute(_tbMenuInfo.CostTime);
        _tbQteInfos.Clear();
        
        _uiWindow.ShowGameOver(_result);
        EventModule.Instance.CookFinishTopic.OnNext(_result);
    }

    private void UnloadRes()
    {
        pan.RemoveAllFood();
        foreach (var handler in _cacheHandles)
        {
            handler.Release();
        }
    }
    
    private void ListenProgress(float param)
    {
        for (int i = 0;i < _tbQteInfos.Count;i++)
        {
            var qteInfo = _tbQteInfos[i];
            var percent = _curProgress.Value / _currentRecipeDifficulty.finishValue;
            if (percent >= qteInfo.StartArea && percent < qteInfo.EndArea)
            {
                _uiWindow.ShowQteTips(qteInfo.QteId);
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
    
}
