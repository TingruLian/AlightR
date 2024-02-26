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

   private float lastUpdateTime;

    [SerializeField] protected UnityEvent onHurt;

    protected Sequence _attackSequence;
    protected Tween _shakeTween;

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


      // the enmy already passed the player, so destroy it
      if (Vector3.Distance(transform.position,target.transform.position) < 0.9f && _attackSequence == null) {
            /*Destroy(gameObject);
            GameManager.instance.ModifyLives(-1);
         
            Debug.Log(Gamedata.bookHP);
            //Debug.Log("Enemy reached the base...");

            return;*/

            speed = 0;
            GetComponentInChildren<Animator>().Play("attack");
            //the delay in this loop is based on animation
            _attackSequence = DOTween.Sequence().AppendInterval(7f/12f).AppendCallback(() =>
            {
                GameManager.instance.ModifyLives(-1);
            }).AppendInterval(1.25f - 7f/12f).SetLoops(-1);

        }

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      Vector3 curPos = gameObject.transform.position;

      Vector3 distTraveled = Vector3.Normalize(target.transform.position - curPos) * speed * elapsedTime;

      transform.position += distTraveled;
      transform.LookAt(target.transform);
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
        _attackSequence.Pause();
        GetComponentInChildren<Animator>().Play("idle");
        speed = 1;
        target = transform.parent.gameObject;
    }

}
