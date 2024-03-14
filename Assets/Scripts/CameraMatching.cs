using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMatching : MonoBehaviour
{
   public Camera matchingTarget;
   public Camera myCam;

   private void Update()
   {
      myCam.projectionMatrix = matchingTarget.projectionMatrix;
      myCam.fieldOfView = matchingTarget.fieldOfView;
   }

}
