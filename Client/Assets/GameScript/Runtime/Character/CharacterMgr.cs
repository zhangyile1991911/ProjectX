using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using YooAsset;

public class CharacterMgr : SingletonModule<CharacterMgr>
{
    private Dictionary<string,RestaurantRoleBase> _characters;
    public List<RestaurantRoleBase> Characters => _characters.Values.ToList();
    public override void OnCreate(object createParam)
    {
        _characters = new Dictionary<string, RestaurantRoleBase>();
        base.OnCreate(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        // Debug.Log("CharacterMgr");
        // foreach (var one in _characters.Values)
        // {
        //     one.IsTimeToLeave();
        // }
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
        
        if (_characters.TryGetValue(tbCharacter.Name, out var character))
        {
            return character;
        }
        
        var handle = YooAssets.LoadAssetAsync<GameObject>(tbCharacter.ResPath);
        await handle.ToUniTask();
    
        var go = handle.AssetObject as GameObject;

        var ins_character = GameObject.Instantiate(go);
        character = ins_character.GetComponent<RestaurantRoleBase>();
        
        character.InitCharacter(tbCharacter);
        
        _characters.Add(tbCharacter.Name,character);

        return character;
    }

    public RestaurantRoleBase GetCharacterByName(string name)
    {
        if (_characters.TryGetValue(name, out var character))
        {
            return character;
        }

        return null;
    }

    public RestaurantRoleBase GetCharacterById(int cid)
    {
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var tbCharacter = dataProvider.GetCharacterBaseInfo(cid);
        return GetCharacterByName(tbCharacter.Name);
    }

    public void RemoveCharacter(RestaurantRoleBase character)
    {
        _characters.Remove(character.CharacterName);
        character.ReleaseCharacter();
    }
    
    public void RemoveCharacter(int cid)
    {
        var character = GetCharacterById(cid);
        RemoveCharacter(character);
    }

    public void ClearAllCharacter()
    {
        foreach (var cha in _characters.Values)
        {
            cha.ReleaseCharacter();
        }
        _characters.Clear();
    }
    
}
