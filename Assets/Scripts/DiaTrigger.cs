using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaTrigger : MonoBehaviour
{
    [SerializeField] protected CanvasGroup diaObject;

    protected Tweener tweener;

    private void OnTriggerEnter(Collider other)
    {
        if (tweener != null) { tweener.Kill(); }
        tweener = diaObject.DOFade(1, 0.5f).From(0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (tweener != null) { tweener.Kill(); }
        tweener = diaObject.DOFade(0, 0.5f).From(1);
    }
}
