using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Playables;
using System.Collections;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour {
    public static List<EnemyMovement> enemies;

   public Vector3 target;
   public float speed;
   public int life = 3;

   private float lastUpdateTime;

   [SerializeField]
   private GameScriptableObject Gamedata;

    protected bool _attacked = false;

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
   }

   private void OnEnable() {
      enemies.Add(this);
   }

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {


      // the enmy already passed the player, so destroy it
      if (Vector3.Distance(transform.position,target) < 0.9f && !_attacked) {
            /*Destroy(gameObject);
            GameManager.instance.ModifyLives(-1);
         
            Debug.Log(Gamedata.bookHP);
            //Debug.Log("Enemy reached the base...");

            return;*/

            _attacked = true; speed = 0;
            GetComponentInChildren<Animator>().Play("attack");        
            DOTween.Sequence().AppendInterval(0.4f).AppendCallback(() =>
            {
                GameManager.instance.ModifyLives(-1);
            }).AppendInterval(0.85f).SetLoops(-1);

        }

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      Vector3 curPos = gameObject.transform.position;

      Vector3 distTraveled = Vector3.Normalize(target - curPos) * speed * elapsedTime;

      transform.position += distTraveled;
      transform.LookAt(target);
   }

   public void TakeDamage() {
      life--;

      transform.DOShakeScale(0.5f, 0.25f, 200, 90, true, ShakeRandomnessMode.Harmonic);
      if (life <= 0) {
         GameManager.instance.DestroyEnemy(this);
      }
   }
}
