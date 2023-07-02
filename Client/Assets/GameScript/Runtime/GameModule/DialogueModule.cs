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

    public RestaurantCharacter CurentDialogueRestaurantCharacter
    {
        get
        {
            return _curDialogueRestaurantCharacter;
        }
        set
        {
            _curDialogueRestaurantCharacter = value;    
        }
    }

    private RestaurantCharacter _curDialogueRestaurantCharacter;

    
}
