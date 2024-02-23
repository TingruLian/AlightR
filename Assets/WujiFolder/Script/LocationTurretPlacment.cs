using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTurretPlacment : MonoBehaviour
{
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private GameObject currentTurret;
    [SerializeField] private GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTurret()
    {
        if(currentTurret != null) { return; }

        button.SetActive(false);
        currentTurret = Instantiate(turretPrefab, transform.position,transform.rotation, transform);
        currentTurret.GetComponentInChildren<TurretBehavior>().SetAttackRange(attackRange);
    }
}
