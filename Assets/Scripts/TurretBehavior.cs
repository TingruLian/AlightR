using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurretBehavior : MonoBehaviour, Health {
   public enum Type {
      GROUND_TURRET,
      FENCE_TURRET,
      FLYING_TURRET
   }

   // TODO: Keep track of turretList in GameManager
   public static List<TurretBehavior> turretList;

   private static float ANGULAR_RANGE = 50.0f;

   public float attackInterval = .5f;
   public float bulletSpeed = 5.0f;
   public Type type { get; private set; }

   [SerializeField]
   protected int health = 5;

   [SerializeField]
   protected UnityEvent onHurt;

   [SerializeField]
   protected UnityEvent onDestroy;

   public UnityEvent onManualRotationEnd;

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

   //user rotation parameters
   protected float rotationMutiplier = 20f;
   protected bool userHodling = false;
   protected Vector2 lastHoldPosition;

   public void Init(Type type, float attackRange) {
      this.type = type;
      sqrRange = Mathf.Pow(attackRange, 2f);
   }

   public void AddOnDestroyLisnterner(UnityAction action) {
      onDestroy.AddListener(action);
   }

   void Awake() {
      if (turretList == null) {
         turretList = new List<TurretBehavior>();
      }
      turretList.Add(this);

      lastRotateTime = Time.time;
   }

   void Start() {
      // maybe find the EnemyContainer object

      target = null;
      
      attackTime = Time.time + attackInterval;

      attackDir = transform.forward;
   }

   void Update() {
      ProcessUserRotation();
      if (userHodling) return;

      if (target == null || !IsValidTarget(target.transform.position - gameObject.transform.position)) {
         SelectNewTarget();
      }

      if (target == null) {
         target = null;
      } else {
         AttackTarget(target);
      }



   }

   void ProcessUserRotation()
   {
      // let the user rotate the turret by clicking on it
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         //if (GetComponent<SphereCollider>().Raycast(ray, out hit, 100.0f) && lastRotateTime < (Time.time - .25f))
         //{
         //   // rotate the turret 90 degrees to the right
         //   transform.DORotateQuaternion(Quaternion.LookRotation(transform.right), 0.5f)
         //   .OnComplete(() => { attackDir = transform.forward; });


         //   lastRotateTime = Time.time;
         //}
         if (GetComponent<SphereCollider>().Raycast(ray, out hit, 100.0f))
         {
            userHodling = true;
            lastHoldPosition = position;
         }

      });

      if(!userHodling) return; 


      Utils.OnHold((Vector2 position, Ray ray) =>
      {
         transform.Rotate(new Vector3(0, (lastHoldPosition.x-position.x) * rotationMutiplier * Time.deltaTime, 0));
         attackDir = transform.forward;
         lastHoldPosition = position;

      });


      Utils.OnRelease((Vector2 position, Ray ray) =>
      {
         userHodling = false;
         onManualRotationEnd.Invoke();
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

   private void OnDestroy() {
      if (turretList != null) {
         turretList.Remove(this);
      }
      onDestroy.Invoke();
   }

   private bool IsValidTarget(Vector3 targetDir) {
      float sqrDist = targetDir.sqrMagnitude;
      targetDir.y = attackDir.y; // only consider the angle along the xz plane when considering whether the turret can turn towards the enemy

      return sqrDist <= sqrRange && Vector3.Angle(attackDir, targetDir) <= ANGULAR_RANGE;
   }

   private void SelectNewTarget() {
      target = null;

      GameObject closest = null;
      float sqrClosestDist = float.MaxValue;

      Vector3 turretPos = gameObject.transform.position;

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
      bullet.GetComponent<BulletBehavior>().Init(type, bulletSpeed, target);

      //This will make the bullet match the turret angle more at the beginning, supposedly
      bulletForce iniForce = new bulletForce();
      iniForce.forceMag = 5;
      iniForce.forceDec = -5;
      iniForce.forceDirection = transform.forward;
      iniForce.forceLife = 1;
      bullet.GetComponent<BulletBehavior>().AddForce(iniForce);
   }
}
