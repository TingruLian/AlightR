using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameScriptableObject", order = 1)]
public class GameScriptableObject : ScriptableObject {

    public int grassCount;
    public int bookHP;
    public bool winFirst, winSecond;
    public void OnEnable() {
        grassCount = 0;
        bookHP = 10;
        winFirst = false;
        winSecond = false;
    }
}


