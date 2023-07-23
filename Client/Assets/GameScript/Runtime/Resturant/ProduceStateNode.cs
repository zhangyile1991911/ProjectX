using System.Collections;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ProduceStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private PickFoodAndTools _data;
    private FryModule _fryModule;
    private CompositeDisposable _handle;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
    }

    public async void OnEnter(object param)
    {
        _data = param as PickFoodAndTools;
        // var menuTb = DataProviderModule.Instance.GetMenuInfo(_data.MenuId);
        GameObject go = null;
        var openWindowParam = new CookWindowParamData();
        openWindowParam.StateMachine = _machine;
        switch (_data.CookTools)
        {
            case cookTools.Cook:
                await UIManager.Instance.OpenUI(UIEnum.FryingFoodWindow,openWindowParam,UILayer.Bottom);
                go = await _restaurant.ShowCookGamePrefab(_data.CookTools);
                // go = await _restaurant.LoadCookGamePrefab(_data.CookTools);
                _fryModule = go.GetComponent<FryModule>();
                _fryModule.SetFryRecipe(_data);
                _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
                break;
            default:
                Debug.LogError("其他玩法暂时没有实现");
                break;
        }
        
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.Kitchen);
        
        // EventModule.Instance.ExitKitchenSub.Subscribe(_ =>
        // {
        //     _machine.ChangeState<PrepareStateNode>();
        // }).AddTo(_handle);

        EventModule.Instance.CookGameStartSub.Subscribe(_ =>
        {
            foreach (var one in _data.CookFoods)
            {
                UserInfoModule.Instance.SubItemNum(one.Id,(uint)one.Num);    
            }
        }).AddTo(_handle);
    }
    
    public void OnExit()
    {
        _restaurant.CutCamera(RestaurantEnter.RestaurantCamera.RestaurantMain);
        _handle.Clear();
        
        switch (_data.CookTools)
        {
            case cookTools.Cook:
                UIManager.Instance.CloseUI(UIEnum.FryingFoodWindow);
                _restaurant.HideCookGamePrefab(_data.CookTools);
                break;
            default:
                Debug.LogError("其他玩法暂时没有实现");
                break;
        }
        _data = null;
        
    }
    
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _machine.ChangeState<PrepareStateNode>();
        }
    }
}
