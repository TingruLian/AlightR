using UnityEngine;

public class EnemySpawner : MonoBehaviour {

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
      spawnCenter = new Vector3(targetPos.x - 24f, targetPos.y - 5.6f, targetPos.z - 6f);

      forwardAxis = new Vector3(-6f, 0f, -1.9f).normalized;
      sideAxis = new Vector3(forwardAxis.z, 0, -forwardAxis.x).normalized;

      //spawnDirectionIndicators();

      spawnTime = Time.time + spawnInterval;
   }

   void Update() {
      if (Time.time > spawnTime) {
         spawnTime = Time.time + spawnInterval;

         float radius = Random.Range(0f, maxRadius);
         float angle = Random.Range(0f, Mathf.PI * 2f);

         Debug.Log($"Selected radius: {radius}");
         Debug.Log($"Selected angle: {angle}");

         float x = Mathf.Sin(angle) * radius;
         float z = Mathf.Cos(angle) * radius;

         Vector3 enemyCenter = spawnCenter + forwardAxis * 20;
         Vector3 enemyPos = new Vector3(enemyCenter.x + x, enemyCenter.y, enemyCenter.z + z);

         GameObject enemyInstance = GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

         Vector3 targetPos = spawnCenter - forwardAxis * 20 + new Vector3(0f, 4f, 0f); ; // target.transform.position;

         enemyInstance.GetComponent<EnemyMovement>().target = new Vector3(targetPos.x, targetPos.y - 4f, targetPos.z - 1f);
         enemyInstance.GetComponent<EnemyMovement>().speed = enemySpeed;
      }
   }

   // This method spawns different colored enemies to help visualize the axes
   void spawnDirectionIndicators() {
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
