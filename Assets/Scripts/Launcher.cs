using UnityEngine;

public class Launcher : MonoBehaviour {
   public GameObject _prefab;

   void Update() {
#if UNITY_EDITOR
      if (Input.GetMouseButtonDown(0)) {
#else
      if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
#endif
         //spawn in front of at the camera
         Vector3 pos = Camera.main.transform.position;
         Vector3 forw = Camera.main.transform.forward;
         GameObject thing = Instantiate(_prefab, pos + (forw * 0.1f), Quaternion.identity);

         //if it has physics fire it!
         if (thing.TryGetComponent(out Rigidbody rb)) {
            rb.AddForce(forw * 200.0f);
         }
      }
   }
}
