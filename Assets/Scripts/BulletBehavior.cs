using UnityEngine;

public class BulletBehavior : MonoBehaviour {

   private GameObject target;

   private float speed;
   private Vector3 targetPos;

   private float lastUpdateTime;

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {
      
      // the target was destroyed
      if (target == null) {
         target = null;
         Destroy(gameObject);
         
         return;
      }

      targetPos = target.transform.position;

      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      Vector3 curPos = gameObject.transform.position;

      float dist = (targetPos - curPos).sqrMagnitude;

      if (dist < .1f) {

         // the target was destroyed
         if (target == null) {
            target = null;
            Destroy(gameObject);

            return;
         }

         target.GetComponent<EnemyMovement>().TakeDamage();
         target = null;

         Destroy(gameObject);

         return;
      }

      Vector3 distTraveled = Vector3.Normalize(targetPos - curPos) * speed * elapsedTime;

      transform.position += distTraveled;
   }

   public void setTarget(GameObject target) {
      if (target == null) {
         Debug.Log("The target was destroyed...");
      }

      this.target = target;
   }

   public void setSpeed(float speed) {
      this.speed = speed;
   }
}
