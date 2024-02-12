using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.Coordinates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct LocationUnit
{
    public SerializableLatLng position;
    public string name;
    public int sceneID;
}

public class LocationSpawnControl : MonoBehaviour
{
    [SerializeField] private LayerGameObjectPlacement _layerObject;
    public List<LocationUnit> units;

    private void Awake()
    {
        if(_layerObject == null) _layerObject = GetComponent<LayerGameObjectPlacement>();
    }


    private void Start()
    {
        foreach(LocationUnit unit in units)
        {
            Debug.Log("placed a unit");
            GameObject location = _layerObject.PlaceInstance(unit.position, unit.name).Value;
            location.GetComponentInChildren<TextMeshProUGUI>().text = unit.name;
            location.GetComponent<SceneLoader>().SceneId = unit.sceneID;
        }
    }


}