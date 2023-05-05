using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public interface CharacterBehaviour
{
    public Character Char { get; }

    public void Enter(Character character);


    public void Exit();
}


public class CharacterEnterSceneBehaviour : CharacterBehaviour
{
    public Character Char => _character;
    private Character _character;
    
    private Vector3 destPoint;
    private Vector3 spawnPoint;
    public CharacterEnterSceneBehaviour(Vector3 spawn,Vector3 dest)
    {
        destPoint = dest;
        spawnPoint = spawn;
    }
    
    public void Enter(Character character)
    {
        _character = character;
        _character.transform.position = spawnPoint;

        var dest1 = new Vector3(destPoint.x, 0, _character.transform.position.z);
        var dest2 = new Vector3(destPoint.x, 0, destPoint.z);
        var seq = DOTween.Sequence();
        seq.Append(_character.transform.DOMove(dest1, 7f));
        seq.Append(_character.transform.DOMove(dest2, 3f));
        // seq.Complete();
        // _character.transform.DOMove(dest1, 10f);
        // _character.MoveTo(dest1,10);
    }

    public void Exit()
    {
        
    }
}