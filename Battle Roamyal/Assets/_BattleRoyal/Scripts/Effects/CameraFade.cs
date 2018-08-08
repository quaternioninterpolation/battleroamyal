// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:15 p.m.
// // --
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

public class CameraFade : MonoBehaviour
{
    public Color fadeOutColor;
    public Image image;

    public Color fadeColor
    {
        get
        {
            return image.color;
        }

        set
        {
            Color newColor = value;
            newColor.a = image.color.a;
            image.color = newColor;
        }
    }

    private void Awake()
    {
        if (image == null)
        {
            //Create UI structure
            ConstructFadeUI();
        }

        image.color = fadeOutColor;
    }

    protected virtual void ConstructFadeUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        GameObject imageGo = new GameObject("FadeImage");
        image = imageGo.AddComponent<Image>();
    }

    public void FadeOut(Action callback)
    {
        BeginFade(true, callback);
    }

    public void FadeIn(Action callback)
    {
        BeginFade(false, callback);
    }

    public void BeginFade(bool fadeOut, Action callback)
    {
        image.DOKill();
        image.DOFade(fadeOut ? 0f : 1f, 0f).Complete();
        image.DOFade(fadeOut ? 1f : 0f, 0.3f)
            .OnComplete(()=>callback?.Invoke());
    }
}
