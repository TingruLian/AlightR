using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Mono.Collections.Generic;
using System.Collections;

public class JsonSaver : MonoBehaviour
{
   // Method to export attributes of all components on a GameObject
   public static void ExportAttributes(GameObject prefab)
   {
      // Create a dictionary to hold component data
      var componentData = new Dictionary<string, Dictionary<string, object>>();

      // Create a dictionary to hold references to GameObjects by their instance IDs
      var gameObjectReferences = new Dictionary<int, string>();

      // Iterate over all components attached to the prefab
      foreach (Component component in prefab.GetComponents<Component>())
      {
         // Create a dictionary to hold attribute data for this component
         Dictionary<string, object> attributeData = new Dictionary<string, object>();

         // Get the type of the component
         Type componentType = component.GetType();

         // Get all fields of the component, including protected and private ones
         var fields = componentType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

         // Iterate over all fields in the component
         foreach (var field in fields)
         {
            // Check if the field is marked with the SerializeField attribute
            if (Attribute.IsDefined(field, typeof(SerializeField)))
            {
               if(field.GetValue(component) is UnityEngine.Object) Debug.Log(AssetDatabase.Contains((UnityEngine.Object)field.GetValue(component)));
               if(field.GetValue(component) is ICollection) { Debug.Log("is collection"); }
                  // Check if the field type is GameObject
                  if (field.FieldType == typeof(GameObject))
               {
                  GameObject referencedObject = (GameObject)field.GetValue(component);

                  // Add reference to the referenced GameObject by its instance ID
                  if (referencedObject != null)
                  {
                     int gameObjectID = referencedObject.GetInstanceID();
                     if (!gameObjectReferences.ContainsKey(gameObjectID))
                     {
                        gameObjectReferences[gameObjectID] = AssetDatabase.GetAssetPath(referencedObject);
                     }
                     Debug.Log("find gameobject reference at: " + gameObjectReferences[gameObjectID]);
                     attributeData[field.Name] = gameObjectReferences[gameObjectID];
                  }
                  else
                  {
                     attributeData[field.Name] = null;
                  }
               }
               else
               {
                  // Add the field name and its value to the attribute data dictionary
                  attributeData[field.Name] = field.GetValue(component);
               }
            }
         }

         // Add the component name and its attribute data to the component data dictionary
         componentData[componentType.Name] = attributeData;
      }

      // Serialize the component data to JSON using Newtonsoft.Json with custom converter for GameObject references
      string jsonData = JsonConvert.SerializeObject(componentData, Formatting.Indented, new GameObjectReferenceConverter(gameObjectReferences));

      // Open file save panel
      string savePath = EditorUtility.SaveFilePanel("Save Component Data", "", "component_data.json", "json");

      // Write JSON data to file
      if (!string.IsNullOrEmpty(savePath))
      {
         File.WriteAllText(savePath, jsonData);
      }
   }

   // Custom JsonConverter for handling GameObject references
   public class GameObjectReferenceConverter : JsonConverter
   {
      private Dictionary<int, string> gameObjectReferences;

      public GameObjectReferenceConverter(Dictionary<int, string> gameObjectReferences)
      {
         this.gameObjectReferences = gameObjectReferences;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         GameObject gameObject = (GameObject)value;
         int instanceID = gameObject.GetInstanceID();

         // Serialize the GameObject reference by its instance ID
         writer.WriteValue(instanceID);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         // Deserialize GameObject reference by its instance ID
         int instanceID = Convert.ToInt32(reader.Value);
         GameObject referencedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
         return referencedObject;
      }

      public override bool CanConvert(Type objectType)
      {
         // This converter can handle GameObject references
         return objectType == typeof(GameObject);
      }
   }



   // Method to read JSON data and apply values to the prefab
   public static void ReadAttributes(GameObject prefab)
   {
      // Open file selection dialog to choose JSON file
      string filePath = EditorUtility.OpenFilePanel("Select Component Data File", "", "json");

      // Check if file selection was canceled
      if (string.IsNullOrEmpty(filePath))
      {
         return;
      }

      // Read JSON data from file
      string jsonData = File.ReadAllText(filePath);

      // Deserialize JSON data into dictionary of component data
      var componentData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonData);

      // Iterate over each component data
      foreach (var kvp in componentData)
      {
         // Get component type by name
         Type componentType = Type.GetType(kvp.Key);
         if (componentType == null)
         {
            Debug.LogWarning($"Component type {kvp.Key} not found.");
            continue;
         }

         // Get or add component to the prefab
         Component component = prefab.GetComponent(componentType);
         if (component == null)
         {
            component = prefab.AddComponent(componentType);
         }

         // Get attribute data for this component
         var attributeData = kvp.Value;

         // Apply attribute values to the component
         foreach (var kvpAttr in attributeData)
         {
            // Get field by name
            var field = componentType.GetField(kvpAttr.Key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (field != null)
            {
               // Convert value to correct data type
               object value = kvpAttr.Value;
               if (field.FieldType == typeof(float) && value is double)
               {
                  value = Convert.ToSingle((double)value); // Convert double to float
               }

               // Set field value
               field.SetValue(component, value);
            }
            else
            {
               Debug.LogWarning($"Field {kvpAttr.Key} not found in component {kvp.Key}.");
            }
         }
      }

      Debug.Log("Prefab attributes successfully loaded from file.");
   }

   // Method to get the hierarchy path of a GameObject
   private static string GetGameObjectPath(Transform transform)
   {
      if (transform.parent == null)
      {
         return transform.name;
      }

      return GetGameObjectPath(transform.parent) + "/" + transform.name;
   }
}