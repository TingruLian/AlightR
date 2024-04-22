using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelfRegister : NetworkBehaviour {
   public void Spawn(DraggableUI owner) {
      if (NetworkManager.Singleton.IsServer) {
         GetComponent<NetworkObject>().Spawn();
      } else  {
         StartCoroutine(WaitForCopy(owner));
      }
   }

   IEnumerator WaitForCopy(DraggableUI owner) {
      owner.idUpdateFlag = false;
      owner.SpawnCopy(transform);
      yield return new WaitUntil(() => owner.idUpdateFlag);

      Destroy(gameObject);
   }
    
}
