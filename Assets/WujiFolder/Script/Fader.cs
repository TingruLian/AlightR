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
   private void Awake()
   {
      instance = this;
      img = GetComponent<Image>();
   }

   private void OnEnable()
   {
      if (img != null)
      {
         img.DOFade(0, fadeTime).From(1);
      }
   }
}
