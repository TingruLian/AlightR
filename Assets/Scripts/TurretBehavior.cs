using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

   [SerializeField]
   private GameObject bulletPrefab;

   public GameObject enemyContainer;

   [SerializeField]
   private List<Transform> shootAnchor;
    
   [SerializeField]
   private float attackRange;
   private float sqrRange;

   [SerializeField]
   private GameObject target;

   private Vector3 attackDir;
   private Vector3 currentDir;

   public float attackInterval = .5f;
   private float attackTime;
   public float bulletSpeed = 5.0f;

   void Start() {
      // maybe find the EnemyContainer object

      target = null;
      
      attackTime = Time.time + attackInterval;

      attackDir = transform.forward;
      currentDir = attackDir;
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

      // let the user rotate the turret by clicking on it
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         if (GetComponent<SphereCollider>().Raycast(ray, out hit, 100.0f)) {
            // rotate the turret 90 degrees to the right
            transform.rotation = Quaternion.LookRotation(transform.right);
            attackDir = transform.forward;
         }
      });
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

        //EnemyMovement[] enemies = enemyContainer.GetComponentsInChildren<EnemyMovement>();
        EnemyMovement[] enemies = EnemyMovement.enemies.ToArray();

      foreach (EnemyMovement enemyController in enemies) {
         GameObject enemy = enemyController.gameObject;
         Vector3 enemyDir = enemy.transform.position - turretPos;

         if (enemyDir.sqrMagnitude > sqrRange || Vector3.Angle(attackDir, enemyDir) > 50.0f) {
            continue;
         }

         if (closest == null || enemyDir.sqrMagnitude < sqrClosestDist) {
            closest = enemy;
            sqrClosestDist = enemyDir.sqrMagnitude;
         }
      }

      target = closest;
   }

   private void AttackTarget(GameObject target) {
      // if the target GameObject is destroyed, "target == null" will be true, but the target variable won't actually be null
      if (target == null) {
         target = null;
         return;
      }

      Vector3 targetDir = (target.transform.position - transform.position).normalized;

      Rotate(targetDir, 60.0f);
      if (Time.time > attackTime) {
         attackTime = Time.time + attackInterval;
         for (int i = 0; i < shootAnchor.Count; i++) { FireBullet(i); }
      }
    }

   private void Rotate(Vector3 dir, float speed) {
      Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * speed * Mathf.Deg2Rad, 0f);

      transform.rotation = Quaternion.LookRotation(newDir);
   }

   private void FireBullet(int anchorID) {
      GameObject bullet = GameObject.Instantiate(bulletPrefab, shootAnchor[anchorID].position, Quaternion.identity);
      bullet.GetComponent<BulletBehavior>().SetTarget(target);
      bullet.GetComponent<BulletBehavior>().SetSpeed(bulletSpeed);

      //This will make the bullet match the turret angle more at the beginning, supposedly
      bulletForce iniForce = new bulletForce();
      iniForce.forceMag = 5;
      iniForce.forceDec = -5;
      iniForce.forceDirection = transform.forward;
      iniForce.forceLife = 1;
      bullet.GetComponent<BulletBehavior>().AddForce(iniForce);
   }
}
