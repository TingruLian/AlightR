using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEditor;

public enum EnemySpawnLocation
{
   air,
   ground
}

public class EnemySpawner : MonoBehaviour
{
   [SerializeField]
   SemanticColorControl colorControl;

   [SerializeField]
   protected List<Wave> mainWaves;
   [SerializeField]
   protected List<Wave> onGoingWaves;
   protected int waveId;

   [SerializeField]
   protected TextMeshProUGUI waveInfo;

   private const float MAX_ENEMIES = 20;
   private static float numEnemies = 0;

   [SerializeField]
   private List<GameObject> enemySpawnLocation;
   
   [SerializeField]
   private GameObject parent;

   // This is the target toward which enemies will move, i.e. the bridge. They will also be spawned in a radius around it. 
   [SerializeField]
   private GameObject target;

   [Header("The remianing parameter are unused")]

   [SerializeField]
   private GameObject enemy;

   [SerializeField]
   private GameObject enemy2;

   [SerializeField]
   private GameObject enemy3;

   // This is the max distance from the target that enemies will spawn
   [SerializeField]
   private float maxRadius;

   [SerializeField]
   private float spawnInterval;

   [SerializeField]
   private float enemySpeed;

   private float spawnTime;

   private Vector3 spawnCenter;
   private Vector3 forwardAxis;
   private Vector3 sideAxis;


   private void Awake()
   {
      if (colorControl == null) { colorControl = SemanticColorControl.GetInstance(); }
   }

   void Start() {
      Vector3 targetPos = target.transform.position;

      forwardAxis = new Vector3(-6f, 0f, -1.9f).normalized;
      sideAxis = new Vector3(forwardAxis.z, 0, -forwardAxis.x).normalized;

      //SpawnDirectionIndicators();

      spawnTime = Time.time + spawnInterval;


      //We start the first wave
      onGoingWaves = new List<Wave>();
      waveId = 0;
      mainWaves[waveId].StartWave(this);
      if (colorControl != null) { colorControl.SetToWaveColor(waveId);}
   }

   void Update() {

      //Update each ongoing waves
      //back ward iteration because waves might be removed in progress
      for (int i = onGoingWaves.Count - 1; i >= 0; i--) {
         Wave w = onGoingWaves[i];

         if (!w.Validate()) {
            w.EndWave(); 
         }
      }


      //If there's no wave, check if we shall start a new one, or end 
      if (onGoingWaves.Count == 0) {

         waveId++;
         
         if (waveId < mainWaves.Count) {
            mainWaves[waveId].StartWave(this);
            if (colorControl != null) { colorControl.SetToWaveColor(waveId); }

         } else {
            onGoingWaves = null;
            InvokeWaveInformation("All Waves Cleared");
            if (colorControl != null) { colorControl.SetToVicColor(); }

            DOTween.Sequence().AppendInterval(2)
            .AppendCallback(() => {
               GameManager.instance.AttemptWin();   
            });
         }
      }
   }


   // This method spawns different colored enemies to help visualize the axes
   void SpawnDirectionIndicators() {
      spawnCenter = enemySpawnLocation[0].transform.position;

      Vector3 enemyPos = spawnCenter;
      GameObject.Instantiate(enemy3, enemyPos, Quaternion.identity, parent.transform);

      enemyPos = spawnCenter + (-forwardAxis * 12);
      GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

      enemyPos = spawnCenter + (forwardAxis * 12);
      GameObject.Instantiate(enemy2, enemyPos, Quaternion.identity, parent.transform);

      enemyPos = spawnCenter + (-sideAxis * 3);
      GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

      enemyPos = spawnCenter + (sideAxis * 3);
      GameObject.Instantiate(enemy2, enemyPos, Quaternion.identity, parent.transform);
   }

   public void InvokeWaveInformation(string data) {
      waveInfo.gameObject.SetActive(true);

      //make visiable
      waveInfo.DOFade(1, 0);
      waveInfo.text = data;
      waveInfo.transform.DOPunchScale(new Vector3(1.2f,1.2f,1.2f), 0.25f, 10, 1);

      //fade out and turn off
      waveInfo.DOFade(0, 0.5f).SetDelay(2f).OnComplete(() => { waveInfo.gameObject.SetActive(false); });
   }

   public GameObject GetParent() {
      return parent;
   }

   public GameObject GetTarget() {
      return target;
   }

   public GameObject GetSpwanLocation(EnemySpawnLocation point) {
      return enemySpawnLocation[(int)point];
   }

   public List<Wave> GetOnGoingWaves() {
      return onGoingWaves;
   }
}
