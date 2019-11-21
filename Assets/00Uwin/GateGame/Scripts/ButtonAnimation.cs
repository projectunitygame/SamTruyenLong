using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
{
    private float scaleAdd = 0.05f;
    private float timeScale = 1.5f;
    private RectTransform thisRect;
    private Vector3 originScale;
    private Vector3 targetScale;
    private bool isClicked;
    private void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        originScale = Vector3.one;
        targetScale = originScale + scaleAdd*Vector3.one;
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        isClicked = true;
        LeanTween.scale(gameObject, targetScale, timeScale).setEaseOutElastic();
        
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    private void Release()
    {
        if (isClicked)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, originScale, timeScale).setEaseOutElastic();
            
            isClicked = false;
        }
    }
   
}
