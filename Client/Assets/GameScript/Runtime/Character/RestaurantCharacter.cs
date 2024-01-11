using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.character;
using cfg.common;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEditor.VersionControl;
using UnityEngine;
using Yarn.Unity;
using YooAsset;
using Random = UnityEngine.Random;

public class DialogueTitleInfo
{
    public int ID;
    public bubbleType Type;
}
public class RestaurantCharacter : RestaurantRoleBase
{
    //表格配置数据
    protected List<CharacterBubble> _mainLineBubbleTB;
    protected List<CharacterBubble> _talkBubbleTB;
    protected List<CharacterBubble> _orderBubbleTB;
    // private List<CharacterBubble> _commentBubbleTB;

    public CharacterBubble OMAKASEComment { get; protected set; }
    public CharacterBubble HybridOrderComment { get; protected set; }
    public CharacterBubble SpecifiedOrderComment { get; protected set;}
    //记录当前已经生成的chatid
    private List<DialogueTitleInfo> _saidBubbles;

    public int SaidBubbleNum => _saidBubbles.Count;

    //收到的料理ScheduleGroup
    private CookResult _receivedFood = null;

    public OrderMealInfo CurOrderInfo
    {
        get => _orderMealInfo;
        set
        {
            _orderMealInfo = value;
            if (_orderMealInfo != null)
            {
                // createOrderBoard();
                _npcData.TodayOrderCount += 1;
                UserInfoModule.Instance.AddNPCOrder(_orderMealInfo);    
            }
        }
    }
    private OrderMealInfo _orderMealInfo;
    
    private Clocker _clocker;
    // private int foodScore;
    
    // public Transform OrderNode => _orderNode;
    // protected Transform _orderNode;
    
    protected OrderBoard orderBoardBoard;

    protected Animator _animator;

    
    public override void InitCharacter(CharacterBaseInfo info)
    {
       
        base.InitCharacter(info);
        if (!_baseInfo.PicturePath.Equals(""))
        {
            LoadSprite(_baseInfo.PicturePath);    
        }

        _animator = GetComponent<Animator>();
        // _orderNode = transform.Find("OrderNode");
        _saidBubbles = new(10);
       
        _clocker = UniModule.GetModule<Clocker>();
        
    }

    public override void ReleaseCharacter()
    {
        base.ReleaseCharacter();
        
        _saidBubbles.Clear();
        _animator = null;
        
        // for (int i = 0; i < _orderNode.childCount;i++)
        // {
        //     var one = _orderNode.GetChild(i);
        //     Destroy(one.gameObject);
        // }
        Destroy(gameObject);
    }
    
    protected override void LoadTableData()
    {
        base.LoadTableData();
        _mainLineBubbleTB = new(10);
        _talkBubbleTB = new(10);
        _orderBubbleTB = new(10);
        // _commentBubbleTB = new(10);
        
        if (_baseInfo.Soul == 0) return;
        
        var dataList = DataProviderModule.Instance.GetCharacterBubbleList(CharacterId);
        for (int i = 0; i < dataList.Count; i++)
        {
            switch (dataList[i].BubbleType)
            {
                case bubbleType.Talk:
                    _talkBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.Omakase:
                case bubbleType.HybridOrder:
                case bubbleType.SpecifiedOrder:
                    _orderBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.MainLine:
                    _mainLineBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.SpecifiedComment:
                    SpecifiedOrderComment = dataList[i];
                    break;
                case bubbleType.HybridComment:
                    HybridOrderComment = dataList[i];
                    break;
                case bubbleType.OmakaseComment:
                    OMAKASEComment = dataList[i];
                    break;
            }
        }
    }
    
    private bool checkPreCondition(List<int> preConditions)
    {
        var result = true;
        foreach (var one in preConditions)
        {
            if(one==0)continue;

            if (UserInfoModule.Instance.HaveReadDialogueId(one)) continue;
            result = false;
            break;
        }

        return result;
    }
    private int generateMainLine()
    {
        foreach (var one in _mainLineBubbleTB)
        {
            var exist= UserInfoModule.Instance.HaveReadDialogueId(one.Id);
            if(exist)continue;
            
            var isEnough = Friendliness >= one.FriendValue.StartValue &&
                           Friendliness <= one.FriendValue.EndValue;
            if(!isEnough)continue;

            //如果没有前置条件
            //当前这条有没有读过
            //如果没有读过就这条
            var isCond = checkPreCondition(one.PreCondition);
            if(!isCond)continue;
            
            var isDay = checkWeekday(one.WeekDay);
            if (!isDay)continue;
            
            return one.Id;

        }

        return -1;
    }

    private List<CharacterBubble> conditionTalk = new(20);
    private int generateTalk()
    {
        conditionTalk.Clear();
        foreach (var one in _talkBubbleTB)
        {
            var isEnough = Friendliness >= one.FriendValue.StartValue &&
                           Friendliness <= one.FriendValue.EndValue;
            if(!isEnough)continue;
            var isCondition = checkPreCondition(one.PreCondition);
            if(!isCondition)continue;
            if (!checkWeekday(one.WeekDay)) continue;
            conditionTalk.Add(one);
        }

        if (conditionTalk.Count <= 0) return -1;
        int index = Random.Range(0, conditionTalk.Count);   
        var choiced = conditionTalk[index];
        return choiced.Id;
    }

    List<CharacterBubble> _orderPool = new List<CharacterBubble>(10);
    private CharacterBubble generateOrderChatId()
    {
        _orderPool.Clear();
        foreach (var one in _orderBubbleTB)
        {
            var isEnough = Friendliness >= one.FriendValue.StartValue &&
                           Friendliness <= one.FriendValue.EndValue;
            if(!isEnough)continue;
            
            var isCondition = checkPreCondition(one.PreCondition);
            if(!isCondition)continue;
            
            if (checkWeekday(one.WeekDay))
                _orderPool.Add(one);
        }

        if (_orderPool.Count <= 0) return null;

        var index = Random.Range(0, _orderPool.Count);
        
        return _orderPool[index];
    }

    private bool checkWeekday(List<WeekDay> weekDays)
    {
        foreach (var one in weekDays)
        {
            if (one == WeekDay.AllWeekDay)
                return true;
            if (one == _clocker.NowDateTime.DayOfWeek)
                return true;
        }

        return false;
    }

    int haveMainLineChatId()
    {
        var result = _saidBubbles.FirstOrDefault(one=>one.Type==bubbleType.MainLine);
        return result?.ID ?? 0;
    }

    int haveTalkChatId()
    {
        return _saidBubbles.Count(one=>one.Type==bubbleType.Talk);
    }

    int haveOrderChatId()
    {
        var result = _saidBubbles.FirstOrDefault(one=>
            one.Type==bubbleType.HybridOrder||one.Type==bubbleType.Omakase||one.Type==bubbleType.SpecifiedOrder);
        return result?.ID ?? 0;
    }
    
    public void GenerateChatId()
    {
        //气泡生成规则
       
        //1 主线剧情
        GenerateMain();

        GenerateTalkId();

        GenerateOrder();
    }

    public bool GenerateMain()
    {
        if (haveMainLineChatId() <= 0)
        {
            var info = new CharacterSaidInfo
            {
                CharacterId = CharacterId
            };
            info.ChatId = generateMainLine();
            if (info.ChatId > 0)
            {
                _saidBubbles.Add(new DialogueTitleInfo(){ID = info.ChatId,Type = bubbleType.MainLine});
                EventModule.Instance.CharBubbleTopic.OnNext(info);
                return true;
            }
        }

        return false;
    }
    
    public bool GenerateTalkId()
    {
        //普通重复的对话
        if (haveTalkChatId() > 5)
        {
            return false;
        }
        var talkId = generateTalk();
        if (talkId < 0)
        {
            return false;
        }

        var existed = _saidBubbles.Find(one => one.ID == talkId);
        if (existed != null) return false;
        
        var info = new CharacterSaidInfo
        {
            CharacterId = CharacterId,
            ChatId = talkId
        };
        _saidBubbles.Add(new DialogueTitleInfo(){ID = info.ChatId,Type = bubbleType.Talk});
        EventModule.Instance.CharBubbleTopic.OnNext(info);
        return true; ;
    }

    public bool GenerateOrder()
    {
        //下单
        if (haveOrderChatId() > 0) return true;
        
        var chatBubble = generateOrderChatId();
        if (chatBubble == null) return false;
        var info = new CharacterSaidInfo
        {
            CharacterId = CharacterId,
            ChatId =  chatBubble.Id,
        };
        _saidBubbles.Add(new DialogueTitleInfo(){ID = info.ChatId,Type = chatBubble.BubbleType});
        
        EventModule.Instance.CharBubbleTopic.OnNext(info);
        return true;
    }

    public void AddCommentChat(int chatId,bubbleType commentType)
    {
        _saidBubbles.Add(new DialogueTitleInfo(){ID = chatId,Type = commentType});
    }
    

    public override bool Equals(object other)
    {
        var otherCharacter = other as RestaurantCharacter;
        return otherCharacter.CharacterId == this.CharacterId;
    }

    public override void InjectVariableToDialogue(VariableStorageBehaviour storageBehaviour)
    {
       base.InjectVariableToDialogue(storageBehaviour);

        if (_receivedFood != null)
        {
            storageBehaviour.SetValue("$orderId",CurOrderInfo.MenuId);
            storageBehaviour.SetValue("$orderType",(int)CurOrderInfo.OrderType);
            storageBehaviour.SetValue("$foodscore",commentFood(_receivedFood));
            storageBehaviour.SetValue("$matchTag",isMatchTags());
        }
        storageBehaviour.SetValue("$withPartner",(_npcData.PartnerId > 0));
    }

    // public void DialogueOrder(int menuId)
    // {
    //     _dialogueOrderId = menuId;
    // }
    
    public void ReceiveFood(CookResult food,bool switchStatus = true)
    {
        _receivedFood = food;
        _receivedFood.characterId = CharacterId;
        UserInfoModule.Instance.UpdateCookedMealOwner(food);
        for (int i = 0; i < _saidBubbles.Count; i++)
        {
            var one = _saidBubbles[i];
            var isOrder = one.Type == bubbleType.HybridOrder || one.Type == bubbleType.Omakase ||
                one.Type == bubbleType.SpecifiedOrder;
            
            if(!isOrder)continue;
            
            _saidBubbles.RemoveAt(i);
            break;
        }

        if (switchStatus)
        {
            CurBehaviour = new CharacterEating();    
        }
    }

    public void ClearReceiveFood()
    {
        UserInfoModule.Instance.PayCookedMeal(_receivedFood);
        _receivedFood = null;
        UserInfoModule.Instance.RemoveNPCOrder(CharacterId);
        CurOrderInfo = null;
    }

    private int commentFood(CookResult food)
    {
        var foodScore = 0;
        //时间内完成获得50分基础分
        foodScore = food.Score >= 1.0 ? 50 : 0;
        //标签评分：每个正向标签+10分，每个负面标签-10分，无关联标签0分
        foreach (var tag in food.Tags)
        {
            foodScore += TBBaseInfo.LikeFlavour.Contains(tag) ? 10 : 0;
            foodScore += TBBaseInfo.UnlikeFlavour.Contains(tag) ? -10 : 0;
        }
        //QTE评分：完成一个+5分 失败一个-5分
        foreach (var success in food.QTEResult.Values)
        {
            foodScore += success ? 5 : -5;
        }

        return foodScore;
    }

    private bool isMatchTags()
    {
        if (CurOrderInfo.flavor is not { Count: > 0 }) return false;

        int flavorNum = CurOrderInfo.flavor.Count;

        foreach (var tag in _receivedFood.Tags)
        {
            if (CurOrderInfo.flavor.Contains(tag))
                flavorNum--;
        }
        return flavorNum <= 0;
    }

    private bool willLeave = false;
    public void SetLeave()
    {
        willLeave = true;
    }
    
    public bool IsTimeToLeave()
    {
        if (willLeave)
        {
            return true;
        }
        return false;
    }
    
    public async void ShowOrderBoard()
    {
        if (orderBoardBoard == null)
        {
            var prefab = await LoadPrefab("Assets/GameRes/Prefabs/OrderBoard.prefab");
            var go = Instantiate<GameObject>(prefab, transform);
            go.transform.position = Restaurant.SeatOrderBoardPositionInWorld(_seatOccupy);
            orderBoardBoard = go.GetComponent<OrderBoard>();    
        }
        orderBoardBoard.Info = _orderMealInfo;
        orderBoardBoard.gameObject.SetActive(true);
    }

    public void HideOrderBoard()
    {
        orderBoardBoard?.gameObject.SetActive(false);
    }

    public override void ToDark()
    {
        base.ToDark();
        // for (int i = 0; i < _orderNode.childCount; i++)
        // {
        //     var sr = _orderNode.GetChild(i).GetComponent<SpriteRenderer>();
        //     sr.sortingOrder -= 0;
        // }        
    }

    public override void ToLight()
    {
        base.ToLight();
        // for (int i = 0; i < _orderNode.childCount; i++)
        // {
        //     var sr = _orderNode.GetChild(i).GetComponent<SpriteRenderer>();
        //     sr.sortingOrder = Mathf.Abs(sr.sortingOrder);
        // }
    }
    
    public virtual bool CanOrder()
    {
        var limit = DataProviderModule.Instance.OrderCountLimit();
        return _npcData.TodayOrderCount <= limit;
    }

    public bool IsWaitForOrder()
    {
        return CurBehaviour.BehaviourID == behaviour.WaitOrder;
    }

    public void RemoveSaidBubble(int chatId)
    {
        for (int i = 0; i < _saidBubbles.Count; i++)
        {
            var one = _saidBubbles[i];

            if (one.ID == chatId)
            {
                _saidBubbles.RemoveAt(i);
                break;    
            }
        }
    }

    public override void ClearDailyData()
    {
        base.ClearDailyData();
        _npcData.TodayOrderCount = 0;
        UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
    }

    public override void ClearWeeklyData()
    {
        base.ClearWeeklyData();
        _npcData.AccumulateFriendAtWeek = 0;
        UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
    }


    public override void PlayIdleAnimation()
    {
        PlayAnimation(behaviour.WaitReply);
    }
    

    public override void PlayTalkAnimation()
    {
        PlayAnimation(behaviour.Talk);
    }
    

    public override void PlayAnimation(behaviour behaviourId)
    {
        switch (behaviourId)
        {
            case behaviour.Eating:
                _animator.Play("idle");
                break;
            case behaviour.Talk:
                _animator.Play("talk");
                break;
            case behaviour.WaitOrder:
                _animator.Play("idle");
                break;
            case behaviour.WaitReply:
                _animator.Play("idle");
                break;
            case behaviour.Comment:
                _animator.Play("idle");
                break;
            default:
                _animator.Play("idle");
                break;
        }
    }
}