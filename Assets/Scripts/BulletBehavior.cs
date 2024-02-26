using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct bulletForce {
   public Vector3 forceDirection;
   public float forceMag;
   public float forceDec;
   public float forceLife;
}

public class BulletBehavior : MonoBehaviour {

   [SerializeField] protected float speed;
   [SerializeField] protected float fadeTime = 0.5f;

   protected GameObject target;

   protected Vector3 targetPos;

   private float lastUpdateTime;
   private Tween selfDestruct;
   private List<bulletForce> bulletForceList = new List<bulletForce>();

   public UnityEvent onFadeOut;

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {
      if (target != null) {
         targetPos = target.transform.position;
         Vector3 curPos = gameObject.transform.position;
         float dist = (targetPos - curPos).sqrMagnitude;

         if (dist < .1f) {
               target.GetComponent<EnemyMovement>().TakeDamage();
               target = null;
               Destroy(gameObject);
               return;
         }

         transform.LookAt(targetPos);
         Vector3 distTraveled = Vector3.Normalize(targetPos - curPos) * speed * Time.deltaTime;
         transform.position += distTraveled;
      } else {
         //Keep the bullet flying on the old direction
         transform.position += transform.forward * speed * Time.deltaTime;

         //This will only be called on the first frame of fade out
         if (selfDestruct == null) {
               onFadeOut.Invoke();
               transform.DOScale(0, fadeTime);
               //A delayed selfdestruction
               selfDestruct = DOTween.Sequence().AppendInterval(fadeTime).AppendCallback(
                  () => {
                     Destroy(gameObject);
                     return;
                  });
         }
      }

      ProcessForce();
   }

   void ProcessForce() {
      for(int i = bulletForceList.Count - 1; i >= 0; i--) {
         bulletForce f = bulletForceList[i];
         transform.position += f.forceDirection * f.forceMag * Time.deltaTime;

         f.forceMag -= f.forceDec*Time.deltaTime;
         f.forceLife -= Time.deltaTime;

         if(f.forceLife <= 0) { bulletForceList.RemoveAt(i); }
      }
   }

   public void AddForce(bulletForce newForce) {
      bulletForceList.Add(newForce);
   }

   public virtual void SetTarget(GameObject target) {
      if (target == null) {
         Debug.Log("The target was destroyed...");
      }

      this.target = target;
   }

   public virtual void SetSpeed(float speed) {
   this.speed = speed;
   }
}
