using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

   public Vector3 target;
   public float speed;

   private float lastUpdateTime;

   void Start() {
      lastUpdateTime = Time.time;
   }

   void Update() {
      float curTime = Time.time;
      float elapsedTime = curTime - lastUpdateTime;
      lastUpdateTime = curTime;

      Vector3 curPos = gameObject.transform.position;

      Vector3 distTraveled = Vector3.Normalize(target - curPos) * speed * elapsedTime;

      //transform.position += distTraveled;
   }
}
