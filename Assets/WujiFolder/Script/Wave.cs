using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// We spawn enemies in the unit of waves
/// so the difficulty could be more fexible
/// </summary>
/// 
[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Waves", order = 1)]
public class Wave : ScriptableObject
{
   public string waveInformation;
   [Tooltip("The delay after last wave, or from start if this is the first wave")]
   public float delay;

   public List<GameObject> enemyUnits;
   public EnemySpawnLocation spawnLocation;
   public int enemyCount;
   public float maxRadius;
   public float spawnInterval;
   public float enemySpeed;

   public UnityEvent onWaveBegin;
   public UnityEvent onWaveCleared;
   [Tooltip("Child waves will be spawn after the initial of this wave")]
   public List<Wave> childWaves;

   public List<EnemyMovement> currentEnemies;
   public int generatedCount = 0;
   
}