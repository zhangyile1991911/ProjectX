using System;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

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
    private CompositeDisposable handler;
    private List<bool> qteRecords;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        pan.Init();
        
        handler?.Clear();
        handler ??= new CompositeDisposable();
        
        _start = false;
        
        progressTopic = new Subject<bool>();
    }

    public void SetFryRecipe(FriedFoodRecipe recipe)
    {
        _currentRecipe = recipe;
        
        var min = _currentRecipe.temperatureArea.x;
        var max = _currentRecipe.temperatureArea.y;
        sliderBgMat.SetFloat("low",min);
        sliderBgMat.SetFloat("_medium",max);

        temperatureSlider.maxValue = _currentRecipe.maxTemperature;
        temperatureSlider.minValue = 0;
        progressSlider.maxValue = _currentRecipe.finishValue;
        progressSlider.minValue = 0;
        
        components?.Clear();
        components ??= new List<QTEComponent>(5);
        for (int i = 0; i < _currentRecipe.qteList.Count; i++)
        {
            var one = _currentRecipe.qteList[i];
            var handle = YooAssets.LoadAssetSync<GameObject>("Assets/GameRes/Prefabs/QTENode.prefab");
            var node = handle.InstantiateSync(qteArea);
            var comp = node.GetComponent<QTEComponent>();
            float y = qteArea.rect.height * one.progress;
            comp.self.anchoredPosition = new Vector3(0,y,0);
            comp.tips.text = ZString.Concat("Press ",one.pressKey.ToString());
            comp.gameObject.SetActive(false);
            components.Add(comp);
        }
    }

    public void StartFry()
    {
        handler?.Clear();
        _curTemperature = 0;
        _curProgress = 0;
        _start = true;
        
        qteRecords?.Clear();
        qteRecords ??= new List<bool>();
        for (int i = 0; i < _currentRecipe.qteList.Count; i++)
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
            handler.Clear();
        }).AddTo(handler);

        progressSlider.OnValueChangedAsObservable()
            .Subscribe(ListenProgress)
            .AddTo(handler);
        
        this.FixedUpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(pan.UpdatePan)
            .AddTo(handler);
        
        this.UpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(CalculateHeat)
            .AddTo(handler);

        this.UpdateAsObservable()
            .Where(_ => _start && Input.anyKeyDown)
            .Subscribe(ListenQTE)
            .AddTo(handler);

        gameOverText.gameObject.SetActive(false);
    }

    private void ListenProgress(float param)
    {
        for (int i = 0;i < _currentRecipe.qteList.Count;i++)
        {
            var qte = _currentRecipe.qteList[i];
            if (param >= qte.progress*_currentRecipe.finishValue)
            {
                components[i].gameObject.SetActive(true);    
            }
        }
    }
    
    private void CalculateHeat(Unit param)
    {
        //todo 考虑取消 根据距离的远近 加热速度不同 这种做法 太复杂了
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
        for (int i = 0; i < _currentRecipe.qteList.Count; i++)
        {
            var one = _currentRecipe.qteList[i];
            var curProgress = _curProgress / _currentRecipe.finishValue;
            if (curProgress >= one.progress && !qteRecords[i])
            {
                if (Input.GetKeyDown(one.pressKey))
                {
                    qteRecords[i] = true;
                    Debug.Log($"进度 {curProgress*100}% 按下了 {one.pressKey}");
                }
            }
        }
    }
}
