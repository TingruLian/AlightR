using UnityEngine;

public abstract class BaseEffect:MonoBehaviour {

   protected float startTime;
   protected float duration;

   public BaseEffect(float duration) {
      startTime = Time.time;
      this.duration = duration;
   }

   public abstract void ApplyEffect(EnemyMovement enemy);
   public abstract void ResetEffect(EnemyMovement enemy);

   public bool IsExpired() {
      float curTime = Time.time;

      return (Time.time - startTime) > duration;
   }
}
