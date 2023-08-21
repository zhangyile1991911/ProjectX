using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SameChatBubble : EqualityComparer<ChatBubble>
{
    public override bool Equals(ChatBubble x, ChatBubble y)
    {
        return x.InstanceId == y.InstanceId;
    }

    public override int GetHashCode(ChatBubble obj)
    {
        int hCode = (int)obj.InstanceId ^ 1; 
        return hCode;
    }
}
/// <summary>
/// Auto Generate Class!!!
/// </summary>
public partial class ChatBubble : UIComponent
{
    private static long ChatBubbleInstanceId = 0; 
    public RestaurantRoleBase Owner => _owner;
    public int ChatId => _chatId;
    private RestaurantRoleBase _owner;
    private int _chatId;
    private Action<ChatBubble> _click;
    private Button _btn;
    private Tweener _tweener;
    public long InstanceId => _instanceId;
    private long _instanceId;
    public ChatBubble(GameObject go,UIWindow parent):base(go,parent)
    {
        _instanceId = ChatBubbleInstanceId++;
    }
    
    public override void OnCreate()
    {
        _btn = uiTran.GetComponent<Button>();
        _btn.OnClickAsObservable().Subscribe(onBubbleClick).AddTo(uiTran);
    }

    public override void OnShow(UIOpenParam openParam)
    {
        base.OnShow(openParam);
        if (_tweener != null)
        {
            _tweener.timeScale = 1f;    
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        if (_tweener != null)
        {
            _tweener.timeScale = 0f;    
        }
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        stopFloating();
        _owner = null;
        _click = null;
        GameObject.Destroy(uiGo);
    }

    public override void OnUpdate()
    {
        
    }

    private void onBubbleClick(Unit param)
    {
        stopFloating();
        _click?.Invoke(this);
    }

    private void stopFloating()
    {
        _tweener?.Pause();
        _tweener?.Kill();
        _tweener = null;
    }
    
    public void SetBubbleInfo(int chatId,RestaurantRoleBase origin,Action<ChatBubble> click)
    {
        _chatId = chatId;
        var positionToUI = UIManager.Instance.WorldPositionToUI(origin.ChatNode.position);
        var characterObj = origin as RestaurantCharacter;
        var y_offset = (characterObj.SaidBubbleNum - 1) * uiRectTran.sizeDelta.y;
        positionToUI.y += y_offset;
        uiRectTran.anchoredPosition = positionToUI;
        refreshContent(chatId);
        
        _tweener = uiRectTran.DOAnchorPosY(positionToUI.y+20f, 2f).SetLoops(-1,LoopType.Yoyo);
        _owner = origin;
        _click = click;
    }

    private void refreshContent(int chatId)
    {
        var bubble = DataProviderModule.Instance.GetCharacterBubble(chatId);
        if (bubble == null)
        {
            Txt_content.text = "error";
        }
        else
        {
            Txt_content.text = bubble.Title;
        }
    }

    public override bool Equals(object obj)
    {
        var tmp = obj as ChatBubble;
        return tmp._instanceId == _instanceId;
    }
}