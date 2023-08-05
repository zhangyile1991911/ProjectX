using System;
using System.Collections;
using System.Collections.Generic;
using cfg.character;
using UniRx;
using UnityEngine;
using Yarn.Unity;

public abstract class RestaurantRoleBase : MonoBehaviour
{
    public SpriteRenderer Sprite => _spriteRenderer;
    protected SpriteRenderer _spriteRenderer;
    public Transform EmojiNode => _emojiNode;
    protected Transform _emojiNode;
    
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

    protected CharacterBehaviour _behaviour;
    
    private IDisposable halfSecondTImer;
    public virtual void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _emojiNode = transform.Find("EmojiNode");
        halfSecondTImer = Observable.Interval(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            CurBehaviour?.Update();
        });
        
        LoadTableData();
        LoadDataBase();
    }

    public virtual void ReleaseCharacter()
    {
        _baseInfo = null;
        _spriteRenderer.sprite = null;
        _spriteRenderer = null;
        halfSecondTImer?.Dispose();
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
        _npcData.FriendlyValue += num;
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }

    public virtual void AddAppearCount()
    {
        _npcData.AppearCount += 1;
        UserInfoModule.Instance.UpdateNPCData(CharacterId);
    }
    
    public virtual void ToDark()
    {
        _spriteRenderer.color = Color.gray;
    }

    public virtual void ToLight()
    {
        _spriteRenderer.color = Color.white;
    }

    // private void Update()
    // {
    //     CurBehaviour?.Update();
    // }
}
