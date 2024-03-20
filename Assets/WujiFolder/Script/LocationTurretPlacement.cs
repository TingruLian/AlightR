using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

   [SerializeField]
   private Image CDBar;
   [SerializeField]
   private GameObject CDBarContainer;

   [SerializeField]
   private float manualCD = 3f;
   [SerializeField]
   private float destroyedCD = 6f;

   private void Awake()
   {
      CDBarContainer.SetActive(false);
   }

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


      CDBarContainer.SetActive(true);

      float cd = 0;
      if(byPlayer ) { cd = manualCD; }
      else { cd = destroyedCD; }

      CDBar.DOFillAmount(0, cd).From(1).OnComplete(() =>
      {
         CDBarContainer.SetActive(false);
         button.SetActive(true);
      });


   }
}
