using UnityEngine;

public class CartMovement : MonoBehaviour {

   public Vector3 start
   { get; set; }

   public Vector3 end
   { get; set; }

   private float progress = 0f; // 0 - 1, basically a percentage
   private bool moving = false;
   private Vector3 targetPos;
   private Vector3 startPos;
   private float startTime;
   private float speed = 10f;

   void Start() {
   }

   void Update() {
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         if (!moving && Physics.Raycast(ray, out hit)) {
            if (hit.transform.tag == "MetalCart") {
               if (progress < 1f) {
                  progress += .1f;

                  //Vector3 cartPos = start + ((end - start) * progress);
                  //transform.position = cartPos;

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
      }
   }
}
