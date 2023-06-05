using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class PrepareStateNode : IStateNode
{
    private StateMachine _machine;
    private RestaurantEnter _restaurant;
    private CompositeDisposable _handle;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handle = new CompositeDisposable();
    }

    public void OnEnter(object param)
    {
        UIManager.Instance.OpenUI(UIEnum.KitchenWindow, (uiBase)=>{}, null);
        EventModule.Instance.StartCookSub.Subscribe(StartCook).AddTo(_handle);
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        UIManager.Instance.CloseUI(UIEnum.KitchenWindow);
        _handle?.Clear();
    }

    private async void StartCook(FoodReceipt foo)
    {
        var tb = DataProviderModule.Instance.GetMenuInfo(foo.MenuId);
        // var sceneHandler = YooAssets.LoadSceneAsync(tb.CookScene,LoadSceneMode.Additive);
        var sceneHandler = YooAssets.LoadSceneAsync(tb.CookScene);
        
        await sceneHandler.ToUniTask();
        
        List<GameObject> rootObjects = new (10);
        sceneHandler.SceneObject.GetRootGameObjects(rootObjects);
        FryModule fm = null;
        foreach (var one in rootObjects)
        {
            if (one.name.Equals("FryModule"))
            {
                fm = one.GetComponent<FryModule>();
                break;
            }
        }
        Debug.Log(fm.gameObject);
        
        _machine.ChangeState<ProduceStateNode>();
    }
}
