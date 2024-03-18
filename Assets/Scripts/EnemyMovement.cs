using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using Sequence = DG.Tweening.Sequence;

public enum EnemyState {
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
   protected EnemyState currentState;
   protected EnemyState lastState;

   public GameObject target;
   public int life = 3;
   [SerializeField]
   protected int damage;

   public Attribute<float> speed = new Attribute<float>();

   [SerializeField]
   protected UnityEvent onAttack;
   [SerializeField]
   protected UnityEvent onHurt;
   [SerializeField]
   protected UnityEvent onDeath;

   [SerializeField]
   protected Animator animator;

   protected Sequence _attackSequence;
   protected Tween _shakeTween;

   protected List<BaseEffect> effects = new List<BaseEffect>();


   private bool moving = true;
   private float lastUpdateTime;

   private void Awake() {
      if (enemies == null) {
         enemies = new List<EnemyMovement>();
      }
      enemies.Add(this);

      if (animator == null) {
         animator = GetComponent<Animator>();
      }

      ChangeState(EnemyState.spawn);
   }

   private void OnDisable() {
      enemies.Remove(this);
   }

   private void OnDestroy() {
      enemies.Remove(this);
      onDeath.Invoke();

      GameManager.instance.RemovePlayerLoseListener(Win);
   }

   private void OnEnable() {
      enemies.Add(this);

      GameManager.instance.AddPlayerLoseListener(Win);
   }

   void Start() {
      lastUpdateTime = Time.time;
      if (GetComponentInChildren<Animator>() != null) GetComponentInChildren<Animator>().Play("idle");
   }

   void Update() {
      ApplyEffects();

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      switch (currentState) {
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

      if (moving) {
         Vector3 curPos = gameObject.transform.position;
         Vector3 distTraveled = Vector3.Normalize(target.transform.position - curPos) * speed.GetCurValue() * elapsedTime;

         transform.position += distTraveled;
         transform.LookAt(target.transform.position);
      }
   }

   void ChangeState(EnemyState newState) {
      lastState = currentState;
      currentState = newState;
   }

   void ProcessSpawn() {
      //On Enter
      if (lastState != currentState) {
         lastState = currentState;
         if (animator != null) {
            animator.Play("spawn");
         }
      }

      if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=1) {
         ChangeState(EnemyState.engage);
      }
   }

   void ProcessEngage() {
      moving = true;

      //On Enter
      if (lastState != currentState) {
         lastState = currentState;
         if (animator != null) animator.Play("idle");
      }

      FindTarget();

      // the enmy reached the player, so play an attack animation and deal damage
      if (Vector3.Distance(transform.position, target.transform.position) < 0.9f) {
         ChangeState(EnemyState.attack); 
      }
   }

   void ProcessAttack() {
      moving = false;

      //On Enter
      if (lastState != currentState) {
         lastState = currentState;

         if (animator != null) animator.Play("attack");

         //the delay in this loop is based on animation
         _attackSequence = DOTween.Sequence()
            .AppendInterval(7f / 12f).AppendCallback(() => {
               target.GetComponent<Health>().TakeDamage(damage);
               onAttack.Invoke();
            })
            .AppendInterval(1.25f - 7f / 12f).AppendCallback(() => {
               ChangeState(EnemyState.engage);
               if (Vector3.Distance(transform.position, target.transform.position) < 0.9f) {
                  ChangeState(EnemyState.attack);
               }
            });
      }
   }

   void ProcessWin() {
      moving = true;

      //On Enter
      if (lastState != currentState) {
         lastState = currentState;
         if (animator != null) {
            animator.Play("idle");
         }

         target = transform.parent.gameObject;
         _attackSequence.Kill();
      }
   }

   public void TakeDamage() {
      life--;
      onHurt.Invoke();

      if (life <= 0) {
         GameManager.instance.DestroyEnemy(this);
         if (_attackSequence != null) { _attackSequence.Kill(); }
      }
   }

   public void Win() {
      ChangeState(EnemyState.leave);
   }


   public void FindTarget() {
      if (target == null) {
         _attackSequence.Kill(); _attackSequence = null;

         if (GetComponentInChildren<Animator>() != null) {
            GetComponentInChildren<Animator>().Play("idle");
         }

         target = GameManager.instance.playerBook;
         moving = true;
      }

      float dis = Vector3.Distance(transform.position, target.transform.position);

      if (Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position) < dis) {
         target = GameManager.instance.playerBook;
         dis = Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position);
      }

      if (TurretBehavior.turretList == null) {
         return;
      }

      foreach (TurretBehavior t in TurretBehavior.turretList) {
         if (Vector3.Distance(transform.position, t.transform.position) < dis) {
            target = t.gameObject;
            dis = Vector3.Distance(transform.position, t.transform.position);
         }
      }
   }

   public void AddEffect(BaseEffect effect) {
      effects.Add(effect);
   }

   private void ApplyEffects() {
      for (int i = effects.Count - 1; i >= 0; i--) {
         BaseEffect effect = effects[i];

         if (effect.IsExpired()) {
            effect.ResetEffect(this);
            effects.RemoveAt(i);
         } else {
            effect.ApplyEffect(this);
         }
      }
   }

   public void AddDeathListener(UnityAction action) {
      onDeath.AddListener(action);
   }

   public void RemoveDeathListener(UnityAction action) {
      onDeath.RemoveListener(action);
   }
}
