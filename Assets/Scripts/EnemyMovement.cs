using UnityEngine;

public class EnemyMovement : MonoBehaviour {

   public Vector3 target;
   public float speed;

   private float lastUpdateTime;

   private int life;

   void Start() {
      lastUpdateTime = Time.time;

      life = 3;
   }

   void Update() {
      // the enmy already passed the player, so destroy it
      if (transform.position.z > -2.4f) {
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
   }

   public void TakeDamage() {
      life--;

      if (life <= 0) {
         Destroy(gameObject);
      }
   }
}
