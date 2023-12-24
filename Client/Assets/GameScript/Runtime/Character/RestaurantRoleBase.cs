using System;
using System.Collections;
using System.Collections.Generic;
using cfg.character;
using Codice.Client.BaseCommands.Merge.Restorer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Yarn.Unity;
using YooAsset;

public abstract class RestaurantRoleBase : MonoBehaviour
{
    public SpriteRenderer Sprite => _spriteRenderer;
    protected SpriteRenderer _spriteRenderer;
    public Transform EmojiNode => _emojiNode;
    protected Transform _emojiNode;
    public Transform ChatNode => _chatNode;
    protected Transform _chatNode;
    
    public int CharacterId => _baseInfo.Id;
    public CharacterBaseInfo TBBaseInfo => _baseInfo;
    protected  CharacterBaseInfo _baseInfo;

    public ScheduleGroup TbsScheduleGroup => _scheduleGroup;
    protected ScheduleGroup _scheduleGroup;
    public int BehaviourGroupId => _scheduleGroup.BehaviourId;
    public string CharacterName => _baseInfo.Name;
    public int SeatOccupy
    {
        get => _seatOccupy;
        set => _seatOccupy = value;
    }

    public bool HaveSoul => _baseInfo.Soul > 0;
    protected int _seatOccupy;
    
    //好感度
    public int Friendliness => _npcData.FriendlyValue;
    
    protected NPCTableData _npcData;
    
    
    public HashSet<int> Partners => _partnerIds;
    private HashSet<int> _partnerIds;

    public int PatientValue
    {
        get => _npcData.patient;
    }
    
    // public behaviour BehaviourID => (behaviour)_npcData.Behaviour;
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
                if (_behaviour != null)
                {
                    _behaviour.Enter(this);
                    // _npcData.Behaviour = (int)value.BehaviourID;    
                }
                // else
                // {
                //     // _npcData.Behaviour = 0;
                // }
                // UserInfoModule.Instance.UpdateNPCData(CharacterId);
                
            }
        }
    }

    protected CharacterBehaviour _behaviour;
    
    private IDisposable halfSecondTImer;

    protected List<AssetOperationHandle> loadResHandlers;

    protected bool isActive;

    [HideInInspector]
    public RestaurantEnter Restaurant;
    // protected Animation _animation;
    public virtual void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // _animation = GetComponent<Animation>();
        _emojiNode = transform.Find("EmojiNode");
        _chatNode = transform.Find("ChatNode");
        isActive = true;

        halfSecondTImer = Clocker.Instance.Topic.Where(_ => isActive).Subscribe(_ => { CurBehaviour?.Update(); });
        loadResHandlers = new List<AssetOperationHandle>(10);
        _partnerIds = new(5);
        LoadTableData();
        LoadDataBase();
    }

    public virtual void ReleaseCharacter()
    {
        _baseInfo = null;
        _spriteRenderer.sprite = null;
        _spriteRenderer = null;
        halfSecondTImer?.Dispose();
        halfSecondTImer = null;
        Restaurant = null;
        spriteAtlasHandle?.Dispose();
        spriteAtlasHandle = null;
        _spriteDict.Clear();
        
        UnLoadDataBase();
        UnLoadTableData();
        foreach (var handler in loadResHandlers)
        {
            handler.Release();
            handler.Dispose();
        }
        loadResHandlers.Clear();
        _partnerIds.Clear();
    }
    
    protected virtual void LoadTableData()
    {
        //当前行动组
        LoadScheduleGroup();
    }

    protected void LoadScheduleGroup()
    {
        _scheduleGroup = CharacterScheduler.Instance.CharacterScheduleId(CharacterId);
        if (_scheduleGroup == null)
        {
            Debug.Log($"RestaurantRoleBase LoadTableData CharacterId = {CharacterId} CharacterScheduler == null");
            return;
        }
        foreach (var id in _scheduleGroup.PartnerId)
        {
            AddPartner(id);
        }
    }

    protected virtual void UnLoadTableData()
    {
        _baseInfo = null;
        
    }
    
    protected virtual void LoadDataBase()
    {
        var userInfoModule = UniModule.GetModule<UserInfoModule>();
        _npcData = userInfoModule.NPCData(CharacterId);
    }

    protected virtual void UnLoadDataBase()
    {
        UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
        _npcData = null;
    }

    public virtual void InjectVariableToDialogue(VariableStorageBehaviour storageBehaviour)
    {
        storageBehaviour.SetValue("$friendliness", _npcData.FriendlyValue);
    }
    
    public virtual void AddFriendly(int num)
    {
        if (num > 0)
        {
            var limit = DataProviderModule.Instance.FriendValLimit();
            int remain = limit - _npcData.AccumulateFriendAtWeek;
            if (num > remain)
            {
                num = remain;
            }
            _npcData.FriendlyValue += num;
            _npcData.AccumulateFriendAtWeek += num;
            //每次更新好感度 都可能会改变NPC行为
            _npcData.ScheduleId = CharacterScheduler.Instance.UpdateCharacterSchedule(_npcData.Id,
                _npcData.ScheduleId,
                _npcData.FriendlyValue);
            //更新日程表
            LoadScheduleGroup();
        }
        else
        {
            _npcData.FriendlyValue = _npcData.FriendlyValue + num < 0 ? 0 : _npcData.FriendlyValue + num;
        }
        
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }

    public virtual void AddAppearCount()
    {
        _npcData.AppearCount += 1;
        // UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }
    
    public virtual void ToDark()
    {
        // _spriteRenderer.color = Color.gray;
        _spriteRenderer.sortingOrder = 0 - _spriteRenderer.sortingOrder;
        isActive = false;
    }

    public virtual void ToLight()
    {
        // _spriteRenderer.color = Color.white;
        _spriteRenderer.sortingOrder = Mathf.Abs(_spriteRenderer.sortingOrder);
        isActive = true;
    }

    public void ResetPatient()
    {
        _npcData.patient = DataProviderModule.Instance.MaxPatientValue();
    }
    public void AttenuatePatient(int val)
    {
        var result = _npcData.patient - val;
        if (result < 0)
        {
            _npcData.patient = 0;
        }
        else
        {
            _npcData.patient = result;
        }
        // UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }
    
    
    protected async UniTask<GameObject> LoadPrefab(string path)
    {
        var go = YooAssets.LoadAssetAsync<GameObject>(path);
        loadResHandlers.Add(go);
        await go.ToUniTask(this);
        return go.AssetObject as GameObject;
    }

    protected SubAssetsOperationHandle spriteAtlasHandle;
    protected Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();
    protected void LoadSprite(string path)
    {
        spriteAtlasHandle = YooAssets.LoadSubAssetsSync<Sprite>(path);
        // loadResHandlers.Add(go);
        // await spriteAtlasHandle.ToUniTask(this);

        var arr = spriteAtlasHandle.GetSubAssetObjects<Sprite>();

        foreach (var one in arr)
        {
            _spriteDict.Add(one.name,one);
        }
        
    }
    
    public virtual void PlayAnimation(behaviour behaviourId)
    {
        
    }

    public virtual void ClearDailyData()
    {
        
    }

    public virtual void ClearWeeklyData()
    {
        
    }
    
    public void AddPartner(int partnerId)
    {
        if (partnerId <= 0) return;
        if (_partnerIds.Contains(partnerId)) return;
        _partnerIds.Add(partnerId);
    }
    
    // private void Update()
    // {
    //     CurBehaviour?.Update();
    // }
    
}