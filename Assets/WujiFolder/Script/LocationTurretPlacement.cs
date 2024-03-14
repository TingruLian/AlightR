using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTurretPlacement : MonoBehaviour {

   [SerializeField]
   private float attackRange;

   [SerializeField]
   private GameObject turretPrefab;

   [SerializeField]
   private TurretBehavior.Type turretType;

   [SerializeField]
   private GameObject button;

   private GameObject currentTurret;


   public void SpawnTurret() {

      bool canSpawn = GameManager.instance.RequestTurret(this);
      if (!canSpawn) return;

      if (currentTurret != null) {
         return;
      }

      button.SetActive(false);
      currentTurret = Instantiate(turretPrefab, transform.position,transform.rotation, transform);
      currentTurret.GetComponentInChildren<TurretBehavior>().Init(turretType, attackRange);

      currentTurret.GetComponentInChildren<TurretBehavior>().AddOnDestroyLisnterner
         (() => { RemoveTurret(false); });
   }

   public void RemoveTurret(bool byPlayer)
   {
      if(currentTurret == null) { return; }
      
      GameManager.instance.FreeTurret(this);
      Destroy(currentTurret);
      currentTurret = null; 
      button.SetActive(true);
   }
}
