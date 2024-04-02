using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXFlasher : MonoBehaviour
{

    [SerializeField] private GameObject Particlevfx;


    void Start()
    {
        Particlevfx.SetActive(false);

    }

    public void SetVFXTrue()
    {
        Particlevfx.SetActive(true);

    }
}
