using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Transform player;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (GameObject.FindWithTag("Player") != null && player == null)
            player = GameObject.FindWithTag("Player").transform;
        
    }
}
