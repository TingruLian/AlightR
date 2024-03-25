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

   [SerializeField]
   private SerializableLatLng cartPosition;

   private Vector2 cartStart;
   private Vector2 cartEnd;

   private bool moveCart = false;

   private GameObject cartObject = null;
   private float progress = 0f; // 0 - 1, basically a percentage

   private void Awake() {
      if (_layerObject == null) _layerObject = GetComponent<LayerGameObjectPlacement>();
   }


   private void Start() {
      cartStart = new Vector2((float)units[0].position.Latitude, (float)units[0].position.Longitude);
      cartEnd = new Vector2((float)units[units.Count - 1].position.Latitude, (float)units[units.Count - 1].position.Longitude);

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

      Vector2 cartPos = cartStart + ((cartEnd - cartStart) * progress);

      SerializableLatLng cartLatLon = new SerializableLatLng(cartPos.x, cartPos.y);

      cartObject = cart.PlaceInstance(cartLatLon, "HotMetalCartPlacementLayer").Value;
   }

   private void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
         if (!moveCart && progress < 1f) {// && cartObject != null) {
            if (cartObject != null) {
               Destroy(cartObject);
            }

            progress += .1f;

            Vector2 cartPos = cartStart + ((cartEnd - cartStart) * progress);
            SerializableLatLng cartLatLon = new SerializableLatLng(cartPos.x, cartPos.y);

            cartObject = cart.PlaceInstance(cartLatLon, "HotMetalCartPlacementLayer").Value;
         }

         moveCart = true;
      } else if (Input.GetKeyUp(KeyCode.Space)) {
         moveCart = false;
      }
   }
}