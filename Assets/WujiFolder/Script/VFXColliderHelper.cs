using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXColliderHelper : MonoBehaviour
{
   public string targetTag;

   void Start()
   {
      GameObject target = GameObject.FindGameObjectWithTag(targetTag);
      ParticleSystem particleSystem = GetComponent<ParticleSystem>();
      if(particleSystem != null&& target != null)particleSystem.collision.SetPlane(0, target.transform);

   }


}
