using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.Coordinates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;

[Serializable]
public struct ArrowUnit
{
    public SerializableLatLng position;
    public string name;
    public bool draw;
}
public class ArrowSpawnControl : MonoBehaviour
{
    [SerializeField] private LayerGameObjectPlacement _layerObject;
    public List<ArrowUnit> arrowInformations;
    public List<GameObject> arrows;

    private void Awake()
    {
        if (_layerObject == null) _layerObject = GetComponent<LayerGameObjectPlacement>();
    }


    private void Start()
    {
        for(int i = 0; i < arrowInformations.Count; i++)
        {
            Debug.Log("placed object");
            GameObject arrow = _layerObject.PlaceInstance(arrowInformations[i].position, arrowInformations[i].name).Value;
            arrows.Add(arrow);

            if(i != 0)
            {
                arrow.transform.LookAt(arrows[i - 1].transform);
            }

            if (!arrowInformations[i].draw) 
            { 
                foreach(Transform g in arrow.GetComponentsInChildren<Transform>())
                {
                    Destroy(g.gameObject);
                }
            }
        }
    }
}
