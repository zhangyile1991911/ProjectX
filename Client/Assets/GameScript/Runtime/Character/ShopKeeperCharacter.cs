using System.Collections;
using System.Collections.Generic;
using cfg.character;
using UnityEngine;

public class ShopKeeperCharacter : RestaurantRoleBase
{

    public override void PlayAnimation(behaviour behaviourId)
    {
        
    }
    
    protected override void LoadTableData()
    {
    }

    public override void ReleaseCharacter()
    {
        base.ReleaseCharacter();
        Destroy(gameObject);
    }
}
