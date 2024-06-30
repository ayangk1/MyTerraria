using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameSceneSO",menuName = "Event/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public AssetReference sceneReference;
}
