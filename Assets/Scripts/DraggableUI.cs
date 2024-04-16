using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform canvasTransform;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float scale;
    [SerializeField] protected Image m_image;

    private bool isDragging = false;
    private  GameObject instantiatedObject;
    void Start()
    {
        m_image = GetComponent<Image>();
        canvasTransform = GetComponentInParent<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        isDragging = true;

        Instantiate3DObjectAt(Input.mousePosition);

        if(m_image != null) { m_image.enabled =false; }
    }

    public void OnDrag(PointerEventData eventData)
    {

        transform.position = Input.mousePosition;
        if (instantiatedObject == null) return;

        Ray newRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(newRay, out hit))
        {
            instantiatedObject.transform.position = hit.point;
            instantiatedObject.transform.LookAt(Camera.main.transform);
        }

        //if (isDragging)
        //{
        //    Instantiate3DObjectAt(transform.position);
        //}
        //else
        //{
        //    instantiatedObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //}
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        transform.position = startPosition; // Reset UI position or destroy it.

        if (m_image != null) { m_image.enabled = true; }
    }

    void Instantiate3DObjectAt(Vector3 position)
    {
        //// Convert screen position to world position
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        //worldPosition.z = 0; // Adjust this depending on your scene setup

        //// Instantiate the prefab at (0,0,0) with no rotation
        //GameObject instantiatedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        //// After instantiation, move it to the desired world position
        //instantiatedObject.transform.position = worldPosition;

        // Instantiate the prefab at (0,0,0) with no rotation
        instantiatedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        Ray newRay = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if(Physics.Raycast(newRay,out hit))
        {
            // After instantiation, move it to the desired world position
            instantiatedObject.transform.position = hit.point;
            instantiatedObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            instantiatedObject.transform.localScale = new Vector3(1f,1f,1f)* scale;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}