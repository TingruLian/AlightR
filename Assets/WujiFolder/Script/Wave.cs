using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using System.Transactions;

/// <summary>
/// We spawn enemies in the unit of waves
/// so the difficulty could be more flexible
/// </summary>
/// 
public class Wave : MonoBehaviour {

   [SerializeField]
   protected string waveInformation;

   [Tooltip("The delay after last wave, or from start if this is the first wave")]
   [SerializeField]
   protected float delay;

   [SerializeField]
   protected List<GameObject> enemyUnits;

   [SerializeField]
   protected EnemySpawnLocation spawnLocation;

   [SerializeField]
   protected int enemyCount;

   [SerializeField]
   protected float maxRadius = 4;

   [SerializeField]
   protected float spawnInterval = 2;

   [SerializeField]
   protected float enemySpeed = 1.5f;

   [SerializeField]
   protected UnityEvent onWaveBegin;

   [SerializeField]
   protected UnityEvent onWaveCleared;

   [Tooltip("Child waves will be spawn after the initial of this wave")]
   [SerializeField]
   protected List<Wave> childWaves;

   [SerializeField]
   protected List<EnemyMovement> currentEnemies;

   [SerializeField]
   protected int generatedCount = 0;

   protected bool started = false;

   protected List<Sequence> allSequence = new List<Sequence>();
   public void StartWave(EnemySpawner spawner) {
      spawner.GetOnGoingWaves().Add(this);
      onWaveCleared.AddListener(() => { spawner.GetOnGoingWaves().Remove(this); });

      started = true;
      generatedCount = 0;

      //The Delay
      Sequence delaySeq = DOTween.Sequence().AppendInterval(delay).
      //The Content
      AppendCallback(() => {
         spawner.InvokeWaveInformation(waveInformation);
         onWaveBegin.Invoke();

         for (int i = 0; i < enemyCount; i++) {
            EnemyMovement spawnedEnemy;
            int id = i;

            //set the delay accordinglg
            Sequence spawnSeq = DOTween.Sequence().AppendInterval(spawnInterval * i)

            //Actual spawning happens after delay
            .AppendCallback(() => {
               generatedCount++;

               spawnedEnemy = SpawnAnEnemy(spawner);
               currentEnemies.Add(spawnedEnemy);
               spawnedEnemy.AddDeathListener(() => { currentEnemies.Remove(spawnedEnemy); });
            });

            allSequence.Add(spawnSeq);
         }

         for (int i = 0; i < childWaves.Count; i++) {
            childWaves[i].StartWave(spawner);
         }

      });

      allSequence.Add(delaySeq);
   }


   /// <summary>
   /// Spawn one enemy based on the wave information
   /// Also assign the enemy to the corresponding wave
   /// </summary>
   /// <returns></returns>

   public EnemyMovement SpawnAnEnemy(EnemySpawner spawner) {
      float radius = Random.Range(0f, maxRadius);
      float angle = Random.Range(0f, Mathf.PI * 2f);

      float x = Mathf.Sin(angle) * radius;
      float z = Mathf.Cos(angle) * radius;

      Vector3 spawnCenter = spawner.GetSpwanLocation(spawnLocation).transform.position;
      Vector3 enemyPos = new Vector3(spawnCenter.x + x, spawnCenter.y, spawnCenter.z + z);

      GameObject selectedPrefab = enemyUnits[Random.Range(0, enemyUnits.Count)];
      GameObject enemyInstance = GameObject.Instantiate(selectedPrefab, enemyPos, Quaternion.identity, spawner.GetParent().transform);

      enemyInstance.GetComponent<EnemyMovement>().target = spawner.GetTarget();
      enemyInstance.GetComponent<EnemyMovement>().speed.SetBaseValue(enemySpeed);

      return enemyInstance.GetComponent<EnemyMovement>();
   }

   /// <summary>
   /// Stop the wave
   /// </summary>
   /// <returns></returns>
   public void EndWave() {
      onWaveCleared.Invoke();
      started = false;
   }

   /// <summary>
   /// Return true if the wave is going on
   /// Return false if this wave is ended
   /// </summary>
   /// <returns></returns>
   public bool Validate() {
      if (!started) {
         return false;
      }

      //if started, check if finished spawning plus all enemy killed
      bool result = !(generatedCount == enemyCount && currentEnemies.Count == 0);

      return result;
   }

   public void PauseWave()
   {
      foreach (Sequence s in allSequence)
      {
         if(s.active) { s.Pause(); }
      }
   }

   public void UnPauseWave()
   {
      foreach (Sequence s in allSequence)
      {
         if (s.active) { s.Play(); }
      }
   }

   public void AddListenerToWaveEnd(UnityAction action){onWaveCleared.AddListener(action);}

   public void RemoveListenerToWaveEnd(UnityAction action){onWaveCleared.RemoveListener(action);}
}

