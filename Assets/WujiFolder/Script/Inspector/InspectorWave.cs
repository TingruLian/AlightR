using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Wave))]
public class InspectorWave : Editor
{
   public override void OnInspectorGUI()
   {
      // Cast the target object to YourScript
      Wave script = (Wave)target;

      // Add a button to the inspector
      if (GUILayout.Button("Save Data"))
      {
         //JsonSaver.ExportAttributes(script.gameObject);
      }

      // Add a button to the inspector
      if (GUILayout.Button("Load Data"))
      {
         //JsonSaver.ReadAttributes(script.gameObject);
      }

      // Draw the default inspector
      DrawDefaultInspector();
   }
}
