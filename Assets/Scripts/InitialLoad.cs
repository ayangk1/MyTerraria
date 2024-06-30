using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitialLoad : MonoBehaviour
{
    public GameSceneSO persisentScence;

    private void Awake()
    {
        Addressables.LoadSceneAsync(persisentScence.sceneReference);
    }
    
}
