using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEventVip : MonoBehaviour {

    [SerializeField]
    private Color colorActive, colorUnActive;

    [SerializeField]
    private Text txtTitle = null;

    [SerializeField]
    private Image imgTitle = null;

    [SerializeField]
    private RectTransform rectLight = null;

    public void Show()
    {
        txtTitle.color = colorActive;
        imgTitle.color = colorActive;
        rectLight.gameObject.SetActive(true);
    }

    public void Hide()
    {
        txtTitle.color = colorUnActive;
        imgTitle.color = colorUnActive;
        rectLight.gameObject.SetActive(false);
    }
}
