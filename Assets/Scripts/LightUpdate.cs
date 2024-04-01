using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpdate : MonoBehaviour
{
   [SerializeField] protected SemanticColorControl colorControl;
   [SerializeField] protected Light lightComponent;

   private void Awake()
   {
      if(colorControl == null) { colorControl = SemanticColorControl.GetInstance(); }
      if(lightComponent == null) { lightComponent = GetComponent<Light>(); }
   }

   // Update is called once per frame
   void Update()
   {
      lightComponent.color = colorControl.getCurrentLight();
   }
}
