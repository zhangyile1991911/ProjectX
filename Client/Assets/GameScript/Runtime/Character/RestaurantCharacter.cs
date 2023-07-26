using System;
using System.Collections;
using System.Collections.Generic;
using cfg.character;
using cfg.common;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Yarn.Unity;
using YooAsset;
using Random = UnityEngine.Random;


public class RestaurantCharacter : MonoBehaviour
{
    public SpriteRenderer Sprite => _spriteRenderer;
    private SpriteRenderer _spriteRenderer;
    private Transform _emojiNode;

    public Transform EmojiNode => _emojiNode;

    public CharacterBaseInfo TBBaseInfo => _baseInfo;

    public int SeatOccupy
    {
        get => _seatOccupy;
        set => _seatOccupy = _seatOccupy | (1 << value);
    }

    private int _seatOccupy;

    public string CharacterName => _baseInfo.Name;

    //好感度
    public int Friendliness => _npcData.FriendlyValue;
    private NPCTableData _npcData;

    public int CharacterId => _baseInfo.Id;
    private CharacterBaseInfo _baseInfo;
    
    //表格配置数据
    private List<CharacterBubble> _mainLineBubbleTB;
    private List<CharacterBubble> _talkBubbleTB;
    private List<CharacterBubble> _orderBubbleTB;
    private CharacterBubble _commentBubbleTB;
    
    //记录当前已经生成的chatid
    private int curCommentChatId;
    private int curMainLineChatId;
    private List<int> curTalkChatId;
    private int curOrderChatId;

    private int curOrderMenuId;
    //收到的料理
    private CookResult _receivedFood = null;
    //记录在对话过程中产生的订单
    public int DialogueOrder
    {
        set => curOrderMenuId = value;
    }
    
    public CharacterBehaviour CurBehaviour
    {
        get => _behaviour;
        set
        {
            if (_behaviour == null)
            {
                _behaviour = value;
                _behaviour?.Enter(this);
            }
            else
            {
                _behaviour.Exit();
                _behaviour = value;
                _behaviour?.Enter(this);
            }
        }
    }

    private CharacterBehaviour _behaviour;
    private Clocker _clocker;
    private int foodScore;

    public void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _emojiNode = transform.Find("EmojiNode");
        curCommentChatId = 0;
        // LoadCharacterSprite();
        curTalkChatId = new List<int>(5);
        curOrderChatId = 0;
        curMainLineChatId = 0;
        curOrderMenuId = 0;
        foodScore = 0;
        _clocker = UniModule.GetModule<Clocker>();
        LoadTableData();
        LoadDataBase();
    }

    public void ReleaseCharacter()
    {
        _baseInfo = null;
        _spriteRenderer.sprite = null;
        _spriteRenderer = null;
        curCommentChatId = 0;
        curTalkChatId = null;
        curOrderChatId = 0;
        curMainLineChatId = 0;
        foodScore = 0;
        curOrderMenuId = 0;
        UnLoadTableData();
        UnLoadDataBase();
        Destroy(gameObject);
    }

    // private async void LoadCharacterSprite()
    // {
    //     var handler = YooAssets.LoadAssetAsync<Sprite>(_baseInfo.ResPath);
    //     await handler.ToUniTask();
    //     var sp = handler.AssetObject as Sprite;
    //     _spriteRenderer.sprite = sp;
    // }
    private void UnLoadTableData()
    {
        _mainLineBubbleTB?.Clear();
        _mainLineBubbleTB = null;
        
        _talkBubbleTB?.Clear();
        _talkBubbleTB = null;
        
        _orderBubbleTB?.Clear();
        _orderBubbleTB = null;
    }
    private void LoadTableData()
    {
        _mainLineBubbleTB = new(10);
        _talkBubbleTB = new(10);
        _orderBubbleTB = new(10);
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var dataList = dataProvider.GetCharacterBubbleList(CharacterId);
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
                case bubbleType.Comment:
                    _commentBubbleTB = dataList[i];
                    break;
            }
        }
    }

    private void UnLoadDataBase()
    {
        UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
        _npcData = null;
    }
    
    private void LoadDataBase()
    {
        var userInfoModule = UniModule.GetModule<UserInfoModule>();
        _npcData = userInfoModule.NPCData(CharacterId);
    }

    //增加好感度
    public void AddFriendly(int num)
    {
        _npcData.FriendlyValue += num;
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }

    public void AddAppearCount()
    {
        _npcData.AppearCount += 1;
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
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
            if (one.PreCondition == 0)
            {
                if (!UserInfoModule.Instance.HaveReadDialogueId(one.PreCondition))
                {
                    return one.Id;
                }

                continue;
            }

            if (!UserInfoModule.Instance.HaveReadDialogueId(one.PreCondition)) continue;
            if (!UserInfoModule.Instance.HaveReadDialogueId(one.Id))
            {
                return one.Id;
            }

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
        
        var isCondition = choiced.PreCondition == 0 || 
                          UserInfoModule.Instance.HaveReadDialogueId(choiced.PreCondition);

        if (!isCondition) return -1;
        
        if (checkWeekday(choiced.WeekDay))
            return choiced.Id;

        return -1;
    }

    private int generateOrderChatId()
    {
        List<int> result = new List<int>(10);
        foreach (var one in _orderBubbleTB)
        {
            var isEnough = Friendliness >= one.FriendValue.StartValue &&
                           Friendliness <= one.FriendValue.EndValue;
            if(!isEnough)continue;
            
            var isCondition = one.PreCondition == 0 || 
                              UserInfoModule.Instance.HaveReadDialogueId(one.PreCondition);
            if(!isCondition)continue;
            
            if (checkWeekday(one.WeekDay))
                result.Add(one.Id);
        }

        if (result.Count <= 0) return -1;

        var index = Random.Range(0, result.Count);
        curOrderMenuId = _orderBubbleTB[index].MenuId;
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
            if ((DayOfWeek)one == _clocker.NowDateTime.DayOfWeek)
                return true;
        }

        return false;
    }
    
    public int GenerateChatId()
    {
        if (curCommentChatId > 0)
        {
            var tmp = curCommentChatId;
            curCommentChatId = 0;
            return tmp;
        }
        //气泡生成规则
        //1 主线剧情
        if (curMainLineChatId <= 0)
        {
            curMainLineChatId = generateMainLine();
            if (curMainLineChatId > 0)
            {
                return curMainLineChatId;
            }
        }

        //普通重复的对话
        if (curTalkChatId.Count < 5)
        {
            int chatId = generateTalk();
            if (chatId > 0)
            {
                curTalkChatId.Add(chatId);
                return chatId;    
            }
        }
        
        //下单
        if (curOrderChatId <= 0)
        {
            curOrderChatId = generateOrderChatId();
            return curOrderChatId;
        }

        return -1;
    }

    public void ToDark()
    {
        _spriteRenderer.color = Color.gray;
    }

    public void ToLight()
    {
        _spriteRenderer.color = Color.white;
    }

    public override bool Equals(object other)
    {
        var otherCharacter = other as RestaurantCharacter;
        return otherCharacter.CharacterId == this.CharacterId;
    }

    public void InjectVariable(VariableStorageBehaviour storageBehaviour)
    {
        storageBehaviour.SetValue("$friendliness", _npcData.FriendlyValue);
        // storageBehaviour.SetValue("$appear", _npcData.AppearCount);
        
        if (_receivedFood != null)
        {
            storageBehaviour.SetValue("$orderedId",curOrderMenuId);
            storageBehaviour.SetValue("$foodscore",foodScore);
            storageBehaviour.SetValue("$cookFoodId",_receivedFood.menuId);
            _receivedFood = null;
        }
    }

    // public void DialogueOrder(int menuId)
    // {
    //     _dialogueOrderId = menuId;
    // }
    
    public void ReceiveFood(CookResult food)
    {
        _receivedFood = food;
        curCommentChatId = _commentBubbleTB.Id;
        EventModule.Instance.CharBubbleTopic.OnNext(curCommentChatId);
        commentFood(food);
    }

    private void commentFood(CookResult food)
    {
        foodScore = 0;
        //时间内完成获得50分基础分
        foodScore = food.CompletePercent >= 1.0 ? 50 : 0;
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
}