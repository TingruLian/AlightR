using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
   public static Fader instance;
   public const float fadeTime = 1f;
   public Image img;
   public Image progress;
   public CanvasGroup loadGroup;

   private void Awake()
   {
      instance = this;
      img = GetComponent<Image>();
      loadGroup.gameObject.SetActive(false);
   }

   private void OnEnable()
   {
      if (img != null)
      {
         img.DOFade(0, fadeTime).From(1);
      }
   }
}
