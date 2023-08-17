using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} OnTriggerEnter2D");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"{name} OnCollisionEnter2D");
    }
}
