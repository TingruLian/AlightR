using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.Coordinates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public struct LocationUnit
{
   public SerializableLatLng position;
   public string name;
   public int sceneID;
   public Sprite sprite;
   public GameObject child;
}

public class LocationSpawnControl : MonoBehaviour
{
   [SerializeField] private LayerGameObjectPlacement _layerObject;
   public List<LocationUnit> units;

   private void Awake()
   {
      if (_layerObject == null) _layerObject = GetComponent<LayerGameObjectPlacement>();
   }


   private void Start()
   {
      foreach (LocationUnit unit in units)
      {
         GameObject location = _layerObject.PlaceInstance(unit.position, unit.name).Value;
         location.GetComponentInChildren<TextMeshProUGUI>().text = unit.name;
         location.GetComponent<SceneLoader>().SceneId = unit.sceneID;
         location.GetComponentInChildren<Image>().sprite = unit.sprite;

         if(unit.child != null)
         {
            Instantiate(unit.child,location.transform.position, Quaternion.identity);
         }
      }
   }


}
