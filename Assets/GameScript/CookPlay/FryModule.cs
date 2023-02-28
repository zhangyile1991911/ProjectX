using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FryModule : MonoBehaviour
{
    public Slider temperatureSlider;
    public Slider progressSlider;
    public TextMeshProUGUI gameOverText;
    public Material sliderBgMat;
    public PanSimulator pan;
    public Transform firePointTransform;

    [InspectorName("距离热度比例")]
    public AnimationCurve heatCurve;

    [InspectorName("移动速度降温比例")]
    public AnimationCurve lowerCurve;

    private Recipe _currentRecipe;
    
    private float _curTemperature;
    private float _curProgress;
    private bool _start;
    private IDisposable _temperatureDisposal;
    private IDisposable _gameFinishDisposal;
    private Subject<bool> progressTopic;
    private Subject<bool> timeUpTopic;
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

    public void SetFryRecipe(Recipe recipe)
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
    }

    public void StartFry()
    {
        _curTemperature = 0;
        _curProgress = 0;
        _start = true;
        
        var min = _currentRecipe.temperatureArea.x;
        var max = _currentRecipe.temperatureArea.y;
        
        _temperatureDisposal?.Dispose();
        _temperatureDisposal = this.UpdateAsObservable()
            .Where(_=>_start)
            .Where(_=>_curTemperature >= min && _curTemperature <= max)
            .Subscribe(HeatFood).AddTo(gameObject);

        
        _gameFinishDisposal?.Dispose();
        _gameFinishDisposal = Observable.Amb(
            progressTopic,
            Observable.Timer(TimeSpan.FromSeconds(20)).Select(_=>false)
            ).Subscribe(_ =>
        {
            Debug.Log("GameOver");
            _start = false;
            gameOverText.gameObject.SetActive(true);
        });
        
        gameOverText.gameObject.SetActive(false);
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
        _curProgress += _currentRecipe.addValue;
        _curProgress = Mathf.Clamp(_curProgress, 0, _currentRecipe.finishValue);
        progressSlider.value = _curProgress;
        if (_curProgress >= _currentRecipe.finishValue)
        {
            progressTopic.OnNext(true);
        }
    }
}
