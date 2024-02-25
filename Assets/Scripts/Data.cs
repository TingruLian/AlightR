using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameScriptableObject", order = 1)]
public class GameScriptableObject : ScriptableObject
{
    public int grassCount;
    public int bookHP;

    public void OnEnable()
    {
        grassCount = 0;
        bookHP = 10;

    }
}


