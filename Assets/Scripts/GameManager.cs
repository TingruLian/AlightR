using UnityEngine;

using TMPro;
using UnityEngine.Events;

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
   private GameObject turretPlaceholder;

   [SerializeField]
   private float turretAttackRange;

   [SerializeField]
   private GameObject enemyContainer;

   private int resources;
   private int lives;

    public UnityEvent onResourceEnough;
    public UnityEvent onResourceNotEnough;
   public UnityEvent onInitialPlace;
   public UnityEvent onCompletePlace;

   // There should only be one turrent placeholder in the scene at a time
   private GameObject placeholderInstance;

   private const int TURRET_COST = 20;

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

      if(resources >= TURRET_COST) { onResourceEnough.Invoke(); }
      else { onResourceNotEnough.Invoke(); }
   }

   public void ModifyResources(int mod) {
      resources += mod;
      uiResources.updateValue(resources.ToString());

      if (resources >= TURRET_COST) { onResourceEnough.Invoke(); }
      else { onResourceNotEnough.Invoke(); }
    }

   public void ModifyLives(int mod) {
      lives += mod;
      uiLives.updateValue(lives.ToString());
   }

   public void DestroyEnemy(EnemyMovement enemy) {
      Destroy(enemy.gameObject);

      ModifyResources(10);
   }

   public GameObject GetPlaceholderInstance() {
      return placeholderInstance;
   }

   public void ClearPlaceholderInstance() {
      Destroy(placeholderInstance);
      placeholderInstance = null;
   }

   public void PlaceTurretInitial(Vector3 pos) {
      // check that the player has enough resources, but don't remove them until the player confirms placement
      if (resources < TURRET_COST) {
         return;
      }

      if (placeholderInstance != null) {
         Debug.LogError("There should not currently be any instances of the turret placeholder");
      }

      placeholderInstance = GameObject.Instantiate(turretPlaceholder, pos, Quaternion.identity);
      onInitialPlace.Invoke();
   }

   public void PlaceTurretConfirm() {
      if (placeholderInstance == null) {
         Debug.LogError("There should be an instance of the turret placeholder in the scene");
      }

      if (resources < TURRET_COST) {
         return;
      }

      ModifyResources(-TURRET_COST);

      Vector3 pos = placeholderInstance.transform.position;

      ClearPlaceholderInstance();

      GameObject turretInstance = GameObject.Instantiate(turret, pos, Quaternion.identity).transform.GetChild(0).gameObject;
      turretInstance.GetComponent<TurretBehavior>().enemyContainer = enemyContainer;
      turretInstance.GetComponent<TurretBehavior>().SetAttackRange(turretAttackRange);

      onCompletePlace.Invoke();
   }
}
