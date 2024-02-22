using System.Collections;
using UnityEngine;

public class DebugGrid : MonoBehaviour {
   public bool canPlaceTower = false;
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
      Utils.OnPress((Vector2 position, Ray ray) => {
         RaycastHit hit;

         GameObject turretPlaceholder = GameManager.instance.GetPlaceholderInstance();

         if (turretPlaceholder != null && turretPlaceholder.transform.GetChild(0).gameObject.GetComponent<SphereCollider>().Raycast(ray, out hit, 100.0f)) {
            GameManager.instance.PlaceTurretConfirm();
         } else if (gridCollider.Raycast(ray, out hit, 100.0f)) {
            Vector3 pos = hit.point + Vector3.up * .6f;

            GameManager.instance.ClearPlaceholderInstance();
            GameManager.instance.PlaceTurretInitial(pos);
         } else {
            GameManager.instance.ClearPlaceholderInstance();
         }
      });
   }

    public void SetCanPlaceTower(bool value) {
        StartCoroutine(Co_SetCanPlaceTower(value));
    }

    IEnumerator Co_SetCanPlaceTower(bool value) {
        yield return new WaitForSeconds(0.25f);
        canPlaceTower = value;
    }
}
