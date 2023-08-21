using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBoard : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;

    public OrderMealInfo Info
    {
        get;
        set;
    }
    // Start is called before the first frame update
    void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
}
