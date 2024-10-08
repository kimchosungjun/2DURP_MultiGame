using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Image fadeImg;
    [SerializeField] Image movingImg;
    [SerializeField] GameObject movingObj;
    public void Init()
    {
        if (fadeImg.gameObject.activeSelf)
            fadeImg.gameObject.SetActive(false);

        if (movingObj.activeSelf)
            movingObj.SetActive(false);
    }

    #region Fade
    float fadeInTime;
    float fadeOutTime;

    /// <summary>
    /// 점점 밝아짐
    /// </summary>
    public void FadeIn(UnityAction _action = null, float _fadeTime = 1f)
    {
       
        fadeInTime = _fadeTime;
        StartCoroutine(FadeInEffect(_action));
    }

    /// <summary>
    /// 점점 어두워짐
    /// </summary>
    public void FadeOut(UnityAction _action = null, float _fadeTime = 1f)
    {
        fadeOutTime = _fadeTime;
        StartCoroutine(FadeOutEffect(_action));
    }

    #region Fade 코루틴
    IEnumerator FadeInEffect(UnityAction _action = null)
    {
        float _timer = 0f;
        Color _imgColor = fadeImg.color;
        while (_timer < fadeInTime)
        {
            _timer += Time.deltaTime;
            _imgColor.a = Mathf.Lerp(1, 0, _timer / fadeInTime);
            fadeImg.color = _imgColor;
            yield return null;
        }
        _imgColor.a = 0;
        fadeImg.color = _imgColor;
        fadeImg.gameObject.SetActive(false);
        if (_action == null)
            yield break;
        _action();
    }

    IEnumerator FadeOutEffect(UnityAction _action = null)
    {
        float _timer = 0f;
        Color _imgColor = fadeImg.color;
        _imgColor.a = 0;
        fadeImg.color = _imgColor;
        fadeImg.gameObject.SetActive(true);
        while (_timer < fadeOutTime)
        {
            _timer += Time.deltaTime;
            _imgColor.a = Mathf.Lerp(0, 1, _timer / fadeOutTime);
            fadeImg.color = _imgColor;
            yield return null;
        }
        _imgColor.a = 1;
        fadeImg.color = _imgColor;
        if (_action == null)
            yield break;
        _action();
    }
    #endregion

    public void LinkFadeEffect()
    {
        FadeOut(LinkFadeIn);
    }

    public void LinkFadeIn()
    {
        FadeIn();
    }

    #endregion

    #region Loading
    public void ShowLoading(bool _isShow)
    {
        if (_isShow)
        {
            movingObj.SetActive(true);
        }
        else
        {
            movingObj.SetActive(false);
        }
    }
    #endregion
}
