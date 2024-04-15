using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform canvasTransform;
    [SerializeField] private GameObject prefab;

    void Start()
    {
        canvasTransform = GetComponentInParent<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Instantiate3DObjectAt(transform.position);
        transform.position = startPosition; // Reset UI position or destroy it.
    }

    void Instantiate3DObjectAt(Vector3 position)
    {
        // Convert screen position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        worldPosition.z = 0; // Adjust this depending on your scene setup

        // Instantiate the prefab at (0,0,0) with no rotation
        GameObject instantiatedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        // After instantiation, move it to the desired world position
        instantiatedObject.transform.position = worldPosition;
    }
}