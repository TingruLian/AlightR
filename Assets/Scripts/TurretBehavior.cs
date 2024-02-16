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

   private float lastRotateTime;

   void Start() {
      // maybe find the EnemyContainer object

      target = null;
      
      attackTime = Time.time + attackInterval;
      lastRotateTime = Time.time;
   }

   void Update() {
      Vector3 turretPos = gameObject.transform.position;

      if (target == null || (target.transform.position - turretPos).sqrMagnitude > sqrRange) {
         SelectNewTarget();
      }

      if (target == null) {
         target = null;
      } else {
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

      RotateToTarget(target);
      FireBullet();
   }

   private void RotateToTarget(GameObject target) {
      Vector3 targetDir = (target.transform.position - transform.position).normalized;

      float curTime = Time.time;
      float elapsedTime = curTime - lastRotateTime;
      lastRotateTime = curTime;

      Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, elapsedTime * 60f * Mathf.Deg2Rad, 0f);

      transform.rotation = Quaternion.LookRotation(newDir);
   }

   private void FireBullet() {
      if (Time.time > attackTime) {
         attackTime = Time.time + attackInterval;

         GameObject bullet = GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
         bullet.GetComponent<BulletBehavior>().setTarget(target);
         bullet.GetComponent<BulletBehavior>().setSpeed(10);
      }
   }
}
