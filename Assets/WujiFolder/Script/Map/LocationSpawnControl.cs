using System;
using System.Collections.Generic;

using UnityEngine;

using Niantic.Lightship.Maps.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;

[Serializable]
public struct LocationUnit {
   public SerializableLatLng position;
   public string name;
   public int sceneID;
   public GameObject child;
   public int levelId;
   public int preRequisiteLevelId;

   private static SerializableLatLng[] bridgeLatLngs = new SerializableLatLng[] {
      new SerializableLatLng(40.428861278611734, -79.959708921690151),
      new SerializableLatLng(40.428335442151365, -79.960883894297069),
      new SerializableLatLng(40.427771020406936, -79.961830040750016),
   };
   
   private static SerializableLatLng[] rpisLatLngs = new SerializableLatLng[] {
      new SerializableLatLng(40.432640472116539, -79.9651089455859),
      new SerializableLatLng(40.432571272305907, -79.964884694749642),
      new SerializableLatLng(40.432485464441775, -79.964691960247109),
   };

   public SerializableLatLng GetPosition() {
      //return bridgeLatLngs[sceneID - 1];
      return rpisLatLngs[sceneID - 1];
   }
}

public class LocationSpawnControl : MonoBehaviour {
   public List<LocationUnit> units;

   [SerializeField]
   private LayerGameObjectPlacement _layerObject;

   public void SpawnLocations() {
      if (_layerObject == null) {
         _layerObject = GetComponent<LayerGameObjectPlacement>();
      }

      foreach (LocationUnit unit in units) {
         GameObject location = _layerObject.PlaceInstance(unit.GetPosition(), unit.name).Value;
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