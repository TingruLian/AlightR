using UnityEngine;

public class OffscreenPositionIndicator : MonoBehaviour {

   protected bool visible = false;

   public bool IsVisible() {
      return visible;
   }

   void Start() {
      visible = false;
   }

   void OnBecameVisible() {
      visible = true;
   }

   void OnBecameInvisible() {
      visible = false;
   }
}
