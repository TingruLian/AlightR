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

   void Start() {
      UpdateColor(lastDefeatedTurretId);

      GameObject.Find("CartLever").GetComponent<CartController>().cart = this;
   }

   void Update() {
      if (lastDefeatedTurretId < 3 && MapManager.instance.GameData.completionState[lastDefeatedTurretId]) {
         lastDefeatedTurretId++;

         ProgressManager.instance.lastDefeatedTurret = lastDefeatedTurretId;
         UpdateColor(lastDefeatedTurretId);
      }

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

      // Update Data.cartState
      if (IsBlocked() && lastDefeatedTurretId > 0 && !MapManager.instance.GameData.cartState[lastDefeatedTurretId - 1]) {
         MapManager.instance.GameData.cartState[lastDefeatedTurretId - 1] = true;
      }

      // update checkpoint availabilities by cartState
      // There are much more efficient ways to do this, but it works
      for (int i = 1; i <= 2; i++) {
         GameObject checkpoint = GameObject.Find("HotMetal" + (i+1));

         if (MapManager.instance.GameData.cartState[i - 1] != checkpoint.GetComponent<DoOnClick>().enabled) {
            checkpoint.GetComponent<DoOnClick>().enabled = MapManager.instance.GameData.cartState[i - 1];
         }
      }
   }

   bool IsBlocked() {
      if (lastDefeatedTurretId == 0) {
         return true;
      }

      Vector3 startPos = GetNextBattlePos(0); // position of the first turret
      Vector3 nextBattlePos = GetNextBattlePos(lastDefeatedTurretId);

      float distFromStartToNextBattle = (nextBattlePos - startPos).magnitude;
      float distFromStartToPlayer = (transform.position - startPos).magnitude;

      return distFromStartToPlayer > distFromStartToNextBattle;
   }

   // battleId in the range 1-3. An id of 0 means no battle was defeated
   Vector3 GetNextBattlePos(int battleId) {
      float numBattles = 3;

      return start + ((end - start) * (battleId / (numBattles - 1)));
   }

   public void UpdateColor(int colorIndex) {
      Debug.Log("Updating ball color...");

      Material mat = GetComponent<Renderer>().materials[2];

      mat.SetColor("_BaseColor", ballColors[colorIndex]);
   }

   public void MoveCart() {
      if (IsBlocked() || moving || progress >= 1f) {
         return;
      }

      MapManager.instance.gameObject.GetComponent<AudioSource>().Play();

      progress += .1f;

      targetPos = start + ((end - start) * progress);
      startTime = Time.time;
      startPos = transform.position;
      moving = true;
   }
}
