using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public int SceneId;

    AsyncOperation asyncOperation;
    public void LoadScene()
    {
        if (asyncOperation != null) return;

        if (Fader.instance != null)
        {
            asyncOperation = SceneManager.LoadSceneAsync(SceneId);
            asyncOperation.allowSceneActivation = false;

            Fader.instance.loadGroup.gameObject.SetActive(true);
            Fader.instance.img.DOFade(1, Fader.fadeTime).OnComplete(() => {
                Transition(() => { asyncOperation.allowSceneActivation = true; });
            });
        }
        else
        {
            DOTween.Sequence().AppendInterval(Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneId); });
        }
    }

    public void LoadCurrent()
    {
        if (asyncOperation != null) return;

        if (Fader.instance != null)
        {
            asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            asyncOperation.allowSceneActivation = false;

            Fader.instance.loadGroup.gameObject.SetActive(true);
            Fader.instance.img.DOFade(1, Fader.fadeTime).OnComplete(() => {
                Transition(() => { asyncOperation.allowSceneActivation = true; });
            });
        }
        else
        {
            DOTween.Sequence().AppendInterval(Fader.fadeTime).OnComplete(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
        }
    }

    private void Update()
    {
        if (asyncOperation != null)
        {
            Fader.instance.progress.fillAmount = asyncOperation.progress;
        }
    }

    void Transition(UnityAction action)
    {
        Fader.instance.loadGroup.DOFade(1, 0.5f).From(0);
        DOTween.Sequence().AppendInterval(2.5f).
           Append(Fader.instance.loadGroup.DOFade(0, 0.5f).From(1)).AppendInterval(1f).
           OnComplete(action.Invoke);
    }
}
