using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

[Serializable]
public class SpiritUnit
{
    public Transform art;
    public Tweener tween;
    public Transform owner;
}
public class SpiritControl : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected List<SpiritUnit> _spiritUnits;
    [SerializeField] protected List<SpiritUnit> _pendingUnits;
    [SerializeField] protected List<SpiritUnit> _usedUnits;
    [SerializeField] protected List<Transform> _spiritSlotPos;

    protected int slotMax = 0;
    #region Getter
    
    public List<SpiritUnit> spiritUnits { get { return _spiritUnits; } }
    public List<SpiritUnit> pendingUnits { get { return _pendingUnits; } }
    public List<SpiritUnit> usedUnits { get { return _usedUnits; } }
    public List<Transform> spiritSlotPos { get {  return _spiritSlotPos; } }

    #endregion

    public void Initilization(int spiritCount)
    {
        foreach(SpiritUnit unit in _spiritUnits) unit.art.gameObject.SetActive(false);
        ModifySlots(spiritCount);
    }

    public void RequestSpirit(Transform t)
    {
        SpiritUnit spirit = _pendingUnits.Last();
        _pendingUnits.Remove(spirit);
        usedUnits.Add(spirit);
        spirit.owner = t;

        if (spirit.tween != null) { spirit.tween.Kill(); }
        Vector3 targetPos = Camera.main.WorldToScreenPoint(t.position);
        spirit.tween = spirit.art.DOMove(targetPos, 1)
           .OnComplete(() => { spirit.art.gameObject.SetActive(false); });
    }

    public void FreeSpirit(Transform from)
    {
        SpiritUnit spirit = usedUnits.First();
        usedUnits.Remove(spirit);
        pendingUnits.Add(spirit);

        spirit.owner = null;
        spirit.art.gameObject.SetActive(true);
        spirit.art.position = Camera.main.WorldToScreenPoint(from.position);

        if (spirit.tween != null)
        {
            spirit.tween.Kill();
        }
        spirit.tween = spirit.art.DOMove(spiritSlotPos[pendingUnits.Count - 1].transform.position, 1);
    }

    public void ModifySlots(int slotsCount)
    {
        for (int i = slotMax; i < slotsCount +slotMax; i++)
        {
            _spiritUnits[i].art.gameObject.SetActive(true);
            _pendingUnits.Add(_spiritUnits[i]);
        }

        slotMax += slotsCount;

        Vector3 anchor = Vector3.zero;
        float length = (slotMax - 1) * 120f * canvas.transform.localScale.y;
        Vector3 startPos = anchor - new Vector3(length/2, 0, 0);
        float unitLength = 0;
        if (slotMax !=1) unitLength = length / (float)(slotMax-1);

        for(int i= 0; i < _spiritSlotPos.Count; i++) 
        {
            if(i >= slotMax) _spiritSlotPos[i].gameObject.SetActive(false);
            else
            {
                _spiritSlotPos[i].gameObject.SetActive(true);
                _spiritSlotPos[i].localPosition = startPos + new Vector3( i *unitLength, 0, 0);
            }
        }

        
        for(int i= 0; i < _pendingUnits.Count; i++)
        {
            _pendingUnits[i].art.position = _spiritSlotPos[i].position;
        }
    }

    private void Update()
    {
        if (usedUnits.Count <= 0) { return; }

        for (int i = 0; i < usedUnits.Count; i++)
        {
            SpiritUnit spirit = usedUnits[i];
            if (spirit.tween != null && spirit.tween.active && spirit.owner != null)
            {
                Vector3 targetPos = Camera.main.WorldToScreenPoint(spirit.owner.position);
                float t = spirit.tween.position;
                spirit.tween.ChangeEndValue(targetPos);
                spirit.tween.Goto(t, true);
                Debug.Log("updating position");
            }
        }
    }
}
