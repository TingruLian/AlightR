using Niantic.Lightship.Maps;

using UnityEngine;

public class MapManager : MonoBehaviour {

   [SerializeField]
   private LightshipMapManager mapManager;

   [SerializeField]
   private GameObject player;

   [SerializeField]
   private LocationSpawnControl locationSpawner;

   [SerializeField]
   private ArrowSpawnControl arrowSpawner;

   private int initState = 0;
   private float startTime;
   private float waitTime = .5f;

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
