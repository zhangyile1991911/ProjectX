using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
    
    public string CharacterName => _name;
    private string _name;

    //好感度
    public int Like => _like;
    private int _like;

    public int CharacterId => _cid;
    private int _cid;

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

    public void InitCharacter(int cid,string characterName)
    {
        _cid = cid;
        _name = characterName;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        LoadCharacterSprite();
    }
    
    public async void LoadCharacterSprite()
    {
        var spName = ZString.Format("Assets/GameRes/Picture/Character/{0}.png", _name);
        var handler = YooAssets.LoadAssetAsync<Sprite>(spName);
        await handler.ToUniTask();
        var sp = handler.AssetObject as Sprite;
        _spriteRenderer.sprite = sp;
    }

    //增加好感度
    public void AddLike()
    {
        
    }

    //减少好感度
    public void ReduceLike()
    {
        
    }
    
    public int HaveChatId()
    {
        return UnityEngine.Random.Range(0,5);
    }
}
