using Niantic.Lightship.Maps.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;

using UnityEngine;

using System.Collections.Generic;
using System;

[Serializable]
public struct ArrowUnit {
   public SerializableLatLng position;
   public string name;
   public bool draw;
}
public class ArrowSpawnControl : MonoBehaviour {
   public List<ArrowUnit> arrowInformations;
   public List<GameObject> arrows;

   [SerializeField]
   private LayerGameObjectPlacement _layerObject;

   public void DrawArrows() {
      if (_layerObject == null) {
         _layerObject = GetComponent<LayerGameObjectPlacement>();
      }
      if (_layerObject == null) {
         Debug.LogWarning("Still null");
      }

      for (int i = 0; i < arrowInformations.Count; i++) {
         GameObject arrow = _layerObject.PlaceInstance(arrowInformations[i].position, arrowInformations[i].name).Value;
         arrows.Add(arrow);

         if (i != 0) {
            arrow.transform.LookAt(arrows[i - 1].transform);
         }

         if (!arrowInformations[i].draw) {
            foreach (Transform g in arrow.GetComponentsInChildren<Transform>()) {
               Destroy(g.gameObject);
            }
         }
      }
   }
}
