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
    // public SpriteRenderer Sprite => _spriteRenderer;
    // private SpriteRenderer _spriteRenderer;
    // private Transform _emojiNode;
    //
    // public Transform EmojiNode => _emojiNode;

    // public CharacterBaseInfo TBBaseInfo => _baseInfo;

    // public int SeatOccupy
    // {
    //     get => _seatOccupy;
    //     set => _seatOccupy = _seatOccupy | (1 << value);
    // }
    //
    // private int _seatOccupy;

    // public string CharacterName => _baseInfo.Name;

    //好感度
    // public int Friendliness => _npcData.FriendlyValue;
    // private NPCTableData _npcData;

    // public int CharacterId => _baseInfo.Id;
    // private CharacterBaseInfo _baseInfo;
    
    //表格配置数据
    private List<CharacterBubble> _mainLineBubbleTB;
    private List<CharacterBubble> _talkBubbleTB;
    private List<CharacterBubble> _orderBubbleTB;
    // private CharacterBubble _commentBubbleTB;
    
    //记录当前已经生成的chatid
    private List<DialogueTitleInfo> _saidBubbles;

    public int SaidBubbleNum => _saidBubbles.Count;
    // private int curCommentChatId;
    // private int curMainLineChatId;
    // private List<int> curTalkChatId;
    // private int curOrderChatId;
    
    // private int curOrderMenuId;
    
    //收到的料理
    private CookResult _receivedFood = null;
    //记录在对话过程中产生的订单
    // public int DialogueOrder
    // {
    //     set => curOrderMenuId = value;
    // }

    public OrderMealInfo CurOrderInfo
    {
        get => _orderMealInfo;
        set
        {
            _orderMealInfo = value;
            createOrderBoard();
        }
    }
    private OrderMealInfo _orderMealInfo;
    
    private Clocker _clocker;
    private int foodScore;
    
    public Transform OrderNode => _orderNode;
    protected Transform _orderNode;
    
    public override void InitCharacter(CharacterBaseInfo info)
    {
        // _baseInfo = info;
        // _spriteRenderer = GetComponent<SpriteRenderer>();
        // _emojiNode = transform.Find("EmojiNode");
        // LoadCharacterSprite();
        base.InitCharacter(info);
        _orderNode = transform.Find("OrderNode");
        _saidBubbles = new(10);
        // curCommentChatId = 0;
        // curTalkChatId = new List<int>(5);
        // curOrderChatId = 0;
        // curMainLineChatId = 0;
        // curOrderMenuId = 0;
        foodScore = 0;
        _clocker = UniModule.GetModule<Clocker>();
       
    }

    public override void ReleaseCharacter()
    {
        base.ReleaseCharacter();
        // _baseInfo = null;
        // _spriteRenderer.sprite = null;
        // _spriteRenderer = null;
        _saidBubbles.Clear();
        // curCommentChatId = 0;
        // curTalkChatId = null;
        // curOrderChatId = 0;
        // curMainLineChatId = 0;
        foodScore = 0;
        // curOrderMenuId = 0;
        UnLoadTableData();
        UnLoadDataBase();
        for (int i = 0; i < _orderNode.childCount;i++)
        {
            var one = _orderNode.GetChild(i);
            Destroy(one.gameObject);
        }
        Destroy(gameObject);
    }

    // private async void LoadCharacterSprite()
    // {
    //     var handler = YooAssets.LoadAssetAsync<Sprite>(_baseInfo.ResPath);
    //     await handler.ToUniTask();
    //     var sp = handler.AssetObject as Sprite;
    //     _spriteRenderer.sprite = sp;
    // }
    // private void UnLoadTableData()
    // {
    //     _mainLineBubbleTB?.Clear();
    //     _mainLineBubbleTB = null;
    //     
    //     _talkBubbleTB?.Clear();
    //     _talkBubbleTB = null;
    //     
    //     _orderBubbleTB?.Clear();
    //     _orderBubbleTB = null;
    // }
    protected override void LoadTableData()
    {
        _mainLineBubbleTB = new(10);
        _talkBubbleTB = new(10);
        _orderBubbleTB = new(10);
        
        if (_baseInfo.Soul == 0) return;
        
        var dataList = DataProviderModule.Instance.GetCharacterBubbleList(CharacterId);
        for (int i = 0; i < dataList.Count; i++)
        {
            switch (dataList[i].BubbleType)
            {
                case bubbleType.Talk:
                    _talkBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.Order:
                    _orderBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.MainLine:
                    _mainLineBubbleTB.Add(dataList[i]);
                    break;
                // case bubbleType.Comment:
                //     _commentBubbleTB = dataList[i];
                //     break;
            }
        }
    }

    // private void UnLoadDataBase()
    // {
    //     UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
    //     _npcData = null;
    // }
    
    // private void LoadDataBase()
    // {
    //     var userInfoModule = UniModule.GetModule<UserInfoModule>();
    //     _npcData = userInfoModule.NPCData(CharacterId);
    //     var waitingInfo = userInfoModule.GetWaitingCharacter(CharacterId);
    //     if(waitingInfo != null)_seatOccupy = waitingInfo.SeatOccupy;
    // }

    //增加好感度
    // public void AddFriendly(int num)
    // {
    //     _npcData.FriendlyValue += num;
    //     UserInfoModule.Instance.UpdateNPCData(CharacterId);
    // }
    //
    // public void AddAppearCount()
    // {
    //     _npcData.AppearCount += 1;
    //     UserInfoModule.Instance.UpdateNPCData(CharacterId);
    // }
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
            // if (one.PreCondition == 0)
            // {
            //     if (!UserInfoModule.Instance.HaveReadDialogueId(one.PreCondition))
            //     {
            //         return one.Id;
            //     }
            //
            //     continue;
            // }

            // if (!UserInfoModule.Instance.HaveReadDialogueId(one.PreCondition)) continue;
            // if (!UserInfoModule.Instance.HaveReadDialogueId(one.Id))
            // {
            //     return one.Id;
            // }

        }

        return -1;
    }

    private int generateTalk()
    {
        int index = Random.Range(0, _talkBubbleTB.Count);

        var choiced = _talkBubbleTB[index];
        var isEnough = Friendliness >= choiced.FriendValue.StartValue &&
                       Friendliness <= choiced.FriendValue.EndValue;

        if (!isEnough) return -1;
        
        var isCondition = checkPreCondition(choiced.PreCondition);
        if(!isCondition) return -1;


        if (!checkWeekday(choiced.WeekDay)) return -1;
        
        return choiced.Id;
    }

    List<int> _orderPool = new List<int>(10);
    private int generateOrderChatId()
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
                _orderPool.Add(one.Id);
        }

        if (_orderPool.Count <= 0) return -1;

        var index = Random.Range(0, _orderPool.Count);
        
        CurOrderInfo ??= new OrderMealInfo();
        CurOrderInfo.MenuId = _orderBubbleTB[index].MenuId;
        CurOrderInfo.CharacterId = CharacterId;
        
        return _orderBubbleTB[index].Id;
    }

    private bool checkWeekday(List<WeekDay> weekDays)
    {
        if (weekDays.Count == 1)
        {
            return weekDays[0] == WeekDay.AllWeekDay;
        }
        
        foreach (var one in weekDays)
        {
            if (one == _clocker.NowDateTime.DayOfWeek)
                return true;
        }

        return false;
    }

    // int haveCommentChatId()
    // {
    //     var result = _saidBubbles.FirstOrDefault(one=>one.Type==bubbleType.Comment);
    //     return result?.ID ?? 0;
    // }

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
        var result = _saidBubbles.FirstOrDefault(one=>one.Type==bubbleType.Order);
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
        if (haveTalkChatId() < 5)
        {
            var info = new CharacterSaidInfo
            {
                CharacterId = CharacterId
            };
            info.ChatId = generateTalk();
            if (info.ChatId > 0)
            {
                _saidBubbles.Add(new DialogueTitleInfo(){ID = info.ChatId,Type = bubbleType.Talk});
                EventModule.Instance.CharBubbleTopic.OnNext(info);
                return true;    
            }
        }

        return false;
    }

    public bool GenerateOrder()
    {
        //下单
        if (haveOrderChatId() > 0) return true;
        var info = new CharacterSaidInfo
        {
            CharacterId = CharacterId
        };
        info.ChatId = generateOrderChatId();
        if (info.ChatId <= 0) return false;
        _saidBubbles.Add(new DialogueTitleInfo(){ID = info.ChatId,Type = bubbleType.Order});
        _npcData.TodayOrderCount += 1;
        EventModule.Instance.CharBubbleTopic.OnNext(info);
        return true;
    }

    // public void ToDark()
    // {
    //     _spriteRenderer.color = Color.gray;
    // }
    //
    // public void ToLight()
    // {
    //     _spriteRenderer.color = Color.white;
    // }

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
            storageBehaviour.SetValue("$mealScore",_receivedFood.Score);
            storageBehaviour.SetValue("$matchTag",isMatchTags());
            _receivedFood = null;
        }
        storageBehaviour.SetValue("$withPartner",(_npcData.PartnerId > 0));
    }

    // public void DialogueOrder(int menuId)
    // {
    //     _dialogueOrderId = menuId;
    // }
    
    public void ReceiveFood(CookResult food)
    {
        _receivedFood = food;
        // curCommentChatId = _commentBubbleTB.Id;
        
        // CharacterSaidInfo info = new CharacterSaidInfo();
        // info.CharacterId = CharacterId;
        // info.ChatId = 0001;//todo 根据策划文档修改
        // EventModule.Instance.CharBubbleTopic.OnNext(info);
        // commentFood(food);
        CurBehaviour = new CharacterEating();
    }

    private void commentFood(CookResult food)
    {
        foodScore = 0;
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
    }

    private bool isMatchTags()
    {
        if (CurOrderInfo.flavor.Count <= 0) return false;

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
    
    private async void createOrderBoard()
    {
        var prefab = await LoadPrefab("Assets/GameRes/Prefabs/OrderBoard.prefab");
        var go = GameObject.Instantiate<GameObject>(prefab, _orderNode);
        var orderBoardBoard = go.GetComponent<OrderBoard>();
        orderBoardBoard.Info = _orderMealInfo;
    }

    public override void ToDark()
    {
        base.ToDark();
        for (int i = 0; i < _orderNode.childCount; i++)
        {
            var sr = _orderNode.GetChild(i).GetComponent<SpriteRenderer>();
            sr.sortingOrder -= 0;
        }        
    }

    public override void ToLight()
    {
        base.ToLight();
        for (int i = 0; i < _orderNode.childCount; i++)
        {
            var sr = _orderNode.GetChild(i).GetComponent<SpriteRenderer>();
            sr.sortingOrder = Mathf.Abs(sr.sortingOrder);
        }
    }

    public void PlayEating()
    {
        
    }

    public bool CanOrder()
    {
        var limit = DataProviderModule.Instance.OrderCountLimit();
        return _npcData.TodayOrderCount <= limit;
    }
    
    
}