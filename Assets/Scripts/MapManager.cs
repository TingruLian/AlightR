using UnityEngine;
using UnityEngine.SceneManagement;

using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;

public class MapManager : MonoBehaviour {

   public static MapManager instance;

   public LocationSpawnControl locationSpawner;

   [SerializeField]
   private LightshipMapManager mapManager;

   [SerializeField]
   private GameObject player;

   [SerializeField]
   private ArrowSpawnControl arrowSpawner;

   [SerializeField]
   private LayerGameObjectPlacement cart;

   private GameObject cartObject = null;

   private int initState = 0;
   private float startTime;
   private float waitIncrement = .5f;
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
      }
   }
}
