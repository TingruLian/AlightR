using UnityEngine;

public class CartMovement : MonoBehaviour {

   public Vector3 start
   { get; set; }

   public Vector3 end
   { get; set; }

   private float progress = 0f; // 0 - 1, basically a percentage

   void Start() {
   }

   void Update() {
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.tag == "MetalCart") {
               if (progress < 1f) {
                  progress += .1f;

                  Vector3 cartPos = start + ((end - start) * progress);

                  transform.position = cartPos;
               }
            }
         }
      });
   }
}
