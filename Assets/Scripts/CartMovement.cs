using UnityEngine;

public class CartMovement : MonoBehaviour {

   // temp
   Color[] ballColors = {
      Color.green,
      Color.yellow,
      Color.red,
      Color.blue
   };

   public Vector3 start
   { get; set; }

   public Vector3 end
   { get; set; }

   public float progress = 0f; // 0 - 1, basically a percentage
   private bool moving = false;
   private Vector3 targetPos;
   private Vector3 startPos;
   private float startTime;
   private float speed = 10f;

   public int lastDefeatedTurretId = 0;

   // This is a simple way to simulate the player beating different levels
   // Once the ball gets to an unbeaten level, force the player to click it 3 times to "beat" the level
   // this will be replaced by actually tracking which levels have been beaten
   private int battleClickProgress = 0;

   void Start() {
      UpdateColor(lastDefeatedTurretId);
   }

   void Update() {
      Utils.OnPress((Vector2 position, Ray ray) => {
         if (IsBlocked()) {
            battleClickProgress++;

            if (battleClickProgress == 3) {
               lastDefeatedTurretId++;
               battleClickProgress = 0;

               ProgressManager.instance.lastDefeatedTurret = lastDefeatedTurretId;
               UpdateColor(lastDefeatedTurretId);
            }

            return;
         }

         RaycastHit hit;

         if (!moving && Physics.Raycast(ray, out hit)) {
            if (hit.transform.tag == "MetalCart") {
               if (progress < 1f) {
                  progress += .1f;

                  targetPos = start + ((end - start) * progress);
                  startTime = Time.time;
                  startPos = transform.position;
                  moving = true;
               }
            }
         }
      });

      if (moving) {
         float elapsedTime = Time.time - startTime;
         Vector3 dir = targetPos - startPos;
         float dist = speed * elapsedTime;

         if (dir.magnitude < dist) {
            // cart reached its destination
            transform.position = targetPos;
            moving = false;
         } else {
            transform.position = startPos + (dir.normalized * dist);
         }

         ProgressManager.instance.cartPos = transform.position;
         ProgressManager.instance.cartMoved = true;
         ProgressManager.instance.cartProgress = progress;
      }
   }

   bool IsBlocked() {
      if (lastDefeatedTurretId == 0) {
         return true;
      }

      Vector3 startPos = GetNextBattlePos(1); // position of the first turret
      Vector3 nextBattlePos = GetNextBattlePos(lastDefeatedTurretId + 1);

      float distFromStartToNextBattle = (nextBattlePos - startPos).magnitude;
      float distFromStartToPlayer = (transform.position - startPos).magnitude;

      return distFromStartToPlayer > distFromStartToNextBattle;
   }

   // battleId in the range 1-3. An id of 0 means no battle was defeated
   Vector3 GetNextBattlePos(int battleId) {
      float numBattles = 3;
      battleId--;

      return start + ((end - start) * (battleId / (numBattles - 1)));
   }

   public void UpdateColor(int colorIndex) {
      Debug.Log("Updating ball color...");

      Material mat = GetComponent<Renderer>().material;

      mat.SetColor("_Color", ballColors[colorIndex]);
   }
}
