using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DebugGrid : MonoBehaviour
{

   [SerializeField]
   private MeshRenderer floorRenderer;

   [SerializeField]
   private MeshCollider meshCollider;

   [SerializeField]
   private MeshFilter meshFilter;

   [SerializeField]
   private Collider gridCollider;

   [SerializeField]
   private GameObject turret;

   private Texture2D gridImage;

   void Start()
   {


      GenerateGrid();
   }

   void GenerateGrid()
   {
      gridImage = new Texture2D(300, 300, TextureFormat.ARGB32, false);
      int borderSize = 15;

      Color gridColor = Color.cyan;
      Color borderColor = Color.black;
      for (int x = 0; x < gridImage.width; x++)
      {
         for (int y = 0; y < gridImage.height; y++)
         {
            if (x < borderSize || x > gridImage.width - borderSize || y < borderSize || y > gridImage.height - borderSize)
            {
               gridImage.SetPixel(x, y, new Color(borderColor.r, borderColor.g, borderColor.b, 50));
            }
            else
            {
               gridImage.SetPixel(x, y, new Color(gridColor.r, gridColor.g, gridColor.b, 50));
            }
         }

         gridImage.wrapMode = TextureWrapMode.Repeat;
         gridImage.Apply();
      }

      floorRenderer.material.mainTexture = gridImage;
      floorRenderer.material.mainTextureScale = new Vector2(meshCollider.bounds.size.x, meshCollider.bounds.size.z);
      floorRenderer.material.mainTextureOffset = new Vector2(.5f, .5f);
   }

   void Update()
   {
      Vector2 position;

#if UNITY_EDITOR
      if (Input.GetMouseButtonDown(0))
      {
         position = Input.mousePosition;
#else
      if (Input.touchCount > 0) {
         position = Input.touches[0].position;
#endif

         Ray ray = Camera.main.ScreenPointToRay(position);
         RaycastHit hit;

         if (gridCollider.Raycast(ray, out hit, 100.0f)) {
            Debug.LogWarning($"Placing death ray at {hit.point}");

            GameObject.Instantiate(turret, hit.point, Quaternion.identity);
         } else {
            Debug.LogError($"User did not touch the plane");
         }
      }
   }
}
