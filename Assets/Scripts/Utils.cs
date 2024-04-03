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
      if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
         position = Input.touches[0].position;
#endif
         Ray ray = Camera.main.ScreenPointToRay(position);
         RaycastHit hit;
         Physics.Raycast(ray, out hit);
         callback(position, ray);
      }
   }

   public static void OnRelease(ClickCallback callback) {
      Vector3 position;

#if UNITY_EDITOR
      if (Input.GetMouseButtonUp(0)) {
         position = Input.mousePosition;
#else
      if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
         position = Input.touches[0].position;
#endif
         Ray ray = Camera.main.ScreenPointToRay(position);
         callback(position, ray);
      }
   }

   public static void OnHold(ClickCallback callback) {
      Vector3 position;

#if UNITY_EDITOR
      if (Input.GetMouseButton(0)) {
         position = Input.mousePosition;
#else
      if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)) {
         position = Input.touches[0].position;
#endif
         Ray ray = Camera.main.ScreenPointToRay(position);
         callback(position, ray);
      }
   }
}