using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceCollectionSequence : MonoBehaviour
{
    [SerializeField] protected float startDelay = 2f;
    [SerializeField] protected Transform iniTransform;
    [SerializeField] protected List<Sprite> sprites;
    [SerializeField] protected Image image;
    [SerializeField] protected SceneLoader sceneLoader;

    private void Start()
    {
        if (iniTransform == null) iniTransform = FindObjectOfType<BookHP>().transform;
        if (sceneLoader == null) sceneLoader = GetComponent<SceneLoader>();

        if (image == null) image = GetComponentInChildren<Image>();

        if (image != null) { image.sprite = sprites[GameManager.instance.LevelId]; }

        transform.localScale = Vector3.zero;
    }

    [ContextMenu("Test")]
    public void StartSequence()
    {
        if (iniTransform == null) return;

        transform.position = Camera.main.WorldToScreenPoint(iniTransform.position);
        Vector2 newPos = new Vector2(image.canvas.GetComponent<RectTransform>().sizeDelta.x/2, image.canvas.GetComponent<RectTransform>().sizeDelta.y/2);

        DOTween.Sequence()
            .AppendInterval(startDelay)
            .Append(transform.DOScale(Vector3.one, 1).From(Vector3.zero))
            .Join(transform.DOMove(newPos, 1))
            .Join(transform.DORotate(new Vector3(0, 0, 360 * 2), 1, RotateMode.FastBeyond360))
            .AppendInterval(2f)
            .OnComplete(() => { sceneLoader.LoadScene(); });


    }
}
