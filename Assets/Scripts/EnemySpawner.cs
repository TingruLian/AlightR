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

   void Start() {
      spawnTime = Time.time + spawnInterval;
   }

   void Update() {
      if (Time.time > spawnTime) {
         spawnTime = Time.time + spawnInterval;

         Debug.Log("Checking to see if an enemy should be spawned");

         Vector3 targetPos = target.transform.position;
         Vector3 enemyCenter = new Vector3(targetPos.x - 24f, targetPos.y - 6f, targetPos.z - 6f);

         float radius = Random.Range(0f, maxRadius);
         radius = 0;
         float angle = Random.Range(0f, Mathf.PI * 2f);

         Debug.Log($"Selected radius: {radius}");
         Debug.Log($"Selected angle: {angle}");

         float x = Mathf.Sin(angle) * radius;
         float z = Mathf.Cos(angle) * radius;

         Vector3 enemyPos = enemyCenter;

         //GameObject enemyInstance = ...;

         enemyPos = enemyCenter;
         GameObject.Instantiate(enemy3, enemyPos, Quaternion.identity, parent.transform);

         Vector3 zAxis = new Vector3(-6f, 0f, -1.9f).normalized;
         Vector3 xAxis = new Vector3(zAxis.z, 0, -zAxis.x).normalized;

         enemyPos = enemyCenter + (-zAxis * 12);
         GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

         enemyPos = enemyCenter + (zAxis * 12);
         GameObject.Instantiate(enemy2, enemyPos, Quaternion.identity, parent.transform);

         enemyPos = enemyCenter + (-xAxis * 3);
         GameObject.Instantiate(enemy, enemyPos, Quaternion.identity, parent.transform);

         enemyPos = enemyCenter + (xAxis * 3);
         GameObject.Instantiate(enemy2, enemyPos, Quaternion.identity, parent.transform);

         //enemyInstance.GetComponent<EnemyMovement>().target = new Vector3(targetPos.x, targetPos.y - 4f, targetPos.z - 1f);
         //enemyInstance.GetComponent<EnemyMovement>().speed = enemySpeed;
      }
   }
}
