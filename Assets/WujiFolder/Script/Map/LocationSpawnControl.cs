using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.Coordinates;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LocationUnit
{
   public SerializableLatLng position;
   public string name;
   public int sceneID;
   public GameObject child;
   public int levelId;
   public int preRequisiteLevelId;
}

public class LocationSpawnControl : MonoBehaviour
{
   [SerializeField]
   private LayerGameObjectPlacement _layerObject;

   public List<LocationUnit> units;

   [SerializeField]
   private GameScriptableObject GameData;

   [SerializeField]
   private LayerGameObjectPlacement cart;

   private GameObject cartObject = null;

   private void Awake() {
      if (_layerObject == null) {
         _layerObject = GetComponent<LayerGameObjectPlacement>();
      }
   }


   public void SpawnLocations() {
      foreach (LocationUnit unit in units) {
         GameObject location = _layerObject.PlaceInstance(unit.position, unit.name).Value;
         location.GetComponent<SceneLoader>().SceneId = unit.sceneID;
         if (location.GetComponent<CompletionCheck>() != null) {
            location.GetComponent<CompletionCheck>().levelId = unit.preRequisiteLevelId;
         }

         if (unit.child != null) {
            GameObject c = Instantiate(unit.child,location.transform.position, Quaternion.identity);
            if (c.GetComponent<CompletionCheck>() != null) {
               c.GetComponent<CompletionCheck>().levelId = unit.levelId;
            }
         }
      }

      SpawnCart();
   }

   private void SpawnCart() {
      Vector2 cartStart = new Vector2((float)units[0].position.Latitude, (float)units[0].position.Longitude);
      Vector2 cartEnd = new Vector2((float)units[units.Count - 1].position.Latitude, (float)units[units.Count - 1].position.Longitude);

      // these are used as a lazy way to convert from LatLon to Unity 3D world coords
      GameObject startObject = cart.PlaceInstance(new SerializableLatLng(cartStart.x, cartStart.y), "HotMetalCartPlacementLayer").Value;
      GameObject endObject = cart.PlaceInstance(new SerializableLatLng(cartEnd.x, cartEnd.y), "HotMetalCartPlacementLayer").Value;

      cartObject = cart.PlaceInstance(new SerializableLatLng(cartStart.x, cartStart.y), "HotMetalCartPlacementLayer").Value;
      cartObject.SetActive(true);
      CartMovement cartController = cartObject.transform.GetChild(0).GetComponent<CartMovement>();

      cartObject.transform.GetChild(0).gameObject.SetActive(true);

      cartController.start = startObject.transform.position;
      cartController.end = endObject.transform.position;
   }
}