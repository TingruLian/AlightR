using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompletionCheck : MonoBehaviour {
   public GameScriptableObject GameData;
   public int levelId;

   public UnityEvent CompleteInitilization;

   public UnityEvent NotCompleteInitilization;

   public UnityEvent AllCompleteInitilization;

   private void Awake() {
      if (GameData == null) {
         GameData = Resources.Load<GameScriptableObject>("Data");
      }
   }

   private void Start() {
      if (GameData == null) {
         return;
      }

      if (levelId < 0) {
         return;
      }

      if (GameData.completionState[levelId]) {
         CompleteInitilization.Invoke();
      } else {
         NotCompleteInitilization.Invoke();
         //Debug.Log(gameObject.name+" Not Complete");
      }

      int numCompleted = 0;
      for (int i=0; i<GameData.completionState.Count; i++) {
         if (GameData.completionState[i]) {
            numCompleted++;
         }
      }

      if (numCompleted == GameData.completionState.Count) {
         Debug.Log("Completed all the levels in the game");
         AllCompleteInitilization.Invoke();
      }
   }
}
