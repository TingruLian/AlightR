using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurretBehavior : MonoBehaviour, Health {
   // TODO: Keep track of turretList in GameManager
   public static List<TurretBehavior> turretList;

   private static float ANGULAR_RANGE = 50.0f;

   public GameObject enemyContainer;
   public float attackInterval = .5f;
   public float bulletSpeed = 5.0f;

   [SerializeField]
   protected int health = 5;

   [SerializeField]
   protected UnityEvent onHurt;

   [SerializeField]
   protected UnityEvent onDestroy;

   [SerializeField]
   private GameObject bulletPrefab;

   [SerializeField]
   private List<Transform> shootAnchor;

   [SerializeField]
   private GameObject target;

   private float sqrRange;
   private Vector3 attackDir;
   private float attackTime;
   private float lastRotateTime;

   private void Awake() {
      if (turretList == null) {
         turretList = new List<TurretBehavior>();
      }
      turretList.Add(this);

      lastRotateTime = Time.time;
   }

   private void OnDestroy() {
      if (turretList != null) {
         turretList.Remove(this);
      }
      onDestroy.Invoke();
   }

   public void AddOnDestroyLisnterner(UnityAction action) {
      onDestroy.AddListener(action);
   }

   void Start() {
      // maybe find the EnemyContainer object

      target = null;
      
      attackTime = Time.time + attackInterval;

      attackDir = transform.forward;
   }

   void Update() {
      if (target == null || !IsValidTarget(target.transform.position - gameObject.transform.position)) {
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

         if (GetComponent<SphereCollider>().Raycast(ray, out hit, 100.0f) && lastRotateTime  < (Time.time - .5f)) {
            // rotate the turret 90 degrees to the right
            transform.rotation = Quaternion.LookRotation(transform.right);
            attackDir = transform.forward;

            lastRotateTime = Time.time;
         }
      });
   }

   public void SetAttackRange(float attackRange) {
      sqrRange = Mathf.Pow(attackRange, 2f);
   }

   public void TakeDamage(int damage) {
      health -= damage;
      onHurt.Invoke();

      if (health <= 0) {
         Destroy(transform.parent.gameObject);
      }
   }

   private bool IsValidTarget(Vector3 targetDir) {
      return targetDir.sqrMagnitude <= sqrRange && Vector3.Angle(attackDir, targetDir) <= ANGULAR_RANGE;
   }

   private void SelectNewTarget() {
      target = null;

      GameObject closest = null;
      float sqrClosestDist = float.MaxValue;

      Vector3 turretPos = gameObject.transform.position;

      //EnemyMovement[] enemies = enemyContainer.GetComponentsInChildren<EnemyMovement>();
      if (EnemyMovement.enemies == null) {
         return;
      }
      EnemyMovement[] enemies = EnemyMovement.enemies.ToArray();

      foreach (EnemyMovement enemyController in enemies) {
         GameObject enemy = enemyController.gameObject;
         Vector3 enemyDir = enemy.transform.position - turretPos;

         if (!IsValidTarget(enemyDir)) {
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

      if (!IsValidTarget(targetDir)) {
         target = null;
         return;
      }

      // continue rotating to face the target
      Rotate(targetDir, 60.0f);

      // possibly check that the turret is almost directly facing the enemy before firing
      if (Time.time > attackTime) { // && Vector3.Angle(attackDir, targetDir) <= 5.0f) {
         attackTime = Time.time + attackInterval;

         for (int i = 0; i < shootAnchor.Count; i++) {
            FireBullet(i);
         }
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
