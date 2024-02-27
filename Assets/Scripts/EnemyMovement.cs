using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour {
   public static List<EnemyMovement> enemies;

   public GameObject target;
   public float speed;
   public int life = 3;

   [SerializeField]
   protected UnityEvent onHurt;

   protected Sequence _attackSequence;
   protected Tween _shakeTween;

   private bool win=false;
   private float lastUpdateTime;

   private void Awake() {
      if (enemies == null) {
         enemies = new List<EnemyMovement>();
      }
      enemies.Add(this);
   }

   private void OnDisable() {
      enemies.Remove(this);
   }

   private void OnDestroy() {
      enemies.Remove(this);
      GameManager.instance.RemovePlayerLoseListener(Win);
      
   }

   private void OnEnable() {
      enemies.Add(this);
      GameManager.instance.AddPlayerLoseListener(Win);
  }

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {
      FindTarget();

      // the enmy already passed the player, so destroy it
      if (Vector3.Distance(transform.position, target.transform.position) < 0.9f && _attackSequence == null) {
         /*
         Destroy(gameObject);
         GameManager.instance.ModifyLives(-1);

         Debug.Log(Gamedata.bookHP);
         */

         speed = 0;
         GetComponentInChildren<Animator>().Play("attack");
         //the delay in this loop is based on animation
         _attackSequence = DOTween.Sequence().AppendInterval(7f/12f).AppendCallback(() =>
         {
             target.GetComponent<Health>().TakeDamage(1);
         }).AppendInterval(1.25f - 7f/12f).SetLoops(-1);

      }

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      Vector3 curPos = gameObject.transform.position;

      Vector3 distTraveled = Vector3.Normalize(target.transform.position - curPos) * speed * elapsedTime;

      transform.position += distTraveled;
      transform.LookAt(target.transform.position);
   }

   public void TakeDamage() {
      life--;
        onHurt.Invoke();

      if (life <= 0) {
         GameManager.instance.DestroyEnemy(this);
         if(_attackSequence != null) { _attackSequence.Kill(); }
      }
   }

    public void Win()
    {
        win = true;
        GetComponentInChildren<Animator>().Play("idle");
        target = transform.parent.gameObject;
        _attackSequence.Kill();
        speed = 1;
    }


    public void FindTarget()
    {
        if (win) return;

        if(target == null) { 
            _attackSequence.Kill(); _attackSequence = null;
            GetComponentInChildren<Animator>().Play("idle"); 
            target = GameManager.instance.playerBook;
            speed = 1;
        }

        float dis = Vector3.Distance(transform.position, target.transform.position);


        if (Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position) < dis)
        {
            target = GameManager.instance.playerBook;
            dis = Vector3.Distance(transform.position, GameManager.instance.playerBook.transform.position);
        }

        if(TurretBehavior.turretList == null) { return; }
        foreach (TurretBehavior t in TurretBehavior.turretList)
        {
            if(Vector3.Distance(transform.position, t.transform.position) < dis)
            {
                target = t.gameObject;
                dis = Vector3.Distance(transform.position, t.transform.position);
            }
        }


    }
}
