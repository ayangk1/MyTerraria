using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    public VoidEventSO saveGameEvent;

    public GameSceneSO menuScenceSO;
    public GameSceneSO gameScenceSO;

    public CharacterEventSO healthEvent;
    public CharacterEventSO enemyHealthEvent;
    public PlayerStateBar playerStateBar;

    public Button singleButton;
    public Button continueButton;

    public Sprite[] splash;
    public Image splashBg;

    public GameObject canvas;
    public GameObject settingPanel;
    public Button escButton;

    private Scene activeScene;

    private void Start()
    {
        if (!TransitionManager.Instance.game)
        {

            continueButton.onClick?.AddListener(() =>
            {
                DataManager.Instance.isLoadData = true;
                splashBg.color = Color.white;
                int range = Random.Range(0, splash.Length);
                splashBg.sprite = splash[range];
                TransitionManager.Instance.game = true;
                TransitionManager.Instance.Transition(gameScenceSO);
            });
            singleButton.onClick?.AddListener(
                    () =>
                    {

                        DataManager.Instance.isLoadData = false;
                        splashBg.color = Color.white;
                        int range = Random.Range(0, splash.Length);
                        splashBg.sprite = splash[range];
                        TransitionManager.Instance.game = true;
                        TransitionManager.Instance.Transition(gameScenceSO);
                    });

        }

        if (TransitionManager.Instance.game)
        {
            settingPanel.SetActive(false);
            escButton.onClick?.AddListener(
                () =>
                {
                    saveGameEvent.RaiseEvent();
#if UNITY_EDITOR  //在编辑器模式下

                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                });
        }
    }

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        enemyHealthEvent.OnEventRaised += OnEnemyHealthEvent;
    }

    List<string> enemyHealthTextList = new List<string>();
    private void OnEnemyHealthEvent(Character character)
    {
        if (!enemyHealthTextList.Contains(character.GetComponent<DataDefination>().ID))
        {
            GameObject go = ObjectPool.Instance.GetFormPool(3);
            go.transform.SetParent(canvas.transform);
            go.name = character.GetComponent<DataDefination>().ID + "Health";
            go.GetComponent<EnemyHealth>().Init(character);
            if (go.GetComponent<TextMeshProUGUI>() != null)
                go.GetComponent<TextMeshProUGUI>().text = character.currHealth + "/" + character.maxHealth;
            enemyHealthTextList.Add(character.GetComponent<DataDefination>().ID);

            StartCoroutine(ShowEnemyHealth(go, character));
        }
        else
        {
            GameObject go = GameObject.Find(character.GetComponent<DataDefination>().ID + "Health");
            if (go != null) go.GetComponent<TextMeshProUGUI>().text = character.currHealth + "/" + character.maxHealth;

            StopCoroutine(ShowEnemyHealth(go, character));
            StartCoroutine(ShowEnemyHealth(go, character));
        }
    }

    [SerializeField] GameObject promptObj;
    public void ShowGamePrompt(TileClass tileClass, int x, int y, Vector3 worldpos)
    {
        Vector2 screenPoint = RectTransformUtility
        .WorldToScreenPoint(Camera.main, new Vector3(worldpos.x + 0.1f, worldpos.y + 0.1f, 0));
        promptObj.transform.position = new Vector3(screenPoint.x,screenPoint.y,0);
        if (promptObj.GetComponent<TextMeshProUGUI>() != null)
            promptObj.GetComponent<TextMeshProUGUI>().text
            = ((int)TerrainManager.Instance.demagedCondition[(int)tileClass.layer, x, y]).ToString() + '%';

        
    }
    public void HideGamePrompt()
    {
            if (promptObj.GetComponent<TextMeshProUGUI>() != null)
            promptObj.GetComponent<TextMeshProUGUI>().text = "";

    }
    

    private IEnumerator ShowEnemyHealth(GameObject go, Character character)
    {
        yield return new WaitForSeconds(2);
        enemyHealthTextList.Remove(character.GetComponent<DataDefination>().ID);
        go.name = "normal";
        ObjectPool.Instance.ReturnPool(go, 3);
    }


    private void OnHealthEvent(Character character)
    {
        playerStateBar.OnHealthChange(character);
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        enemyHealthEvent.OnEventRaised -= OnEnemyHealthEvent;
    }

    private void Update()
    {
        if (TransitionManager.Instance.game)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (settingPanel.activeSelf)
                    settingPanel.SetActive(false);
                else
                    settingPanel.SetActive(true);
            }
        }

    }


}
