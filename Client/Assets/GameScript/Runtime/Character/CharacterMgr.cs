using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using YooAsset;

public class CharacterMgr : IModule
{
    private Dictionary<string,Character> _characters;
    public void OnCreate(object createParam)
    {
        _characters = new Dictionary<string, Character>();
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public async UniTask<Character> CreateCharacter(int cid)
    {
        var dataProvider = UniModule.GetModule<DataProviderModule>();
        var tbCharacter = dataProvider.GetCharacterBaseInfo(cid);
        if(tbCharacter == null)return null;
        
        if (_characters.TryGetValue(tbCharacter.Name, out var character))
        {
            return character;
        }
        
        var handle = YooAssets.LoadAssetAsync<GameObject>("Assets/GameRes/Prefabs/NPC/Character.prefab");
        await handle.ToUniTask();
    
        var go = handle.AssetObject as GameObject;

        var ins_character = GameObject.Instantiate(go);
        character = ins_character.GetComponent<Character>();
        
        character.InitCharacter(tbCharacter);
        
        _characters.Add(tbCharacter.Name,character);

        return character;
    }

    public Character GetCharacter(string name)
    {
        if (_characters.TryGetValue(name, out var character))
        {
            return character;
        }

        return null;
    }

}
