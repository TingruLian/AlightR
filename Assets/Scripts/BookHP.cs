using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookHP : MonoBehaviour, Health
{
    public void TakeDamage(int amount)
    {
        GameManager.instance.ModifyLives(-amount);
    }
}
