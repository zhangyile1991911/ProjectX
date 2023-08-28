using System;
using System.Collections;
using System.Collections.Generic;
using cfg.character;
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
    public string CharacterName => _baseInfo.Name;
    public int SeatOccupy
    {
        get => _npcData.SeatIndex;
        set
        {
            _npcData.SeatIndex = value;
            UserInfoModule.Instance.UpdateNPCData(CharacterId);
        }
    }

    public bool HaveSoul => _baseInfo.Soul > 0;
    protected int _seatOccupy;
    
    //好感度
    public int Friendliness => _npcData.FriendlyValue;
    
    protected NPCTableData _npcData;
    
    public int PartnerID
    {
        get => _npcData.PartnerId;
        set
        {
            _npcData.PartnerId = value;
            UserInfoModule.Instance.UpdateNPCData(_npcData.Id);
        }
    }

    public int PatientValue
    {
        get => _npcData.patient;
    }
    
    public behaviour BehaviourID => (behaviour)_npcData.Behaviour;
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
                _npcData.Behaviour = (int)value.BehaviourID;
                UserInfoModule.Instance.UpdateNPCData(CharacterId);
                
            }
        }
    }

    protected CharacterBehaviour _behaviour;
    
    private IDisposable halfSecondTImer;

    protected List<AssetOperationHandle> loadResHandlers;

    protected bool isActive;

    // protected Animation _animation;
    public virtual void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // _animation = GetComponent<Animation>();
        _emojiNode = transform.Find("EmojiNode");
        _chatNode = transform.Find("ChatNode");
        isActive = true;
        halfSecondTImer = Observable.Interval(TimeSpan.FromSeconds(0.25f))
            .Where(_=>isActive)
            .Subscribe(_ =>
        {
            CurBehaviour?.Update();
        });
        loadResHandlers = new List<AssetOperationHandle>(10);
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
    }
    
    protected virtual void LoadTableData()
    {
        
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
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }
    
    public virtual void ToDark()
    {
        // _spriteRenderer.color = Color.gray;
        _spriteRenderer.sortingOrder -= 0 ;
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
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
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
        switch (behaviourId)
        {
            case behaviour.Eating:
                if (_spriteRenderer.sprite == null)
                {
                    _spriteRenderer.sprite = _spriteDict["eat1"];
                }
                else if (_spriteRenderer.sprite.name == "eat1")
                {
                    _spriteRenderer.sprite = _spriteDict["eat2"];    
                }
                else
                {
                    _spriteRenderer.sprite = _spriteDict["eat1"];
                }
                break;
            case behaviour.Talk:
                _spriteRenderer.sprite = _spriteDict["angry"];
                break;
            case behaviour.Waiting:
                _spriteRenderer.sprite = _spriteDict["happy"];
                break;
            default:
                _spriteRenderer.sprite = _spriteDict["waiting"];       
                break;
        }
    }
    
    // private void Update()
    // {
    //     CurBehaviour?.Update();
    // }
    
}