using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorUnit : MonoBehaviour
{
   [SerializeField] protected List<GameObject> arrowList;
   [SerializeField] protected List<GameObject> iconList;
   [SerializeField] protected GameObject arrowContainer;
   [SerializeField] protected GameObject iconContainer;

   public void Ini(int id)
   {
      arrowList[id].SetActive(true);
      iconList[id].SetActive(true);
   }

   public void SetRotate(Vector3 target)
   {
      arrowContainer.transform.eulerAngles = target;
   }
}
