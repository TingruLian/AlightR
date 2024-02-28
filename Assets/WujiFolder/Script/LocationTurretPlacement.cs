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
      if (currentTurret != null) {
         return;
      }

      button.SetActive(false);
      currentTurret = Instantiate(turretPrefab, transform.position,transform.rotation, transform);
      currentTurret.GetComponentInChildren<TurretBehavior>().Init(turretType, attackRange);

      currentTurret.GetComponentInChildren<TurretBehavior>().AddOnDestroyLisnterner(
         () => {
            currentTurret = null;
            button.SetActive(true);
         });
   }
}
