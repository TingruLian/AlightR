using UnityEngine;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour {

   public Vector3 target;
   public float speed;

   private float lastUpdateTime;

   public  int life = 3;

   void Start() {
      lastUpdateTime = Time.time;

   }

   void Update() {
      // the enmy already passed the player, so destroy it
      if (Vector3.Distance(transform.position,target) < 0.5f) {
         Destroy(gameObject);

         Debug.Log("Enemy reached the base...");

         return;
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
