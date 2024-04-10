using UnityEngine;

using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;
using UnityEditor;


public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField]
    SemanticColorControl colorControl;

    public GameObject playerBook;

    [SerializeField]
    public GameObject enemyIndicator;

    [SerializeField]
    public GameObject uiCanvas;

    [SerializeField]
    public Camera arCamera;



    [SerializeField]
    public int LevelId;

    protected List<LocationTurretPlacement> lpList = new List<LocationTurretPlacement>();

    [SerializeField]
    protected UnityEvent onResourceEnough;

    [SerializeField]
    protected UnityEvent onResourceNotEnough;

    [SerializeField]
    protected UnityEvent onInitialPlace;

    [SerializeField]
    protected UnityEvent onCompletePlace;

    [SerializeField]
    TMP_Text tmpResources;
    UIField uiResources;

    [SerializeField]
    TMP_Text tmpLives;

    [SerializeField]
    Image imgLives;
    [SerializeField]
    Renderer shieldRenderer;
    UIField uiLives;

    [SerializeField]
    private GameObject turret;

    [SerializeField]
    private GameObject turretPlaceholder;

    [SerializeField]
    private float turretAttackRange;

    [SerializeField]
    private GameScriptableObject GameData;

    [SerializeField] protected int maxSpirit = 2;
    protected int currentSpirit;
    protected SpiritControl spiritControl;

    [SerializeField]
    private UnityEvent onPlayerHurt;

    [SerializeField]
    private UnityEvent onPlayerLose;

    [SerializeField]
    private UnityEvent onPlayerWin;

    [SerializeField]
    private int enemyKilled = 0, totalEnemy = 20;

    private int resources;
    private int lives;

    private bool lost = false;

    // There should only be one turret placeholder in the scene at a time
    private GameObject placeholderInstance;

    private const int TURRET_COST = 20;
    protected Tween _shakeTween;
    private void Awake()
    {
        if (colorControl == null) { colorControl = SemanticColorControl.GetInstance(); }
        if (colorControl != null) { colorControl.Initialize(); colorControl.SetToStartColor(); }

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }


        currentSpirit = maxSpirit;

        if (spiritControl == null) { spiritControl = FindObjectOfType<SpiritControl>(); }
        spiritControl.Initilization(maxSpirit);
    }

    private void Start()
    {

        resources = 20;
        lives = 10;

        GameData.bookHP = lives;

        uiResources = new UIField("Resources", tmpResources, resources.ToString());
        uiLives = new UIField("Lives", tmpLives, GameData.bookHP.ToString());

        if (resources >= TURRET_COST) { onResourceEnough.Invoke(); }
        else { onResourceNotEnough.Invoke(); }
    }

    private void Update()
    {

        if (colorControl != null) { colorControl.UpdateLoop(); }

        if (EnemyMovement.enemies == null)
        {
            return;
        }

        //Debug.Log("Forward vector");
        //Debug.Log(Camera.main.transform.forward);

        EnemyMovement[] enemies = EnemyMovement.enemies.ToArray();
        foreach (EnemyMovement enemyController in enemies)
        {
        }


    }

    public void ModifyResources(int mod)
    {
        resources += mod;
        uiResources.updateValue(resources.ToString());

        if (resources >= TURRET_COST) { onResourceEnough.Invoke(); }
        else { onResourceNotEnough.Invoke(); }
    }

    public void ModifyLives(int mod)
    {
        GameData.bookHP += mod;
        uiLives.updateValue(GameData.bookHP.ToString());
        imgLives.fillAmount = ((float)GameData.bookHP) / 10f;
        if (shieldRenderer) shieldRenderer.material.SetFloat("_CrackAlpha", 1f - ((float)GameData.bookHP) / 10f);


        if (mod < 0) { onPlayerHurt.Invoke(); }
        if (GameData.bookHP == 0) { Destroy(shieldRenderer.gameObject); }
        if (GameData.bookHP < 0 && !lost) { lost = true; onPlayerLose.Invoke(); }
    }

    public void DestroyEnemy(EnemyMovement enemy)
    {
        Destroy(enemy.gameObject);
        enemyKilled++;
        //if (enemyKilled == totalEnemy && lives > 0)
        //{
        //    Debug.Log("win");
        //    onPlayerWin.Invoke();
        //}
        //ModifyResources(10);
    }

    public GameObject GetPlaceholderInstance()
    {
        return placeholderInstance;
    }

    public void ClearPlaceholderInstance()
    {
        Destroy(placeholderInstance);
        placeholderInstance = null;
    }

    public void PlaceTurretInitial(Vector3 pos)
    {
        // check that the player has enough resources, but don't remove them until the player confirms placement
        if (resources < TURRET_COST)
        {
            return;
        }

        if (placeholderInstance != null)
        {
            Debug.LogError("There should not currently be any instances of the turret placeholder");
        }

        placeholderInstance = GameObject.Instantiate(turretPlaceholder, pos, Quaternion.identity);
        onInitialPlace.Invoke();
    }

    public void PlaceTurretConfirm()
    {
        if (placeholderInstance == null)
        {
            Debug.LogError("There should be an instance of the turret placeholder in the scene");
        }

        if (resources < TURRET_COST)
        {
            return;
        }

        ModifyResources(-TURRET_COST);

        Vector3 pos = placeholderInstance.transform.position;

        ClearPlaceholderInstance();

        GameObject turretInstance = GameObject.Instantiate(turret, pos, Quaternion.identity).transform.GetChild(0).gameObject;
        turretInstance.GetComponent<TurretBehavior>().SetAttackRange(turretAttackRange);

        onCompletePlace.Invoke();
    }

    public void AddPlayerLoseListener(UnityAction act)
    {
        onPlayerLose.AddListener(act);
    }

    public void RemovePlayerLoseListener(UnityAction act)
    {
        onPlayerLose.RemoveListener(act);
    }

    public void AttemptWin()
    {
        if (lives > 0)
        {
            onPlayerWin.Invoke();
            GameData.completionState[LevelId] = true;
        }
    }

    public bool RequestTurret(LocationTurretPlacement lp)
    {
        if (currentSpirit > 0)
        {
            lpList.Add(lp);
            currentSpirit--;

            spiritControl.RequestSpirit(lp.transform);
            return true;
        }

        if (currentSpirit <= 0)
        {
            if (lpList.Count > 0)
            {
                lpList[0].RemoveTurret(true);
                return RequestTurret(lp);
            }

            return false;
        }

        return true;
    }

    public void FreeTurret(LocationTurretPlacement lp)
    {
        lpList.Remove(lp);

        spiritControl.FreeSpirit(lp.transform);
        currentSpirit++;
    }


    public void AddResource2(int count)
    {
        currentSpirit += count;
        maxSpirit += count;
        spiritControl.ModifySlots(count);

        GameObject.FindGameObjectWithTag("SpiritIcon")
            .GetComponent<DoTweenFeedback>().Appear(2);
    }
}
