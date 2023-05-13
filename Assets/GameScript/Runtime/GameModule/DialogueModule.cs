using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueModule : IModule
{
    public void OnCreate(object createParam)
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public Character CurentDialogueCharacter
    {
        get
        {
            return _curDialogueCharacter;
        }
        set
        {
            _curDialogueCharacter = value;    
        }
    }

    private Character _curDialogueCharacter;

    
}
