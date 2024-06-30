using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class TransitionManager : SingletonMonoBehaviour<TransitionManager>
{
    public GameSceneSO menuScenceSO;
    public GameSceneSO currLoadedScene;
    public bool game;
    private void OnEnable()
    {
        StartCoroutine(TransitionToScene(menuScenceSO));
        
    }
 
    public void Transition(GameSceneSO to)
    {
        StartCoroutine(TransitionToScene(to));
    }

    private IEnumerator TransitionToScene(GameSceneSO to)
    {
        
        if (currLoadedScene != null)
            yield return currLoadedScene.sceneReference.UnLoadScene();
        if (to.sceneReference != null)
            yield return to.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        
        currLoadedScene = to;

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
    }
}
