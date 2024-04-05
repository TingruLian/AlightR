using System;
using System.Collections.Generic;

using UnityEngine;

using Niantic.Lightship.Maps.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;

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

public class LocationSpawnControl : MonoBehaviour {
   public List<LocationUnit> units;

   [SerializeField]
   private LayerGameObjectPlacement _layerObject;

   [SerializeField]
   private GameScriptableObject GameData;

   public void SpawnLocations() {
      if (_layerObject == null) {
         _layerObject = GetComponent<LayerGameObjectPlacement>();
      }

      foreach (LocationUnit unit in units) {
         GameObject location = _layerObject.PlaceInstance(unit.position, unit.name).Value;
         location.GetComponent<SceneLoader>().SceneId = unit.sceneID;
         if (location.GetComponent<CompletionCheck>() != null) {
            Debug.Log("Initing PARENT");
            location.GetComponent<CompletionCheck>().levelId = unit.preRequisiteLevelId;
         }

         if (unit.child != null) {
            GameObject c = Instantiate(unit.child,location.transform.position, Quaternion.identity);
            if (c.GetComponent<CompletionCheck>() != null) {
               Debug.Log("Initing CHILD");
               c.GetComponent<CompletionCheck>().levelId = unit.levelId;
            }
         }
      }
   }
}