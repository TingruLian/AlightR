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
      Debug.Log("invisibale");
   }

   void OnBecameInvisible() {
      visible = false;
      Debug.Log("visibale");
   }
}
