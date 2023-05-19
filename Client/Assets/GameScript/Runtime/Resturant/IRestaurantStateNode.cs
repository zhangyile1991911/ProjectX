using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestaurantStateNode : IStateNode
{
    public void OnClickBubble(ChatBubble bubble);
}
