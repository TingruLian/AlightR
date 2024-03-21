using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompletionCheck : MonoBehaviour
{
   public GameScriptableObject GameData;
   public int levelId;

   public UnityEvent CompleteInitilization;

   public UnityEvent NotCompleteInitilization;

   private void Awake()
   {
      if(GameData == null) { GameData = Resources.Load<GameScriptableObject>("Data"); }
   }
   private void Start()
   {
      if (GameData == null) { Debug.Log("Completion Check don't have GameData"); return; }
      if (levelId < 0) { Debug.Log("not valid id"); return; }

      if (GameData.completionState[levelId]) { CompleteInitilization.Invoke(); }
      else {  NotCompleteInitilization.Invoke(); }
   }
}
