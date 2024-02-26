using System;
using System.Collections;

using UnityEngine;

public delegate void ClickCallback(Vector2 position, Ray ray);

public static class Utils {
   public static IEnumerator Do(Action action) {
      action();
      yield return 0;
   }

   public static void OnPress(ClickCallback callback) {
      Vector3 position;

#if UNITY_EDITOR
      if (Input.GetMouseButtonDown(0)) {
         position = Input.mousePosition;
#else
      if (Input.touchCount > 0) {
         position = Input.touches[0].position;
#endif
         Ray ray = Camera.main.ScreenPointToRay(position);

         callback(position, ray);
      }
   }
}