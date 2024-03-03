using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using TMPro;

/// <summary>
/// We spawn enemies in the unit of waves
/// so the difficulty could be more fexible
/// </summary>
/// 
[Serializable]
public struct EnemyWave
{
   public string name;
   public string waveInformation;
   [Tooltip("The delay after last wave, or from start if this is the first wave")]
   public float delay;

   public List<GameObject> enemyUnits;
   public GameObject enemySpawn;
   public GameObject parent;
   public int enemyCount;
   public float maxRadius;
   public float spawnInterval;
   public float enemySpeed;

   public UnityEvent onWaveBegin;
   public UnityEvent onWaveCleared;
   [Tooltip("Child waves will be spawn after the initial of this wave")]
   public List<EnemyWave> childWaves;
}


public class EnemySpawner : MonoBehaviour
{
   [SerializeField]
   protected List<EnemyWave> mainWaves;
   protected List<EnemyWave> onGoingWaves;
   protected int waveId;

   [SerializeField]
   protected TextMeshProUGUI waveInfo;

   private const float MAX_ENEMIES = 20;
   private static float numEnemies = 0;

   [SerializeField]
   private GameObject enemySpawn;

   [SerializeField]
   private GameObject enemy;

   [SerializeField]
   private GameObject enemy2;

   [SerializeField]
   private GameObject enemy3;

   // This is the parent GameObject for all spawned enemies. Should be either XR Origin or a child of it
   [SerializeField]
   private GameObject parent;

   // This is the target toward which enemies will move, i.e. the bridge. They will also be spawned in a radius around it. 
   [SerializeField]
   private GameObject target;

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


   void Start()
   {

      Vector3 targetPos = target.transform.position;

      forwardAxis = new Vector3(-6f, 0f, -1.9f).normalized;
      sideAxis = new Vector3(forwardAxis.z, 0, -forwardAxis.x).normalized;

      //SpawnDirectionIndicators();

      spawnTime = Time.time + spawnInterval;




      //We start the first wave
      onGoingWaves = new List<EnemyWave>();
      waveId = 0;
      StartAWave(mainWaves[waveId]);

   }

   void Update()
   {
      //Try to start a new wave if no wave is going on
      if(onGoingWaves!=null && onGoingWaves.Count == 0) {

         waveId ++;
         
         //if we still have remaining waves
         if(waveId < mainWaves.Count) {
            StartAWave(mainWaves[waveId]);
         }
         //end of the game
         else
         {
            onGoingWaves = null;
            InvokeWaveInformation("You Win");
         }
      }

      //---------------Legacy Spawn Code------------//
      /*
      if (Time.time > spawnTime && numEnemies < MAX_ENEMIES)
      {
          spawnTime = Time.time + spawnInterval;

          float radius = Random.Range(0f, maxRadius);
          float angle = Random.Range(0f, Mathf.PI * 2f);

          //Debug.Log($"Selected radius: {radius}");
          //Debug.Log($"Selected angle: {angle}");

          float x = Mathf.Sin(angle) * radius;
          float z = Mathf.Cos(angle) * radius;

          spawnCenter = enemySpawn.transform.position;
          Vector3 enemyPos = new Vector3(spawnCenter.x + x, spawnCenter.y, spawnCenter.z + z);

          Vector3 targetPos = spawnCenter - forwardAxis * 20 + new Vector3(0f, 4f, 0f);

          GameObject enemyInstance = GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

          enemyInstance.GetComponent<EnemyMovement>().target = target;
          enemyInstance.GetComponent<EnemyMovement>().speed.SetBaseValue(enemySpeed);

          numEnemies++;
      }*/

   }

   //Most code are copied from old spawn code, with modification of getting value from the wave
   EnemyMovement SpawnAnEnemy(EnemyWave wave)
   {
      float radius = Random.Range(0f, wave.maxRadius);
      float angle = Random.Range(0f, Mathf.PI * 2f);

      float x = Mathf.Sin(angle) * radius;
      float z = Mathf.Cos(angle) * radius;

      spawnCenter = wave.enemySpawn.transform.position;
      Vector3 enemyPos = new Vector3(spawnCenter.x + x, spawnCenter.y, spawnCenter.z + z);
      Vector3 targetPos = spawnCenter - forwardAxis * 20 + new Vector3(0f, 4f, 0f);

      GameObject selectedPrefab = wave.enemyUnits[Random.Range(0, wave.enemyUnits.Count)];
      GameObject enemyInstance = GameObject.Instantiate(selectedPrefab, enemyPos, Quaternion.identity, wave.parent.transform);

      enemyInstance.GetComponent<EnemyMovement>().target = target;
      enemyInstance.GetComponent<EnemyMovement>().speed.SetBaseValue(wave.enemySpeed);

      return enemyInstance.GetComponent<EnemyMovement>();
   }


   void StartAWave(EnemyWave wave)
   {
      onGoingWaves.Add(wave);
      wave.onWaveCleared.AddListener(() => { onGoingWaves.Remove(wave); });   

      //The Delay
      DOTween.Sequence().AppendInterval(wave.delay).
      //The Content
      AppendCallback(() =>
      {
         InvokeWaveInformation(wave.waveInformation);
         wave.onWaveBegin.Invoke();

         for (int i = 0; i < wave.enemyCount; i++)
         {
            EnemyMovement spawnedEnemy;
            int id = i;

            //set the delay accordinglg
            DOTween.Sequence().AppendInterval(wave.spawnInterval * i)
            //Actual spawning happens after delay
            .AppendCallback(() => {
               spawnedEnemy = SpawnAnEnemy(wave);

               //Last spawned enemy should know if dead
               //A potential bug is that last enemy might not be last one to be killed
               if (id >= wave.enemyCount - 1) { 
                  spawnedEnemy.AddDeathListener(() => { wave.onWaveCleared.Invoke(); }); 
               }
            });
         }

         for(int i = 0; i < wave.childWaves.Count; i++)
         {
            StartAWave(wave.childWaves[i]);
         }

      });

   }

   // This method spawns different colored enemies to help visualize the axes
   void SpawnDirectionIndicators()
   {
      spawnCenter = enemySpawn.transform.position;

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

   void InvokeWaveInformation(string data)
   {
      waveInfo.gameObject.SetActive(true);

      //make visiable
      waveInfo.DOFade(1, 0);
      waveInfo.text = data;
      waveInfo.transform.DOPunchScale(new Vector3(1.2f,1.2f,1.2f), 0.25f, 10, 1);

      //fade out and turn off
      waveInfo.DOFade(0, 0.5f).SetDelay(2f).OnComplete(() => { waveInfo.gameObject.SetActive(false); });
   }
}
