using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBrain
{
    private List<CharacterBehaviour> _behaviours;

    public CharacterBrain()
    {
        _behaviours = new List<CharacterBehaviour>();
    }
    
}
