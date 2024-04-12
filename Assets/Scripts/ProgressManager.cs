using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;

public class ProgressManager : MonoBehaviour {

   public static ProgressManager instance;

   [SerializeField]
   private LayerGameObjectPlacement cart;

   [SerializeField]
   private LayerGameObjectPlacement rails;

   private GameObject player;
   private GameObject cartObject = null;
   private GameObject railsObject = null;

   private MapManager mapManager;

   public int lastDefeatedTurret = 0;
   public Vector3 cartPos;
   public bool cartMoved = false;
   public float cartProgress;

   public void SpawnCart() {
      Debug.LogWarning("Cart getting spawned");
      List<LocationUnit> units = mapManager.locationSpawner.units;

      SerializableLatLng startLatLng = units[0].GetPosition();
      SerializableLatLng endLatLng = units[units.Count - 1].GetPosition();

      Vector2 cartStart = new Vector2((float)startLatLng.Latitude, (float)startLatLng.Longitude);
      Vector2 cartEnd = new Vector2((float)endLatLng.Latitude, (float)endLatLng.Longitude);

      // these are used as a lazy way to convert from LatLon to Unity 3D world coords
      GameObject startObject = cart.PlaceInstance(new SerializableLatLng(cartStart.x, cartStart.y), "HotMetalCartPlacementLayer").Value;
      GameObject endObject = cart.PlaceInstance(new SerializableLatLng(cartEnd.x, cartEnd.y), "HotMetalCartPlacementLayer").Value;

      railsObject = rails.PlaceInstance(new SerializableLatLng(cartStart.x, cartStart.y), "RailPlacementLayer").Value;

      railsObject.transform.position = new Vector3(10.4f, 1.0f, -64.9f);
      railsObject.transform.rotation = Quaternion.Euler(new Vector3(railsObject.transform.rotation.x, -62, railsObject.transform.rotation.z));

      railsObject.SetActive(true);

      cartObject = cart.PlaceInstance(new SerializableLatLng(cartStart.x, cartStart.y), "HotMetalCartPlacementLayer").Value;
      cartObject.SetActive(true);

      CartMovement cartController = cartObject.transform.GetChild(0).GetComponent<CartMovement>();

      cartController.lastDefeatedTurretId = lastDefeatedTurret;
      cartController.UpdateColor(cartController.lastDefeatedTurretId);
      cartController.progress = cartProgress;

      cartObject.transform.GetChild(0).gameObject.SetActive(true);

      cartController.start = startObject.transform.position;
      cartController.end = endObject.transform.position;

      Vector3 cartFwd = (cartController.end - cartController.start).normalized;
      Vector3 cartRight = Quaternion.AngleAxis(90, Vector3.up) * cartFwd;

      Vector3 offset = cartRight * 10.0f;

      if (cartMoved) {
         cartObject.transform.position = cartPos;
      } else {
         cartObject.transform.position = cartObject.transform.position + offset;
      }

      cartController.start += offset;
      cartController.end += offset;
   }

   private void Awake() {
      if (instance != null) {
         Destroy(gameObject);
         return;
      }

      instance = this;

      SceneManager.sceneLoaded += OnSceneLoaded;
      DontDestroyOnLoad(gameObject);
   }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
      if (scene.name == "WorldMap") {
         mapManager = MapManager.instance;
      }
   }
}
