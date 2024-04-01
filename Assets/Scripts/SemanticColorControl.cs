using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct ColorGroup
{
   public Color color1;
   public Color color2;
}

[CreateAssetMenu(fileName = "SemColor", menuName = "ScriptableObjects/SemanticColorControl", order = 1)]
public class SemanticColorControl : ScriptableObject
{
   [SerializeField]
   protected float TweenTime = 1f;

   [SerializeField]
   protected Material semanticMat;

   [SerializeField]
   protected ColorGroup startColor;

   [SerializeField]
   protected ColorGroup vicColor;

   [SerializeField]
   protected List<ColorGroup> colorOfWaves;

   [SerializeField]
   protected ColorGroup currentColor;

   [Header("Light Attributes")]

   [SerializeField]
   protected Color startLightColor;

   [SerializeField]
   protected Color vicLightColor;

   [SerializeField]
   protected List<Color> colorLightOfWaves;

   [SerializeField]
   protected Color currentLightColor;

   public void Initialize()
   {
      currentColor = new ColorGroup();
   }

   public void TweenColor(ColorGroup c)
   {
      DOTween.To(() => currentColor.color1, x => currentColor.color1 = x, c.color1, TweenTime);
      DOTween.To(() => currentColor.color2, x => currentColor.color2 = x, c.color2, TweenTime);
   }

   public void TweenLight(Color c)
   {
      DOTween.To(() => currentLightColor, x => currentLightColor = x, c, TweenTime);
   }

   public void SetToWaveColor(int id)
   {
      TweenColor(colorOfWaves[id]);
      TweenLight(colorLightOfWaves[id]);
   }

   public void SetToStartColor()
   {
      TweenColor(startColor);
      TweenLight(startLightColor);
   }

   public void SetToVicColor()
   {
      TweenColor(vicColor);
      TweenLight(vicLightColor);
   }

   public void UpdateLoop()
   {
      semanticMat.SetColor("_Color", currentColor.color1);
      semanticMat.SetColor("_Color2", currentColor.color2);
   }

   public Color getCurrentLight() { return currentLightColor; }

   public static SemanticColorControl GetInstance()
   {
      return Resources.Load<SemanticColorControl>("SemColor");
   }
}
