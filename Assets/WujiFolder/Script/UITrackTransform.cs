using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrackTransform : MonoBehaviour
{
    [SerializeField] public Transform lookAt;
    [SerializeField] public Vector3 offset;
    [SerializeField] public Camera targetCamera;

    private void Awake()
    {
        if(targetCamera == null) targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        Vector3 pos = targetCamera.WorldToScreenPoint(lookAt.position + offset);

        if (transform.position!=pos) 
        {
            transform.position = new Vector3(pos.x,pos.y,0);
        }
    }
}
