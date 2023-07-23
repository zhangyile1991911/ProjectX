using System.Collections.Generic;
using System.Linq;
using SuperScrollView;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class RestaurantStatementWindow : UIWindow
{
    private Dictionary<int, int> soldMenuCount;//menuId num
    private List<int> gridMenuData;
    private StateMachine _stateMachine;
    public override void OnCreate()
    {
        base.OnCreate();
        Grid_StatementList.InitGridView(0,onGetStatement);
        soldMenuCount ??= new Dictionary<int, int>();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        soldMenuCount.Clear();
    }
    
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        var tmp = openParam as FlowControlWindowData;
        _stateMachine = tmp.StateMachine;
        var soldMenus = UserInfoModule.Instance.GetRestaurantSoldMenuId();
        foreach (var one in soldMenus)
        {
            if (soldMenuCount.ContainsKey(one))
            {
                soldMenuCount[one] += 1;
            }
            else
            {
                soldMenuCount[one] = 1;
            }
        }

        gridMenuData = soldMenuCount.Keys.ToList();
        Grid_StatementList.SetListItemCount(gridMenuData.Count);
        Btn_bg.OnClickAsObservable().Subscribe(onClickFinish).AddTo(handles);
    }

    public override void OnHide()
    {
        base.OnHide();
        Grid_StatementList.RecycleAllItem();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void onClickFinish(Unit param)
    {
        _stateMachine.ChangeState<WaitStateNode>();
    }

    private LoopGridViewItem onGetStatement(LoopGridView gridView,int itemIndex, int row, int column)
    {
        var item = gridView.NewListViewItem("StatementWidget");
        StatementWidget cell = null;
        if (item.IsInitHandlerCalled)
        {
            cell = item.UserObjectData as StatementWidget;
        }
        else
        {
            cell = new StatementWidget(item.gameObject,this);
            item.UserObjectData = cell;
            item.IsInitHandlerCalled = true;
        }
        cell.OnShow(null);

        var menuId = gridMenuData[itemIndex];
        var menuNum = soldMenuCount[menuId];
        cell.SetStatementInfo(menuId,menuNum);
        // cell.SetStatementInfo(data.Id,data.Num,canChoice,clickFoodMaterial);
        
        return item;
    }
}