using UnityEngine;

public class VisibilityChecker : MonoBehaviour {

   [SerializeField]
   protected GameObject prefab;

   protected GameObject enemyIndicator;
   protected GameObject uiCanvas;

   GameObject indicator = null;

   void Start() {
      GameManager gm = GameManager.instance;

      enemyIndicator = gm.enemyIndicator;
      uiCanvas = gm.uiCanvas;

      indicator = Instantiate(enemyIndicator, uiCanvas.transform);
      indicator.transform.localScale = new Vector3(.5f, .5f, 1f);
   }

   void Update() {
      Vector2 screenPos = GetRelativeScreenPos();

      // TODO: Do a better job of mapping 3d game units to onscreen pixels
      GetIndicator().transform.position = new Vector2(540, 960) + (screenPos * 125);
   }

   void OnBecameVisible() {
      indicator.SetActive(false);
   }

   void OnBecameInvisible() {
      indicator.SetActive(true);
      indicator.transform.position = new Vector2(540, 960) + (GetRelativeScreenPos() * 125);
   }

   GameObject GetIndicator() {
      if (indicator == null) {
         indicator = Instantiate(enemyIndicator, uiCanvas.transform);
         indicator.transform.localScale = new Vector3(.5f, .5f, 1f);
      }

      return indicator;
   }

   Vector2 GetRelativeScreenPos() {
      Transform cam = GameManager.instance.arCamera.transform;

      Vector3 objPosition = prefab.transform.position - cam.transform.position;

      Vector3 projectedPoint = Vector3.ProjectOnPlane(objPosition, cam.forward);

      Vector3 upVec = Vector3.Project(projectedPoint, cam.up);
      Vector3 rightVec = Vector3.Project(projectedPoint, cam.right);

      float upCoord = upVec.x / cam.up.x;
      float rightCoord = rightVec.x / cam.right.x;

      upCoord = CapNumber(upCoord, -960f / 125f, 960f / 125f);
      rightCoord = CapNumber(rightCoord, -540f / 125f, 540f / 125f);

      return new Vector2(rightCoord, upCoord);
   }

   float CapNumber(float val, float min, float max) {
      if (val < min) {
         return min;
      }
      if (val > max) {
         return max;
      }

      return val;
   }
}
