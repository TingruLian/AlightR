using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchPointReader : MonoBehaviour
{
    [Header("Dependency")]
    public Camera mainCamera;
    public PlayerInput playerInput;
    private InputAction primaryTouch;

    [Header("Attributes")]
    public LayerMask mask;

    [Header("Data")]
    public Vector3 hitPoint;
    public List<Action<Vector3>> onHitActions = new List<Action<Vector3>>();


    private void Awake()
    {
        if(mainCamera == null) mainCamera= Camera.main;
        if(playerInput == null) playerInput = GetComponent<PlayerInput>();

        primaryTouch = playerInput.actions["Point"];
    }

    private void Update()
    {
        ProcessTouchInput();
    }


    void ProcessTouchInput()
    {
        //no process if no input
        if (!primaryTouch.WasPerformedThisFrame()) return;

        Ray ray = mainCamera.ScreenPointToRay(primaryTouch.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
            hitPoint = hit.point;
            Debug.Log(hitPoint);
            OnHit();
        }
    }

    void OnHit()
    {
        foreach(Action<Vector3> action in onHitActions)
        {
            action.Invoke(hitPoint);
        }
    }
}
