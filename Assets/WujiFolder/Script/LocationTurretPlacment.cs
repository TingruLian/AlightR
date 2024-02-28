using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTurretPlacment : MonoBehaviour {

   [SerializeField]
   private float attackRange;

   [SerializeField]
   private GameObject turretPrefab;

   [SerializeField]
   private GameObject button;

   private GameObject currentTurret;

   public void SpawnTurret() {
      if (currentTurret != null) {
         return;
      }

      button.SetActive(false);
      currentTurret = Instantiate(turretPrefab, transform.position,transform.rotation, transform);
      currentTurret.GetComponentInChildren<TurretBehavior>().SetAttackRange(attackRange);
      currentTurret.GetComponentInChildren<TurretBehavior>().AddOnDestroyLisnterner(
         () => {
            Debug.Log("Turret got placed");
            currentTurret = null;
            button.SetActive(true);
         });
   }
}
