using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    private int placedPrefabCount;

    public GameObject PlacedPrefab
    {
        get 
        {
            return placedPrefab;
        }
        set 
        {
            placedPrefab = value;
        }
    }

    private ARRaycastManager arRaycastManager;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {


                    var hitPose = hits[0].pose;
                    Instantiate(placedPrefab, hitPose.position, hitPose.rotation);

                    placedPrefabCount++;
            }
        }
    }

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}
