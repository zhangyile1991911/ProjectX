using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class CookResultWidget : UIComponent
{
    public CookResultWidget(GameObject go,UIWindow parent):base(go,parent)
    {
		
    }
    
    public override void OnCreate()
    {
    }
    
    public override void OnDestroy()
    {
        
    }
    
    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnUpdate()
    {
        
    }
    
    public void ShowGameOver(CookResult cookResult)
    {
        //标题
        var tbMenu = DataProviderModule.Instance.GetMenuInfo(cookResult.menuId);
        Txt_Menu.text = tbMenu.Name; 
        //评分
        StringBuilder sb = new StringBuilder();
        sb.Append(ZString.Format("完成度\t\t{0}\n", cookResult.Score));
        foreach (var pair in cookResult.QTEResult)
        {
            var tb = DataProviderModule.Instance.GetQTEInfo(pair.Key);
            sb.Append(ZString.Format("{0}\t\t\t{1}\n",tb.Name,pair.Value?"成功":"失败"));
        }

        foreach (var tag in cookResult.Tags)
        {
            var tb = DataProviderModule.Instance.DataBase.TbFlavour.DataMap[(int)tag];
            sb.Append(" ");
            sb.Append(tb.Desc);
        }

        sb.Append("\n");
        Txt_Result.text = sb.ToString();
    }

    
    
}