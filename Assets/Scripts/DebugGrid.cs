using UnityEngine;

public class DebugGrid : MonoBehaviour {

   [SerializeField]
   private MeshRenderer floorRenderer;

   [SerializeField]
   private MeshCollider meshCollider;

   [SerializeField]
   private Collider gridCollider;

   private Texture2D gridImage;

   // these were used when turret placement was restricted by a timer
   float lastTurretPlaced;
   float turretCooldown = 10;

   void Start() {
      GenerateGrid();

      lastTurretPlaced = 0;
   }

   void GenerateGrid() {
      gridImage = new Texture2D(300, 300, TextureFormat.RGBA32, false);
      int borderSize = 15;

      Color gridColor = Color.black;
      Color borderColor = Color.white;
      for (int x = 0; x < gridImage.width; x++) {
         for (int y = 0; y < gridImage.height; y++) {
            if (x < borderSize || x > gridImage.width - borderSize || y < borderSize || y > gridImage.height - borderSize) {
               gridImage.SetPixel(x, y, new Color(borderColor.r, borderColor.g, borderColor.b, 127));
            } else {
               gridImage.SetPixel(x, y, new Color(gridColor.r, gridColor.g, gridColor.b, 0));
            }
         }

         gridImage.wrapMode = TextureWrapMode.Repeat;
         gridImage.Apply();
      }

      floorRenderer.material.mainTexture = gridImage;
      floorRenderer.material.mainTextureScale = new Vector2(meshCollider.bounds.size.x, meshCollider.bounds.size.z);
      floorRenderer.material.mainTextureOffset = new Vector2(.5f, .5f);
   }

   void Update() {
      Vector2 position;

#if UNITY_EDITOR
      if (Input.GetMouseButtonDown(0)) {
         float curTime = Time.time;

         // Require resources to place turrets instead
         /*
         // Quit if not enough time has elapsed
         if (curTime < (lastTurretPlaced + turretCooldown)) {
            return;
         }

         lastTurretPlaced = curTime;
         */

         position = Input.mousePosition;
#else
      if (Input.touchCount > 0) {
         position = Input.touches[0].position;
#endif

         Ray ray = Camera.main.ScreenPointToRay(position);
         RaycastHit hit;

         if (gridCollider.Raycast(ray, out hit, 100.0f)) {
            Debug.LogWarning($"Placing death ray at {hit.point}");

            Vector3 pos = hit.point + Vector3.up * .6f;

            GameManager.instance.PlaceTurret(pos);
         } else {
            Debug.LogError($"User did not touch the plane");
         }
      }
   }
}
