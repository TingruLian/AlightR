using UnityEngine;

public class TurretBehavior : MonoBehaviour {

   [SerializeField]
   private GameObject bulletPrefab;

   public GameObject enemyContainer;

   private float attackRange;
   private float sqrRange;

   private GameObject target;

   private float attackInterval = .5f;
   private float attackTime;

   void Start() {
      // maybe find the EnemyContainer object

      target = null;
      
      attackTime = Time.time + attackInterval;
   }

   void Update() {
      Vector3 turretPos = gameObject.transform.position;

      if (target == null || (target.transform.position - turretPos).sqrMagnitude > sqrRange) {
         SelectNewTarget();
      }

      if (target == null) {
         target = null;
      } else {
         Debug.Log("Apparently a target was set");
         AttackTarget(target);
      }
   }

   public void SetAttackRange(float attackRange) {
      this.attackRange = attackRange;
      sqrRange = Mathf.Pow(attackRange, 2f);
   }

   private void SelectNewTarget() {
      target = null;

      GameObject closest = null;
      float sqrClosestDist = float.MaxValue;

      Vector3 turretPos = gameObject.transform.position;

      EnemyMovement[] enemies = enemyContainer.GetComponentsInChildren<EnemyMovement>();

      foreach (EnemyMovement enemyController in enemies) {
         GameObject enemy = enemyController.gameObject;

         float sqrDist = (enemy.transform.position - turretPos).sqrMagnitude;

         if (sqrDist > sqrRange) {
            continue;
         }

         if (closest == null || sqrDist < sqrClosestDist) {
            closest = enemy;
            sqrClosestDist = sqrDist;
         }
      }

      target = closest;
   }

   private void AttackTarget(GameObject target) {
      // if the target GameObject is destroyed, "target == null" will be true, but the target variable won't actually be null
      if (target == null) {
         target = null;
         return;
         Debug.Log("Should have returned");
      }

      Debug.Log("About to fire a projectile");

      if (Time.time > attackTime) {
         attackTime = Time.time + attackInterval;

         if (target == null) {
            Debug.Log("THE TARGET IS NULL");
         }

         GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
         bullet.GetComponent<BulletBehavior>().setTarget(target);
         bullet.GetComponent<BulletBehavior>().setSpeed(10);
      }
   }
}
