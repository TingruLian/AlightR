using UnityEngine;

using TMPro;

public class GameManager : MonoBehaviour {

   public static GameManager instance { get; private set; }

   [SerializeField]
   TMP_Text tmpResources;
   UIField uiResources;

   [SerializeField]
   TMP_Text tmpLives;
   UIField uiLives;

   [SerializeField]
   private GameObject turret;

   [SerializeField]
   private float turretAttackRange;

   [SerializeField]
   private GameObject enemyContainer;

   private int resources;
   private int lives;

   private void Awake() {
      if (instance != null && instance != this)
      {
         Destroy(this);
      } else {
         instance = this;
      }
   }

   void Start() {
      resources = 20;
      lives = 3;

      uiResources = new UIField("Resources", tmpResources, resources.ToString());
      uiLives = new UIField("Lives", tmpLives, lives.ToString());
   }

   public void modifyResources(int mod) {
      resources += mod;
      uiResources.updateValue(resources.ToString());
   }

   public void modifyLives(int mod) {
      lives += mod;
      uiLives.updateValue(lives.ToString());
   }

   public void DestroyEnemy(EnemyMovement enemy) {
      Destroy(enemy.gameObject);

      modifyResources(10);
   }

   public void PlaceTurret(Vector3 pos) {
      int turretCost = 20;

      if (resources < turretCost) {
         return;
      }

      modifyResources(-turretCost);

      GameObject turretInstance = GameObject.Instantiate(turret, pos, Quaternion.identity);
      turretInstance.GetComponent<TurretBehavior>().enemyContainer = enemyContainer;
      turretInstance.GetComponent<TurretBehavior>().SetAttackRange(turretAttackRange);
   }
}
