using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour {

   public static OverlayManager instance { get; private set; }

   [SerializeField]
   CanvasGroup lightning;

   [SerializeField]
   CanvasGroup flash;

   float start = 0f;
   float lightningEnd = .4f;

   bool started = false;

   float elapsedTime = 0;

   float fadeSpeed = 1;

   private void Awake() {
      if (instance != null && instance != this) {
         Destroy(this);
      } else {
         instance = this;
      }
   }

   public void ActivateFlash() {
      Debug.LogWarning("TRYING TO ACTIVATE THE FLASH");
      started = true;
      flash.gameObject.SetActive(true);
      lightning.gameObject.SetActive(true);
      start = Time.time;
      elapsedTime = start;
   }

   void FixedUpdate() {
      if (started) {
         flash.alpha = Mathf.Lerp(.4f, 0, (elapsedTime - start) * .8f);
         elapsedTime += Time.deltaTime;

         Debug.LogWarning($"elapsed: {elapsedTime}, start: {start}, end: {lightningEnd}");

         if (elapsedTime > (start + lightningEnd)) {
            Debug.LogWarning("ENDING THE FLASH");
            lightning.gameObject.SetActive(false);
            flash.gameObject.SetActive(false);
            started = false;
         }
      }
   }

   /*
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
   */
}
