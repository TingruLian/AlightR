using Niantic.Lightship.AR.Semantics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SemanticQuery : MonoBehaviour {
   public ARCameraManager _cameraMan;
   public ARSemanticSegmentationManager _semanticMan;
   public TMP_Text _text;
   public RawImage _image;
   public RawImage _imageLava;
   public Material _material;
   public Material _materialLava;

   private string _channel = "ground";
   private string _groundChannel = "ground";

    /// <summary>
    /// code for counting foliage
    /// </summary>
    [SerializeField] private int grassCount = 0;
    [SerializeField] private float interval = 1.5f;    // Interval time in seconds
    [SerializeField] private TMP_Text grassText;


    void OnEnable() {
      _cameraMan.frameReceived += OnCameraFrameUpdate;
   }

   private void OnDisable() {
      _cameraMan.frameReceived -= OnCameraFrameUpdate;
   }

   private void OnCameraFrameUpdate(ARCameraFrameEventArgs args) {
      if (_semanticMan.subsystem == null || !_semanticMan.subsystem.running) {
         return;
      }

      //get the semantic texture
      Matrix4x4 mat = Matrix4x4.identity;
      var texture = _semanticMan.GetSemanticChannelTexture(_channel, out mat);

      if (texture) {
         //the texture needs to be aligned to the screen so get the display matrix
         //and use a shader that will rotate/scale things.
         Matrix4x4 cameraMatrix = args.displayMatrix ?? Matrix4x4.identity;
         _image.material = _material;
         _image.material.SetTexture("_SemanticTex", texture);
         _image.material.SetMatrix("_SemanticMat", mat);
      }

      InitGroundLayer(args, _groundChannel);
   }

   private void InitGroundLayer(ARCameraFrameEventArgs args, string channelName) {
      if (_semanticMan.subsystem == null || !_semanticMan.subsystem.running) {
         return;
      }

      //get the semantic texture
      Matrix4x4 mat = Matrix4x4.identity;
      var texture = _semanticMan.GetSemanticChannelTexture(channelName, out mat);

      if (texture) {
         //the texture needs to be aligned to the screen so get the display matrix
         //and use a shader that will rotate/scale things.
         Matrix4x4 cameraMatrix = args.displayMatrix ?? Matrix4x4.identity;
         _imageLava.material = _materialLava;
         _imageLava.material.SetTexture("_SemanticTex", texture);
         _imageLava.material.SetMatrix("_SemanticMat", mat);
      }
   }

   private float _timer = 0.0f;
   private float timerF = 0f;  // Timer to keep track of count foliage elapsed time
    void Update() {
      if (_semanticMan.subsystem == null || !_semanticMan.subsystem.running) {
         return;
      }
      //timerF += Time.deltaTime;
        //Unity Editor vs On Device
        if (Input.GetMouseButtonDown(0) || (Input.touches.Length > 0)) {
         var pos = Input.mousePosition;

         if (pos.x > 0 && pos.x < Screen.width) {
            if (pos.y > 0 && pos.y < Screen.height) {
               _timer += Time.deltaTime;
               if (_timer > 0.05f) {
                  var list = _semanticMan.GetChannelNamesAt((int)pos.x, (int)pos.y);

                  if (list.Count > 0) {
                     _channel = list[0];
                     //_channel = list[0];
                     //_text.text = _channel;
                            for (int i = 0; i< list.Count; i++)
                            {
                                _channel = list[i];
                                _text.text = _channel;
                                if (list[i] == "grass" && grassCount < 10)
                                {
                                    grassCount++;
                                    grassText.text = "Grass Count\n" + grassCount + "/10";
                                    //timerF = 0f;
                                }
                            }
                            
                   } 
                  else {
                     _text.text = "?";
                  }

                  _timer = 0.0f;
               }
            }
         }
      }

    }
}