using UnityEngine;

using Niantic.Lightship.Maps;

public class MapManager : MonoBehaviour {

   public static MapManager instance;

   public LocationSpawnControl locationSpawner;

   public GameScriptableObject GameData;

   [SerializeField]
   private LightshipMapManager mapManager;

   [SerializeField]
   private GameObject player;

   [SerializeField]
   private ArrowSpawnControl arrowSpawner;

   private GameObject cartObject = null;

   private int initState = 0;
   private float startTime;
   private float waitTime;

   private void Awake() {
      if (instance != null) {
         Destroy(gameObject);
         return;
      }

      instance = this;
   }

   private void Update() {
      if (initState == 0 && mapManager.IsInitialized) {
         initState++;
         startTime = Time.time;
      }

      if (initState == 1 && Time.time > (startTime + waitTime)) {
         initState++;

         player.SetActive(true);
         arrowSpawner.DrawArrows();
         locationSpawner.SpawnLocations();

         ProgressManager.instance.SpawnCart();
      }
   }
}
