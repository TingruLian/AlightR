using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;



public class Level1Tutorial : MonoBehaviour
{
   [SerializeField] protected TextMeshProUGUI text;
   [SerializeField] protected Tutorial1 t1;
   [SerializeField] protected Tutorial2 t2;
   [SerializeField] protected Tutorial3 t3;

   protected List<TutorialContent> contents;
   protected int id = 0;


   private void Awake()
   {
      contents = new List<TutorialContent>() { t1, t2, t3};
   }

   [ContextMenu("NextOne")]
   protected void NextOne()
   {
      if(id >= contents.Count) { Debug.Log("Reached End of Tutorial"); return; }

      contents[id].Init(this);
      contents[id].AssignTrigger();
      contents[id].onExit.AddListener(NextOne);
      id++;
   }

   private void Update()
   {
      if(contents[id-1].entered) contents[id-1].Update();
   }

   [Serializable]
   public class TutorialContent
   {
      protected Level1Tutorial tut;
      public string mainText;
      public UnityEvent onEnter;
      public UnityEvent onExit;
      public bool entered;
      public virtual void Init(Level1Tutorial t) { tut = t; }
      public virtual void AssignTrigger() { }
      public virtual void Entry() { Debug.Log(this.GetType().ToString() + " entried"); onEnter.Invoke(); FadeText(mainText); entered = true; }
      public virtual void Update() { }
      public virtual void Exit() { onExit.Invoke(); FadeOutText(); }

      public virtual void FadeText(string content)
      {
         tut.text.DOFade(1, 1f).From(0).SetUpdate(true);
         tut.text.text = content;
      }

      public virtual void FadeOutText()
      {
         tut.text.DOFade(0, 1f).From(1)
            .OnComplete(() => { tut.text.text = ""; }).SetUpdate(true);
      }
   }



   [Serializable]
   public class Tutorial1 : TutorialContent
   {
      [SerializeField] protected DoOnEnable trigger;
      [SerializeField] protected float duration = 5f;
      [SerializeField] protected SemanticColorControl colorControl;

      protected UnityAction action;

      public override void AssignTrigger()
      {
         base.AssignTrigger();
         action = () =>
         {
            DOTween.Sequence().AppendInterval(1f)
            .AppendCallback(Entry);
         };
         trigger.onEnable.AddListener(action);
      }

      public override void Entry()
      {
         base.Entry();

         trigger.onEnable.RemoveListener(action);

         colorControl.SetToWaveColor(0);

         DOTween.Sequence().AppendInterval(duration)
            .AppendCallback(() => { Exit(); });
      }

   }



   [Serializable]
   public class Tutorial2 : TutorialContent
   {
      public override void AssignTrigger()
      {
         base.AssignTrigger();
            DOTween.Sequence().AppendInterval(3f)
            .AppendCallback(Entry);

      }

      public override void Entry()
      {
         base.Entry();

         foreach(LocationTurretPlacement t in FindObjectsOfType<LocationTurretPlacement>()) { 
            t.onSpawn.AddListener(Exit);
         }

         Time.timeScale = 0.1f;

      }

      public override void Update()
      {
         base.Update();
      }

      public override void Exit()
      {
         base.Exit();

         foreach (LocationTurretPlacement t in FindObjectsOfType<LocationTurretPlacement>())
         {
            t.onSpawn.RemoveListener(Exit);
         }

         Time.timeScale = 1f;

      }
   }


   [Serializable]
   public class Tutorial3: TutorialContent
   {
      protected List<Wave> waves;
      public override void AssignTrigger()
      {
         base.AssignTrigger();
         waves =  FindObjectOfType<EnemySpawner>().GetOnGoingWaves();

         if(waves.Count <= 0) { Entry(); return; }

         waves[0].AddListenerToWaveEnd(Entry);


      }

      public override void Entry()
      {
         base.Entry();

         if(waves.Count > 0) { waves[0].RemoveListenerToWaveEnd(Entry); }

         foreach (TurretBehavior t in FindObjectsOfType<TurretBehavior>())
         {
            t.onManualRotationEnd.AddListener(Exit);
         }

      }

      public override void Update()
      {
         base.Update();

         foreach (TurretBehavior t in FindObjectsOfType<TurretBehavior>())
         {
            t.onManualRotationEnd.RemoveListener(Exit);
            t.onManualRotationEnd.AddListener(Exit);
         }
      }

      public override void Exit()
      {
         base.Exit();

         foreach (TurretBehavior t in FindObjectsOfType<TurretBehavior>())
         {
            t.onManualRotationEnd.RemoveListener(Exit);
         }

      }
   }

}



