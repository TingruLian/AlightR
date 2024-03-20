using UnityEngine;

public class VisibilityChecker : MonoBehaviour {

   [SerializeField]
   protected GameObject prefab;

   protected GameObject enemyIndicator;
   protected GameObject uiCanvas;

   void Start() {
      Debug.LogError("STARTED VISIBILITY CHECKER");

      GameManager gm = GameManager.instance;

      enemyIndicator = gm.enemyIndicator;
      uiCanvas = gm.uiCanvas;
   }

   void Update() {

   }

   void OnBecameVisible() {
      Debug.LogError("VISIBLE");

      Transform cam = GameManager.instance.arCamera.transform;

      Debug.Log($"Posiiton: {transform.position}");
      Debug.Log($"Camera normal: {cam.forward}");

      Vector3 projectedPoint = Vector3.ProjectOnPlane(prefab.transform.position, cam.forward);
      Debug.Log($"Projected point of spawned enemy: {projectedPoint}");

      Vector3 upVec = Vector3.Project(projectedPoint, cam.up);
      Vector3 rightVec = Vector3.Project(projectedPoint, cam.right);

      float upCoord = upVec.x / cam.up.x;
      float rightCoord = rightVec.x / cam.right.x;

      Debug.Log($"Up unit: {cam.up}");
      Debug.Log($"Up vec: {upVec}");
      Debug.Log($"Up coord: {upCoord}");

      Debug.Log($"Right unit: {cam.right}");
      Debug.Log($"Right vec: {rightVec}");
      Debug.Log($"Right coord: {rightCoord}");

      rightCoord = -6;
      upCoord = -10;

      GameObject indicator = Instantiate(enemyIndicator, uiCanvas.transform);

      indicator.transform.position = new Vector3(540 + (rightCoord * 50), 960 + (upCoord * 50), 0);
      indicator.transform.localScale = new Vector3(.4f, .4f, 1f);
   }

   void OnBecameInvisible() {
      Debug.LogError("INVISIBLE");
   }
}
