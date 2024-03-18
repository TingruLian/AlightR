using UnityEngine;

public class VisibilityChecker : MonoBehaviour {

   void Start() {
      Debug.LogError("STARTED VISIBILITY CHECKER");
   }

   void Update() {

   }

   void OnBecameVisible() {
      Debug.LogError("VISIBLE");
   }

   void OnBecameInvisible() {
      Debug.LogError("INVISIBLE");
   }
}
