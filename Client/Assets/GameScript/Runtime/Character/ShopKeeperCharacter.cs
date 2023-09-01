using System.Collections;
using System.Collections.Generic;
using cfg.character;
using UnityEngine;

public class ShopKeeperCharacter : RestaurantRoleBase
{

    public override void PlayAnimation(behaviour behaviourId)
    {
        
    }

    public override void ReleaseCharacter()
    {
        base.ReleaseCharacter();
        Destroy(gameObject);
    }
}
