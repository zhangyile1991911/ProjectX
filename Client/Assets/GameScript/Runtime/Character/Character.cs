using System.Collections;
using System.Collections.Generic;
using cfg.character;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;
using YooAsset;


public class Character : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public int SeatIndex
    {
        get => _seatIndex;
        set => _seatIndex = value;
    }
    private int _seatIndex;
    
    public string CharacterName => _baseInfo.Name;
    

    //好感度
    public int Friendliness => _friendliness;
    private int _friendliness;

    public int CharacterId => _baseInfo.Id;
    private CharacterBaseInfo _baseInfo;

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
    void Start()
    {
        
    }

    public void InitCharacter(CharacterBaseInfo info)
    {
        _baseInfo = info;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        LoadCharacterSprite();
    }
    
    private async void LoadCharacterSprite()
    {
        var handler = YooAssets.LoadAssetAsync<Sprite>(_baseInfo.ResPath);
        await handler.ToUniTask();
        var sp = handler.AssetObject as Sprite;
        _spriteRenderer.sprite = sp;
    }

    //增加好感度
    public void AddFriendly(int num)
    {
        
    }

    public int HaveChatId()
    {
        return UnityEngine.Random.Range(0,5);
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
        var otherCharacter = other as Character;
        return otherCharacter.CharacterId == this.CharacterId;
    }

    public void InjectVariable(VariableStorageBehaviour storageBehaviour)
    {
        storageBehaviour.SetValue("$friendliness",_friendliness);
        // storageBehaviour.SetValue("NPCName",_name);
    }
}
