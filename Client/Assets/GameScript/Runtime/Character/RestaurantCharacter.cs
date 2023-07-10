using System.Collections;
using System.Collections.Generic;
using cfg.character;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;
using YooAsset;


public class RestaurantCharacter : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Transform _emojiNode;

    public Vector3 EmojiWorldPosition
    {
        get => _emojiNode.transform.position;
    }
    public cfg.character.CharacterBaseInfo TBBaseInfo => _baseInfo;
    
    public int SeatIndex
    {
        get => _seatIndex;
        set => _seatIndex = value;
    }
    private int _seatIndex;
    
    public string CharacterName => _baseInfo.Name;

    //好感度
    public int Friendliness => _npcData.FriendlyValue;
    private NPCTableData _npcData;

    public int CharacterId => _baseInfo.Id;
    private CharacterBaseInfo _baseInfo;

    private List<cfg.character.CharacterBubble> _bubbleTB;
    private Queue<int> _chatIdQueue;
    public CharacterBehaviour CurBehaviour
    {
        get => _behaviour;
        set
        {
            if (_behaviour == null)
            {
                _behaviour = value;
                _behaviour.Enter(this);
            }
            else
            {
                _behaviour.Exit();
                _behaviour = value;
                _behaviour.Enter(this);
            }
        }
    }
    private CharacterBehaviour _behaviour;
    private RestaurantEnter _restaurant;

    public void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _emojiNode = transform.Find("EMojiNode");
        _chatIdQueue = new Queue<int>();
        // LoadCharacterSprite();
        LoadTableData();
        LoadDataBase();
    }
    
    // private async void LoadCharacterSprite()
    // {
    //     var handler = YooAssets.LoadAssetAsync<Sprite>(_baseInfo.ResPath);
    //     await handler.ToUniTask();
    //     var sp = handler.AssetObject as Sprite;
    //     _spriteRenderer.sprite = sp;
    // }

    private void LoadTableData()
    {
        _bubbleTB = new List<cfg.character.CharacterBubble>(50);
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var dataList = dataProvider.DataBase.TbCharacterBubble.DataList;
        for (int i = 0; i < dataList.Count; i++)
        {
            if(dataList[i].NpcId != CharacterId)continue;
            _bubbleTB.Add(dataList[i]);
        }
    }

    private void LoadDataBase()
    {
        var userInfoModule = UniModule.GetModule<UserInfoModule>();
        _npcData = userInfoModule.NpcData(CharacterId);
    }

    //增加好感度
    public void AddFriendly(int num)
    {
        _npcData.FriendlyValue += num;
    }

    public int HaveChatId()
    {
        if (_chatIdQueue.Count > 0)
        {
            return _chatIdQueue.Dequeue();
        }
        
        var friendliness = _npcData.FriendlyValue;
        for (int i = 0; i < _bubbleTB.Count; i++)
        {
            if (friendliness >= _bubbleTB[i].FriendValue.StartValue &&
                friendliness <= _bubbleTB[i].FriendValue.EndValue)
            {
                return _bubbleTB[i].Id;
            }
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
        storageBehaviour.SetValue("$friendliness",_npcData.FriendlyValue);
        storageBehaviour.SetValue("$appear",_npcData.AppearCount);
    }

    public void ReceiveFood(CookResult food)
    {
        int score = 0;
        // 1 根据完成度评价
        
        // 2 根据标签评价
        
        // 3 计算总分
        
        // 4 给出对应对话气泡
        //5 如果第一次出现的对话就 直接进入对话界面
        _chatIdQueue.Enqueue(10004004);
        var existed = UserInfoModule.Instance.HaveReadDialogueId(10004004);
        if (existed)
        {
            EventModule.Instance.CharBubbleTopic.OnNext(this);    
        }
        else
        {
            EventModule.Instance.CharDialogTopic.OnNext(this);
        }
    }

    public void ShowEmoji(string emoji)
    {
        
    }
    

}
