using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Cysharp.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
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
    private IDisposable _temperatureDisposal;
    private IDisposable _gameFinishDisposal;
    private IDisposable _progressValueDisposal;
    private Subject<bool> progressTopic;
    private Subject<bool> timeUpTopic;
    private List<QTEComponent> components;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        pan.Init();
        
        this.FixedUpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(pan.UpdatePan)
            .AddTo(gameObject);
        
        this.UpdateAsObservable()
            .Where(_ => _start)
            .Subscribe(CalculateHeat)
            .AddTo(gameObject);
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
        _curTemperature = 0;
        _curProgress = 0;
        _start = true;
        
        var min = _currentRecipe.temperatureArea.x * _currentRecipe.maxTemperature;
        var max = _currentRecipe.temperatureArea.y * _currentRecipe.maxTemperature;
        
        _temperatureDisposal?.Dispose();
        _temperatureDisposal = this.UpdateAsObservable()
            .Where(_=>_start&&(_curTemperature >= min && _curTemperature <= max))
            // .Where(_=>)
            .Subscribe(HeatFood).AddTo(gameObject);


        var gameCounter = Observable.Timer(TimeSpan.FromSeconds(_currentRecipe.duration)).Select(_ => false);
        _gameFinishDisposal?.Dispose();
        _gameFinishDisposal = Observable.Amb(
            progressTopic,
            gameCounter
            ).Subscribe(_ =>
        {
            Debug.Log("GameOver");
            _start = false;
            gameOverText.gameObject.SetActive(true);
        });
        
        _progressValueDisposal?.Dispose();
        _progressValueDisposal = progressSlider.OnValueChangedAsObservable()
            .Subscribe(ListenProgress)
            .AddTo(gameObject);
        
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
        var distance = Vector2.Distance(pan.transform.position, firePointTransform.position);
        var add = heatCurve.Evaluate(distance)*Time.deltaTime;
        // Debug.Log($"distance = {distance} add = {add}");
        var sub = lowerCurve.Evaluate(pan.velocity)*Time.deltaTime;
        // Debug.Log($"velocity = {pan.velocity} sub = {sub}");
        _curTemperature += (add-sub);
        Debug.Log($"distance = {distance} add = {add} sub = {sub} _curTemperature = {_curTemperature}");
        
        _curTemperature = Mathf.Clamp(_curTemperature, 0, _currentRecipe.maxTemperature);
        temperatureSlider.value = _curTemperature;
    }
    private void HeatFood(Unit param)
    {
        _curProgress += _currentRecipe.addValue * Time.deltaTime;
        _curProgress = Mathf.Clamp(_curProgress, 0, _currentRecipe.finishValue);
        progressSlider.value = _curProgress;
        Debug.Log($"HeatFood!!!!! {_curProgress}");
        if (_curProgress >= _currentRecipe.finishValue)
        {
            progressTopic.OnNext(true);
        }
    }
}
