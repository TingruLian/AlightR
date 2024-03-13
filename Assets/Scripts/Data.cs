using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameScriptableObject", order = 1)]
public class GameScriptableObject : ScriptableObject {

    public int grassCount;
    public int bookHP;
    public List<bool> completionState;
    public void OnEnable() {
        grassCount = 0;
        bookHP = 10;
    }
}


