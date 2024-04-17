using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetWorkTest : MonoBehaviour
{
    [SerializeField] protected GameObject cat;

    [ContextMenu("Spawn Network Object")]
    public void SpawnTest()
    {
        var instance = Instantiate(cat);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}
