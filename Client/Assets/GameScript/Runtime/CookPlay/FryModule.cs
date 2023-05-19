using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using Random = UnityEngine.Random;

public class FryModule : MonoBehaviour
{
    public Slider temperatureSlider;
    public Slider progressSlider;
    public TextMeshProUGUI gameOverText;
    public Material sliderBgMat;
    public PanSimulator pan;
    public Transform firePointTransform;
    public RectTransform qteArea;
    
    [InspectorName("距离热度比例")]
    public AnimationCurve heatCurve;

    [InspectorName("移动速度降温比例")]
    public AnimationCurve lowerCurve;

    private FriedFoodRecipe _currentRecipe;
    
    private float _curTemperature;
    private float _curProgress;
    private bool _start;
    // private IDisposable _temperatureDisposal;
    // private IDisposable _gameFinishDisposal;
    // private IDisposable _progressValueDisposal;
    private Subject<bool> progressTopic;
    private Subject<bool> timeUpTopic;
    private List<QTEComponent> components;
    private CompositeDisposable _handler;
    private List<bool> qteRecords;
    private List<int> qteKeys;
    private List<Animation> qteAnimations;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        pan.Init();
        
        _handler?.Clear();
        _handler ??= new CompositeDisposable();
        
        _start = false;
        
        progressTopic = new Subject<bool>();
    }

    public void SetFryRecipe(FriedFoodRecipe recipe)
    {
        _currentRecipe = recipe;
        
        var min = _currentRecipe.temperatureArea.x;
        var max = _currentRecipe.temperatureArea.y;
        sliderBgMat.SetFloat("_low",min);
        sliderBgMat.SetFloat("_medium",max);

        temperatureSlider.maxValue = _currentRecipe.maxTemperature;
        temperatureSlider.minValue = 0;
        progressSlider.maxValue = _currentRecipe.finishValue;
        progressSlider.minValue = 0;
        
        components?.Clear();
        //初始化QTE相关
        components ??= new List<QTEComponent>(5);
        qteKeys = _currentRecipe.SortQTEKey();
        for(int  i = 0;i < qteKeys.Count;i++)
        {
            var handle = YooAssets.LoadAssetSync<GameObject>("Assets/GameRes/Prefabs/QTENode.prefab");
            var node = handle.InstantiateSync(qteArea);
            var comp = node.GetComponent<QTEComponent>();
            var progress = (float)qteKeys[i] / _currentRecipe.finishValue;
            float y = qteArea.rect.height * progress;
            comp.self.anchoredPosition = new Vector3(0,y,0);
            comp.tips.text = ZString.Concat("Press ",_currentRecipe.qteDict[qteKeys[i]].pressKey.ToString());
            comp.gameObject.SetActive(false);
            components.Add(comp);
        }
        //QTE动画
        qteAnimations?.Clear();
        qteAnimations ??= new List<Animation>(5);
        for (int i = 0; i < qteKeys.Count; i++)
        {
            int key = qteKeys[i];
            var go = Instantiate(_currentRecipe.qteDict[key].anim,pan.animNode);
            go.gameObject.SetActive(false);
            var ani = go.GetComponent<Animation>();
            ani.clip.legacy = true;
            qteAnimations.Add(ani);
        }

        foreach (var one in _currentRecipe.qteDict)
        {
            Debug.Log($"{one.Key}");
        }
        //炒菜原材料
        foreach (var food in _currentRecipe.rawFoodDictionary)
        {
            var num = food.Key;
            var pref = food.Value;
            for (int i = 0; i < num; i++)
            {
                var gameObject = Instantiate(pref,pan.transform);
                gameObject.transform.localPosition = Random.insideUnitCircle*0.4f;
                var fs = gameObject.GetComponent<FoodSimulator>();
                fs.panSimulator = pan;
                pan.AddFood(fs);
            }
        }
    }

    public void StartFry()
    {
        _handler?.Clear();
        _curTemperature = 0;
        _curProgress = 0;
        _start = true;
        
        qteRecords?.Clear();
        qteRecords ??= new List<bool>();
        for (int i = 0; i < qteKeys.Count; i++)
        {
            qteRecords.Add(false);
        }
        
        var min = _currentRecipe.temperatureArea.x * _currentRecipe.maxTemperature;
        var max = _currentRecipe.temperatureArea.y * _currentRecipe.maxTemperature;

        this.UpdateAsObservable()
            .Where(_=>_start&&(_curTemperature >= min && _curTemperature <= max))
            // .Where(_=>)
            .Subscribe(HeatFood).AddTo(gameObject);


        var gameCounter = Observable.
            Timer(TimeSpan.FromSeconds(_currentRecipe.duration))
            .Select(_ => false);
        
        Observable.Amb(
            progressTopic,
            gameCounter
            ).Subscribe(_ =>
        {
            Debug.Log("GameOver");
            _start = false;
            gameOverText.gameObject.SetActive(true);
            _handler.Clear();
        }).AddTo(_handler);

        progressSlider.OnValueChangedAsObservable()
            .Subscribe(ListenProgress)
            .AddTo(_handler);
        
        this.UpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(CalculateHeat)
            .AddTo(_handler);

        this.UpdateAsObservable()
            .Where(_ => _start && Input.anyKeyDown)
            .Subscribe(ListenQTE)
            .AddTo(_handler);
        pan.GameStart(_handler);
        gameOverText.gameObject.SetActive(false);
    }

    private void ListenProgress(float param)
    {
        for(int i = 0;i < qteKeys.Count;i++)
        {
            if (param >= qteKeys[i])
            {
                components[i].gameObject.SetActive(true);    
            }
        }
    }
    
    private void CalculateHeat(Unit param)
    {
        var distance = Vector2.Distance(pan.transform.position, firePointTransform.position);
        var add = heatCurve.Evaluate(distance)*Time.deltaTime;
        // Debug.Log($"distance = {distance} add = {add}");
        // var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        // Debug.Log($"velocity = {pan.velocity} sub = {sub}");
        _curTemperature += (add+sub);
        // Debug.Log($"distance = {distance} add = {add} sub = {sub} _curTemperature = {_curTemperature}");
        
        _curTemperature = Mathf.Clamp(_curTemperature, 0, _currentRecipe.maxTemperature);
        temperatureSlider.value = _curTemperature;
    }
    private void HeatFood(Unit param)
    {
        _curProgress += _currentRecipe.addValue * Time.deltaTime;
        _curProgress = Mathf.Clamp(_curProgress, 0, _currentRecipe.finishValue);
        progressSlider.value = _curProgress;
        // Debug.Log($"HeatFood!!!!! {_curProgress}");
        if (_curProgress >= _currentRecipe.finishValue)
        {
            progressTopic.OnNext(true);
        }
    }

    private void ListenQTE(Unit param)
    {
        for (int i = 0; i < qteKeys.Count(); i++)
        {
            var p = qteKeys[i];
            if (_curProgress >= p && !qteRecords[i])
            {
                var kc = _currentRecipe.qteDict[p].pressKey;
                if (Input.GetKeyDown(kc))
                {
                    qteRecords[i] = true;
                    Debug.Log($"进度 {p} 按下了 {kc}");
                    var one = qteAnimations[i];
                    one.gameObject.SetActive(true);
                    one.Play();
                    var duration = one.clip.length;
                    Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ =>
                    {
                        one.gameObject.SetActive(false);
                    }).AddTo(_handler);
                }
            }
        }
    }
}
