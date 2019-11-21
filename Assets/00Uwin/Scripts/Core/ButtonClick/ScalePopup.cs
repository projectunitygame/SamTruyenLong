using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalePopup : MonoBehaviour {

    [SerializeField]
    private float scaleExtra;
    [SerializeField]
    private RectTransform targetRect = null;
    [SerializeField]
    private Image imgIcon = null;
    [SerializeField]
    private Sprite spritePhongTo, spriteThuNho;
    [SerializeField]
    private RectTransform rectBg = null;
    private Vector2 scaleOrigin;
    [SerializeField]
    private bool isScaling;

	// Use this for initialization
	void Start () {
        scaleOrigin = targetRect.localScale;
        imgIcon.sprite = spriteThuNho;
        if (rectBg)
        {
            rectBg.gameObject.SetActive(false);
        }
    }

    public void OnBtnScale()
    {
        LeanTween.cancel(targetRect.gameObject);
        float timeTween = 1;
        if (isScaling == false)
        {
            LeanTween.scale(targetRect.gameObject, scaleOrigin + Vector2.one* scaleExtra,timeTween).setEaseOutBack();
            imgIcon.sprite = spritePhongTo;
            if (rectBg)
            {
                rectBg.gameObject.SetActive(true);
            }
        }
        else
        {
            LeanTween.scale(targetRect.gameObject, scaleOrigin, timeTween).setEaseOutBack();
            imgIcon.sprite = spriteThuNho;
            if (rectBg)
            {
                rectBg.gameObject.SetActive(false);
            }
        }
        isScaling = !isScaling;
    }
}
