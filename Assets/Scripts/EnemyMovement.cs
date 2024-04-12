using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using Sequence = DG.Tweening.Sequence;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;
using Google.Protobuf.WellKnownTypes;
using System.Collections;

public enum EnemyState
{
    nul,
    spawn,
    engage,
    attack,
    leave
}

public class EnemyMovement : MonoBehaviour
{
    public static List<EnemyMovement> enemies;

    [SerializeField]
    protected int typeID = 0;

    [SerializeField]
    protected EnemyState currentState;
    protected EnemyState lastState;

    public GameObject target;
    public int life = 3;
    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float rotationSpeed = 180;

    [SerializeField]
    protected bool lockY = false;

    public Attribute<float> speed = new Attribute<float>();

    [SerializeField]
    protected UnityEvent onAttack;
    [SerializeField]
    protected UnityEvent onHurt;
    [SerializeField]
    protected UnityEvent onDeath;


    [SerializeField]
    protected Animator animator;

    [SerializeField]
    public GameObject slownDecoration;

    [SerializeField]
    public GameObject moveGroundDecoration;

    [SerializeField]
    protected OffscreenPositionIndicator offscreenIndicator;
    const float indicatorOffset = 120f;

    protected GameObject uiCanvas;
    protected GameObject indicatorUIPrefab;
    protected IndicatorUnit indicatorUI = null;

    protected Sequence _attackSequence;
    protected Tween _shakeTween;

    protected List<BaseEffect> effects = new List<BaseEffect>();


    private bool moving = true;
    private float lastUpdateTime;
    [SerializeField] protected List<SkinnedMeshRenderer> skinnedMeshRenderers;

    [SerializeField] protected LayerMask lockMask;
    public UnityEvent OnLocked;
    public UnityEvent OnUnLock;



    private void Awake()
    {
        if (enemies == null)
        {
            enemies = new List<EnemyMovement>();
        }
        enemies.Add(this);

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        ChangeState(EnemyState.spawn);
        InitOffscreenIndicator();
    }

    private void OnDisable()
    {
        enemies.Remove(this);
    }

    private void OnDestroy()
    {
        if (indicatorUI != null) Destroy(indicatorUI.gameObject);

        enemies.Remove(this);
        onDeath.Invoke();

        GameManager.instance.RemovePlayerLoseListener(Win);
    }

    private void OnEnable()
    {
        enemies.Add(this);

        GameManager.instance.AddPlayerLoseListener(Win);
    }

    void Start()
    {


        lastUpdateTime = Time.time;
    }

    void Update()
    {
        ApplyEffects();

        float curTime = Time.time;
        float elapsedTime = curTime - lastUpdateTime;
        lastUpdateTime = curTime;

        switch (currentState)
        {
            case EnemyState.spawn:
                ProcessSpawn();
                break;
            case EnemyState.engage:
                ProcessEngage();
                break;
            case EnemyState.attack:
                ProcessAttack();
                break;
            case EnemyState.leave:
                ProcessWin();
                break;
        }

        if (moving)
        {

            //--------------Process Rotation-------------//
            Vector3 targetDirection = target.transform.position - transform.position;
            if (lockY) targetDirection.y = 0;
            targetDirection.Normalize();

            Quaternion newRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotationSpeed * elapsedTime);


            //--------------Process Movement--------------//
            Vector3 curPos = gameObject.transform.position;
            Vector3 distTraveled = transform.forward * speed.GetCurValue() * elapsedTime;

            if (lockY) { distTraveled.y = 0; }

            if (Vector3.Magnitude(curPos - target.transform.position) <= distTraveled.magnitude) { transform.position = target.transform.position; }
            else { transform.position += distTraveled; }
        }

        UpdateOffscreenIndicator();
        ProcessLock();

        // the offscreen indicator should be visible when the enemy itself is not visible
        //indicatorUI.SetActive(!offscreenIndicator.IsVisible());
        //Unity's on visible is not reliable as tested (weird)
    }

    void ProcessLock()
    {
        Utils.OnPress((Vector2 pos, Ray ray) =>
        {
            RaycastHit hit;
            Physics.Raycast(ray, out hit, lockMask);
            if (hit.collider == this.GetComponent<Collider>())
            {
                foreach (TurretBehavior t in TurretBehavior.turretList) 
                {
                    t.ForceLockTarget(gameObject);
                }
                this.OnLocked.Invoke();
                foreach(EnemyMovement e in EnemyMovement.enemies)
                {
                    if(e != this) e.OnUnLock.Invoke();
                }
            }
        });
    }
    void InitOffscreenIndicator()
    {
        Debug.LogError("GUESS WHAT? IT'S GETTING INITIIALIZED!!!");
        if (indicatorUI == null)
        {
            GameManager gm = GameManager.instance;

            indicatorUIPrefab = gm.enemyIndicator;
            uiCanvas = gm.uiCanvas;

            indicatorUI = Instantiate(indicatorUIPrefab, uiCanvas.transform).GetComponent<IndicatorUnit>();
            indicatorUI.transform.localScale = new Vector3(.5f, .5f, 1f);
            indicatorUI.Ini(typeID);
            indicatorUI.gameObject.SetActive(false);
        }
    }

    void UpdateOffscreenIndicator()
    {
        //Vector2 screenPos = GetRelativeScreenPos();

        //// TODO: Do a better job of mapping 3d game units to onscreen pixels
        //indicatorUI.transform.position = new Vector2(540, 960) + (screenPos * 125);

        //Wuji's version

        Vector3 indicatorPos = Camera.main.WorldToScreenPoint(transform.position);
        RectTransform canvas = uiCanvas.GetComponent<RectTransform>();

        //if on screen
        if (indicatorPos.z >= 0f && indicatorPos.x >= -20f && indicatorPos.x <= canvas.rect.width * canvas.localScale.x + 20f
           && indicatorPos.y >= -20f && indicatorPos.y <= canvas.rect.height * canvas.localScale.y + 20f)
        {
            indicatorUI.gameObject.SetActive(false);
        }
        //if off screen
        else
        {
            indicatorUI.gameObject.SetActive(true);

            Vector3 oldPos = indicatorPos;

            indicatorPos.x = math.clamp(indicatorPos.x, indicatorOffset * canvas.localScale.x, (canvas.rect.width - indicatorOffset) * canvas.localScale.x);
            indicatorPos.y = math.clamp(indicatorPos.y, indicatorOffset * canvas.localScale.x, (canvas.rect.height - indicatorOffset) * canvas.localScale.y);

            Vector3 dif = oldPos - indicatorPos;

            indicatorUI.transform.position = indicatorPos;
            indicatorUI.SetRotate(new Vector3(0, 0, -Mathf.Atan2(dif.x, dif.y) * (180.0f / Mathf.PI)));
        }
    }

    Vector2 GetRelativeScreenPos()
    {
        Transform cam = GameManager.instance.arCamera.transform;

        Vector3 objPosition = transform.position - cam.transform.position;

        Vector3 projectedPoint = Vector3.ProjectOnPlane(objPosition, cam.forward);

        Vector3 upVec = Vector3.Project(projectedPoint, cam.up);
        Vector3 rightVec = Vector3.Project(projectedPoint, cam.right);

        float upCoord = upVec.x / cam.up.x;
        float rightCoord = rightVec.x / cam.right.x;

        upCoord = CapNumber(upCoord, -960f / 125f, 960f / 125f);
        rightCoord = CapNumber(rightCoord, -540f / 125f, 540f / 125f);

        return new Vector2(rightCoord, upCoord);
    }

    float CapNumber(float val, float min, float max)
    {
        if (val < min)
        {
            return min;
        }
        if (val > max)
        {
            return max;
        }

        return val;
    }

    void ChangeState(EnemyState newState)
    {
        lastState = currentState;
        currentState = newState;
    }

    void ProcessSpawn()
    {
        //On Enter
        if (lastState != currentState)
        {
            lastState = currentState;
            if (animator != null)
            {
                animator.Play("spawn");
            }
        }

        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            ChangeState(EnemyState.engage);
        }
    }

    void ProcessEngage()
    {
        moving = true;

        //On Enter
        if (lastState != currentState)
        {
            lastState = currentState;
            if (animator != null)
            {
                animator.CrossFadeInFixedTime("idle", 0.25f);
            }

            if (moveGroundDecoration) { moveGroundDecoration.SetActive(true); }
        }

        FindTarget();

        // the enmy reached the player, so play an attack animation and deal damage
        if (Vector3.Distance(transform.position, target.transform.position) < 0.9f)
        {
            ChangeState(EnemyState.attack);
        }
    }

    void ProcessAttack()
    {
        moving = false;

        //On Enter
        if (lastState != currentState)
        {
            lastState = currentState;

            if (animator != null) animator.CrossFadeInFixedTime("attack", 0.25f);

            //the delay in this loop is based on animation
            _attackSequence = DOTween.Sequence()
               .AppendInterval(7f / 12f).AppendCallback(() => {
                   target.GetComponent<Health>().TakeDamage(damage);
                   onAttack.Invoke();
               })
               .AppendInterval(1.25f - 7f / 12f).AppendCallback(() => {
                   ChangeState(EnemyState.engage);
                   if (Vector3.Distance(transform.position, target.transform.position) < 0.9f)
                   {
                       ChangeState(EnemyState.attack);
                   }
               });

            if (moveGroundDecoration) { moveGroundDecoration.SetActive(false); }
        }
    }

    void ProcessWin()
    {
        moving = true;

        //On Enter
        if (lastState != currentState)
        {
            lastState = currentState;
            if (animator != null)
            {
                animator.Play("idle");
            }

            target = this.gameObject;
            _attackSequence.Kill();
        }
    }


    public void TakeDamage(int dmg)
    {
        life -= dmg;
        onHurt.Invoke();

        if (life <= 0)
        {
            GameManager.instance.DestroyEnemy(this);
            if (_attackSequence != null) { _attackSequence.Kill(); }
        }
    }

    public void Win()
    {
        ChangeState(EnemyState.leave);
    }


    public void FindTarget()
    {

        if (target != null && lockY && Mathf.Abs(target.transform.position.y - transform.position.y) >= 1) { target = null; }

        if (target == null)
        {
            _attackSequence.Kill(); _attackSequence = null;

            if (GetComponentInChildren<Animator>() != null)
            {
                GetComponentInChildren<Animator>().CrossFadeInFixedTime("idle", 0.25f);
            }

            target = GameManager.instance.playerBook;
            moving = true;
        }

        float dis = Vector3.Distance(transform.position, target.transform.position);

        if (Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position) < dis)
        {
            target = GameManager.instance.playerBook;
            dis = Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position);
        }

        if (TurretBehavior.turretList == null)
        {
            return;
        }

        foreach (TurretBehavior t in TurretBehavior.turretList)
        {
            if (Vector3.Distance(transform.position, t.transform.position) < dis)
            {
                if (lockY && Mathf.Abs(t.transform.position.y - transform.position.y) >= 1) { continue; }

                target = t.gameObject;
                dis = Vector3.Distance(transform.position, t.transform.position);
            }
        }
    }

    public void AddEffect(BaseEffect effect)
    {
        effects.Add(effect);
    }

    private void ApplyEffects()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            BaseEffect effect = effects[i];

            if (effect.IsExpired())
            {
                effect.ResetEffect(this);
                effects.RemoveAt(i);
            }
            else
            {
                effect.ApplyEffect(this);
            }
        }
    }

    public void AddDeathListener(UnityAction action)
    {
        onDeath.AddListener(action);
    }

    public void RemoveDeathListener(UnityAction action)
    {
        onDeath.RemoveListener(action);
    }
    private GameObject materialObject;

    public void changeMaterials()
    {
        foreach (SkinnedMeshRenderer r in skinnedMeshRenderers)
        {
            Material[] materials = r.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_IceScale", 0.65f);
            }
            // Apply the modified materials back to the renderer
            r.materials = materials;
        }
    }
    public void changeBackMaterials(float duration)
    {
        StartCoroutine(ChangeBackMaterialsCoroutine(duration));
        /*materialObject = this.gameObject.transform.GetChild(Child1Index).GetChild(Child2Index).gameObject;//Fish: 3,0, Cat 1,1
        Renderer renderer = materialObject.GetComponent<SkinnedMeshRenderer>();
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_IceScale", 0f);
        }
        // Apply the modified materials back to the renderer
        renderer.materials = materials;*/
    }
    //private float duration = 2.0f;
    private IEnumerator ChangeBackMaterialsCoroutine(float duration)
    {


        // Assuming the Ice Scale is initially set to some value and you want to bring it back to 0
        float initialIceScale = 0.65f; // Example initial value, adjust as needed
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newIceScale = Mathf.Lerp(initialIceScale, 0f, elapsedTime / duration);

            foreach (SkinnedMeshRenderer r in skinnedMeshRenderers)
            {
                Material[] materials = r.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_IceScale", newIceScale);
                }
                r.materials = materials;
            }

            yield return null;

        }

        foreach (SkinnedMeshRenderer r in skinnedMeshRenderers)
        {
            // Ensure the value is set to 0 at the end
            Material[] finalMaterials = r.materials;
            for (int i = 0; i < finalMaterials.Length; i++)
            {
                finalMaterials[i].SetFloat("_IceScale", 0f);
            }
            r.materials = finalMaterials;
        }
    }
}
