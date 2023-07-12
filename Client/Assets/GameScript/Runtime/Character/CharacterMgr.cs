using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using YooAsset;

public class CharacterMgr : SingletonModule<CharacterMgr>
{
    private Dictionary<string,RestaurantCharacter> _characters;
    public override void OnCreate(object createParam)
    {
        _characters = new Dictionary<string, RestaurantCharacter>();
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
    }

    public async UniTask<RestaurantCharacter> CreateCharacter(int cid)
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
        character = ins_character.GetComponent<RestaurantCharacter>();
        
        character.InitCharacter(tbCharacter);
        
        _characters.Add(tbCharacter.Name,character);

        return character;
    }

    public RestaurantCharacter GetCharacterByName(string name)
    {
        if (_characters.TryGetValue(name, out var character))
        {
            return character;
        }

        return null;
    }

    public RestaurantCharacter GetCharacterById(int cid)
    {
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var tbCharacter = dataProvider.GetCharacterBaseInfo(cid);
        return GetCharacterByName(tbCharacter.Name);
    }

    public void RemoveCharacter(RestaurantCharacter character)
    {
        _characters.Remove(character.CharacterName);
        character.ReleaseCharacter();
    }
    
    public void RemoveCharacter(int cid)
    {
        var character = GetCharacterById(cid);
        RemoveCharacter(character);
    }
    
}
