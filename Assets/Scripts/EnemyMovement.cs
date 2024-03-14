using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class EnemyMovement : MonoBehaviour
{
   public static List<EnemyMovement> enemies;

   public GameObject target;
   public int life = 3;

   public Attribute<float> speed = new Attribute<float>();

   [SerializeField]
   protected UnityEvent onHurt;
   [SerializeField]
   protected UnityEvent onDeath;

   protected Sequence _attackSequence;
   protected Tween _shakeTween;

   protected List<BaseEffect> effects = new List<BaseEffect>();

   private bool win = false;
   private bool moving = true;

   private float lastUpdateTime;


   private void Awake()
   {
      if (enemies == null)
      {
         enemies = new List<EnemyMovement>();
      }
      enemies.Add(this);
   }

   private void OnDisable()
   {
      enemies.Remove(this);
   }

   private void OnDestroy()
   {
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
      if (GetComponentInChildren<Animator>() != null) GetComponentInChildren<Animator>().Play("idle");
   }

   void Update()
   {
      if (!win)
      {
         FindTarget();
      }

      ApplyEffects();

      // the enmy reached the player, so play an attack animation and deal damage
      if (Vector3.Distance(transform.position, target.transform.position) < 0.9f && _attackSequence == null)
      {
         moving = false;
         if(GetComponentInChildren<Animator>()!=null) GetComponentInChildren<Animator>().Play("attack");
         //the delay in this loop is based on animation
         _attackSequence = DOTween.Sequence().AppendInterval(7f / 12f).AppendCallback(
            () => {
               target.GetComponent<Health>().TakeDamage(1);
            })
            .AppendInterval(1.25f - 7f / 12f).SetLoops(-1);

      }

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      if (moving)
      {
         Vector3 curPos = gameObject.transform.position;

         Vector3 distTraveled = Vector3.Normalize(target.transform.position - curPos) * speed.GetCurValue() * elapsedTime;

         transform.position += distTraveled;
         transform.LookAt(target.transform.position);
      }
   }

   public void TakeDamage()
   {
      life--;
      onHurt.Invoke();

      if (life <= 0)
      {
         GameManager.instance.DestroyEnemy(this);
         if (_attackSequence != null) { _attackSequence.Kill(); }
      }
   }

   public void Win()
   {
      win = true;
      if (GetComponentInChildren<Animator>() != null) GetComponentInChildren<Animator>().Play("idle");
      target = transform.parent.gameObject;
      _attackSequence.Kill();
      moving = true;
   }


   public void FindTarget()
   {
      if (target == null)
      {
         _attackSequence.Kill(); _attackSequence = null;
         if (GetComponentInChildren<Animator>() != null) GetComponentInChildren<Animator>().Play("idle");
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

   public void AddDeathListener(UnityAction action) { onDeath.AddListener(action); }
   public void RemoveDeathListener(UnityAction action) { onDeath.RemoveListener(action); }
}
