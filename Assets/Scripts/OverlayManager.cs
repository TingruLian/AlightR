using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour {

   [SerializeField]
   CanvasGroup lightning;

   [SerializeField]
   CanvasGroup flash;

   //[SerializeField]
   float start = 0f;

   //[SerializeField]
   float lightningEnd = .8f;

   bool started = false;

   float elapsedTime = 0;

   float fadeSpeed = 1;

   void Start() {
   }

   void FixedUpdate() {
      if ((!started) && (elapsedTime > start)) {
         Debug.LogError($"Start: {start}");
         Debug.LogError($"elapsed: {elapsedTime}");
         started = true;
         flash.gameObject.SetActive(true);
         lightning.gameObject.SetActive(true);
      }

      if (started && (elapsedTime > (start + lightningEnd))) {
         lightning.gameObject.SetActive(false);
      }

      if (started) {
         flash.alpha = Mathf.Lerp(.4f, 0, (elapsedTime - start) * .8f);
      }

      elapsedTime += Time.deltaTime;
   }
}
