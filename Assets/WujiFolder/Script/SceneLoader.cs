using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
   public int SceneId;

   public void LoadScene()
   {

      if (Fader.instance != null)
      {
         Fader.instance.img.DOFade(1, Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneId); });
      }
      else
      {
         DOTween.Sequence().AppendInterval(Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneId); });
      }
   }

   public void LoadCurrent()
   {
      if (Fader.instance != null)
      {
         Fader.instance.img.DOFade(1, Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
      }
      else
      {
         DOTween.Sequence().AppendInterval(Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneId); });
      }
   }

}
