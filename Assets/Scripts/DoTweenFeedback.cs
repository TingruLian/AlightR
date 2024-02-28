using DG.Tweening;
using UnityEngine;

public class DoTweenFeedback : MonoBehaviour {
    public bool rotating = false;
    public float rotationSpeed = 60f;
    public float shakeStrength = 1.0f;
    protected Tween _shakeTween;

    public void Shake()
    {
        Shake(0.5f);
    }

    public void Shake(float t)
    {
        if (_shakeTween == null || !_shakeTween.active)
        {
            _shakeTween = transform.DOShakeScale(t, shakeStrength, 200, 90, false, ShakeRandomnessMode.Harmonic);
        }
    }

    private void Update()
    {
        if(rotating)
        {
            transform.Rotate(new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

}
