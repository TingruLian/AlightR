using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI tmpText;
    [SerializeField] protected RectTransform m_rectTransform;
    void Start()
    {
        if(tmpText == null) tmpText = GetComponent<TextMeshProUGUI>();
        if(tmpText == null) tmpText = GetComponentInChildren<TextMeshProUGUI>();

        m_rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tmpText == null) return;
        if(m_rectTransform == null) return;

        m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,tmpText.textBounds.size.x);
    }
}
