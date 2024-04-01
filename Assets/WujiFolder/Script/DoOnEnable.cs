using UnityEngine;
using UnityEngine.Events;

public class DoOnEnable : MonoBehaviour {

   public UnityEvent onEnable;
   public UnityEvent onDisable;
   public UnityEvent onDestroy;

   private void OnEnable() {
      onEnable.Invoke();
   }

   private void OnDisable() {
      onDisable.Invoke();
   }

   private void OnDestroy()
   {
      onDestroy.Invoke();
   }
}
