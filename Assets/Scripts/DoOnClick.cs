using Niantic.Lightship.AR.NavigationMesh;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class DoOnClick : MonoBehaviour {
   public UnityEvent OnClick;

   private void Update() {
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         foreach (Collider c in transform.GetComponentsInChildren<Collider>()) {
            if (c.Raycast(ray, out hit, math.INFINITY)) {
               OnClick.Invoke();
               return;
            }
         }
      });
   }

}
