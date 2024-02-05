using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public GameObject[] ObjMasked;
    void Update()
    {
        for (int i = 0; i < ObjMasked.Length; i++)
        {
            ObjMasked[i].GetComponent<MeshRenderer>().material.renderQueue = 3002;
            foreach(MeshRenderer meshRenderer in ObjMasked[i].GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.renderQueue = 3002;
            }
            foreach(SkinnedMeshRenderer skinnedMeshRenderer in ObjMasked[i].GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach(Material m in skinnedMeshRenderer.materials) { m.renderQueue = 3002; }
            }
        }
    }
}

