using UnityEngine;

public class EnemySpawner : MonoBehaviour {

   // This is the target toward which enemies will move, i.e. the bridge. They will also be spawned in a radius around it. 
   [SerializeField]
   private GameObject target;

   [SerializeField]
   private GameObject enemy;

   // This is the max distance from the target that enemies will spawn
   [SerializeField]
   private float maxRadius;

   [SerializeField]
   private float enemySpeed;

   private float spawnTime;

   void Start() {
      spawnTime = Time.time + 2;
   }

   void Update() {
      if (Time.time > spawnTime) {
         spawnTime = Time.time + 2;

         Debug.Log("Checking to see if an enemy should be spawned");

         Vector3 targetPos = target.transform.position;
         Vector3 enemyCenter = new Vector3(targetPos.x, targetPos.y - 4f, targetPos.z + 12f);

         float radius = Random.Range(0f, maxRadius);
         float angle = Random.Range(0f, Mathf.PI * 2f);

         Debug.Log($"Selected radius: {radius}");
         Debug.Log($"Selected angle: {angle}");

         float x = Mathf.Sin(angle) * radius;
         float y = Mathf.Cos(angle) * radius;

         Vector3 enemyPos = new Vector3(enemyCenter.x + x, enemyCenter.y, enemyCenter.z + y);

         GameObject enemyInstance = GameObject.Instantiate(enemy, enemyPos, Quaternion.identity);

         enemyInstance.GetComponent<EnemyMovement>().target = new Vector3(targetPos.x, targetPos.y - 4f, targetPos.z - 1f);
         enemyInstance.GetComponent<EnemyMovement>().speed = enemySpeed;
      }
   }
}
