using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public struct bulletForce {
   public Vector3 forceDirection;
   public float forceMag;
   public float forceDec;
   public float forceLife;
}

public class BulletBehavior : MonoBehaviour {

   public UnityEvent onFadeOut;

   protected TurretBehavior.Type damageType;
   protected float speed;
   protected float fadeTime = 0.5f;
   protected GameObject target;
   protected Vector3 targetPos;

   private float lastUpdateTime;
   private Tween selfDestruct;
   private List<bulletForce> bulletForceList = new List<bulletForce>();

   public void Init(TurretBehavior.Type damageType, float speed, GameObject target) {
      this.damageType = damageType;
      this.speed = speed;
      this.target = target;
      SetTarget(this.target);
    }

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {
      Vector3 nextPos = transform.position + transform.forward * speed * Time.deltaTime;

      if (target != null) {
         Vector3 curPos = transform.position;

         Vector3 closestPos = GetClosestPointAlongPathToTarget(transform.position, transform.forward, target.transform.position);

         float nextDist = (nextPos - curPos).magnitude;
         float closestDist = (closestPos - curPos).magnitude;

         if (nextDist > closestDist) {
            // we went pass the enemy

            if (closestDist < .5f) {
               //Debug.Log("DAMAGED THE OPPONENT: " + closestDist);
               target.GetComponent<EnemyMovement>().TakeDamage();
               Destroy(gameObject);
               Debug.Log("Appling effect based on " + damageType + "...");
            } else {
               //Debug.Log("TOO FAR AWAY: " + closestDist);
            }

            target = null;
            return;
         }
      } else {
         //This will only be called on the first frame of fade out
         if (selfDestruct == null) {
               onFadeOut.Invoke();
               transform.DOScale(0, fadeTime);
               //A delayed selfdestruction
               selfDestruct = DOTween.Sequence().AppendInterval(fadeTime).AppendCallback(
                  () => {
                     Destroy(gameObject);
                  });
         }
      }

      transform.position += transform.forward * speed * Time.deltaTime;

      //ProcessForce();
   }

   // find the point on the line in direction dir going through src that is closest to target
   Vector3 GetClosestPointAlongPathToTarget(Vector3 src, Vector3 dir, Vector3 target) {
      Vector3 p1 = src;
      Vector3 p2 = src + dir;
      Vector3 p3 = target;

      float a = Vector3.Dot(p2 - p3, p2 - p1);
      float b = -Vector3.Dot(p1 - p3, p2 - p1);

      return (a * p1) + (b * p2);
   }

   void ProcessForce() {
      for(int i = bulletForceList.Count - 1; i >= 0; i--) {
         bulletForce f = bulletForceList[i];
         transform.position += f.forceDirection * f.forceMag * Time.deltaTime;

         f.forceMag -= f.forceDec * Time.deltaTime;
         f.forceLife -= Time.deltaTime;

         if (f.forceLife <= 0) {
            bulletForceList.RemoveAt(i);
         }
      }
   }

   public void AddForce(bulletForce newForce) {
      bulletForceList.Add(newForce);
   }

   public virtual void SetTarget(GameObject target) {
      if (target == null) {
         Debug.Log("The target was destroyed...");
         return;
      }

      this.target = target;

      targetPos = target.transform.position;
      transform.LookAt(targetPos);
   }

   public virtual void SetSpeed(float speed) {
      this.speed = speed;
   }
}
