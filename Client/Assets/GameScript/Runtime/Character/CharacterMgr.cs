using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using YooAsset;

public class CharacterMgr : SingletonModule<CharacterMgr>
{
    private Dictionary<string,RestaurantRoleBase> _characterName;
    private Dictionary<int, RestaurantRoleBase> _characterId;
    private List<RestaurantRoleBase> _characterList;
    public List<RestaurantRoleBase> Characters => _characterList;
    public override void OnCreate(object createParam)
    {
        base.OnCreate(this);
        _characterName = new Dictionary<string, RestaurantRoleBase>(10);
        _characterId = new Dictionary<int, RestaurantRoleBase>(10);
        _characterList = new List<RestaurantRoleBase>(10);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ClearAllCharacter();
    }

    public async UniTask<RestaurantRoleBase> CreateCharacter(int cid)
    {
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var tbCharacter = dataProvider.GetCharacterBaseInfo(cid);
        if(tbCharacter == null)return null;
        
        if (_characterName.TryGetValue(tbCharacter.Name, out var character))
        {
            return character;
        }
        
        var handle = YooAssets.LoadAssetAsync<GameObject>(tbCharacter.PrefabPath);
        await handle.ToUniTask();
    
        var go = handle.AssetObject as GameObject;

        var ins_character = GameObject.Instantiate(go);
        character = ins_character.GetComponent<RestaurantRoleBase>();
        
        character.InitCharacter(tbCharacter);
        
        addCharacter(character);

        return character;
    }

    private void addCharacter(RestaurantRoleBase role)
    {
        if (role == null) return;
        if (!_characterName.ContainsKey(role.CharacterName))
        {
            _characterName.Add(role.CharacterName,role);
            _characterId.Add(role.CharacterId,role);
            _characterList.Add(role);    
        }
    }

    public RestaurantRoleBase GetCharacterByName(string name)
    {
        if (_characterName.TryGetValue(name, out var character))
        {
            return character;
        }

        return null;
    }

    public RestaurantRoleBase GetCharacterById(int cid)
    {
        if (_characterId.TryGetValue(cid, out var character))
        {
            return character;
        }

        return null;
    }

    public void RemoveCharacter(RestaurantRoleBase character)
    {
        removeCharacter(character);
    }
    
    public void RemoveCharacter(int cid)
    {
        var character = GetCharacterById(cid);
        removeCharacter(character);
    }

    public void RemoveCharacter(string name)
    {
        var character = GetCharacterByName(name);
        removeCharacter(character);
    }

    private void removeCharacter(RestaurantRoleBase character)
    {
        if (character == null) return;
        
        _characterName.Remove(character.CharacterName);
        _characterId.Remove(character.CharacterId);
        _characterList.Remove(character);
        character.ReleaseCharacter();
    }

    public void ClearAllCharacter()
    {
        foreach (var cha in _characterName.Values)
        {
            cha.ReleaseCharacter();
        }
        _characterName.Clear();
        _characterId.Clear();
        _characterList.Clear();
    }
    
}
