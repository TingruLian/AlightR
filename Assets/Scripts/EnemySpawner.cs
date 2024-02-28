using UnityEngine;

public class EnemySpawner : MonoBehaviour {
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


   void Start() {
      Vector3 targetPos = target.transform.position;

      forwardAxis = new Vector3(-6f, 0f, -1.9f).normalized;
      sideAxis = new Vector3(forwardAxis.z, 0, -forwardAxis.x).normalized;

      //SpawnDirectionIndicators();

      spawnTime = Time.time + spawnInterval;
      
   }

   void Update() {
      if (Time.time > spawnTime && numEnemies < MAX_ENEMIES) {
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
         enemyInstance.GetComponent<EnemyMovement>().speed = enemySpeed;

         numEnemies++;
      }

   }

   // This method spawns different colored enemies to help visualize the axes
   void SpawnDirectionIndicators() {
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
}
