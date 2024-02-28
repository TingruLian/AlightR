using DG.Tweening;
using UnityEngine;

public class DoTweenFeedback : MonoBehaviour {
   public float shakeStrength = 1.0f;
   protected Tween _shakeTween;

   public void Shake() {
      Shake(0.5f);
   }

   public void Shake(float t) {
      if (_shakeTween == null || !_shakeTween.active) {
         _shakeTween = transform.DOShakeScale(t, shakeStrength, 200, 90, false, ShakeRandomnessMode.Harmonic);
      }
   }
}
