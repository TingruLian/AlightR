using UnityEngine;

public class OverlayManager : MonoBehaviour {

   [SerializeField]
   float fadeStart;

   [SerializeField]
   float fadeSpeed;

   bool fade = true;

   float elapsedTime = 0;

   void Start() {
   }

   void FixedUpdate() {
      if (elapsedTime > fadeStart) {
         fade = true;
      }

      if (fade) {
         GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, (elapsedTime - fadeStart) * fadeSpeed);
      }

      elapsedTime += Time.deltaTime;
   }
}
