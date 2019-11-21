using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconFill : MonoBehaviour {

    [SerializeField]
    private Image imgFill;
    [SerializeField]
    private float radius;
    private RectTransform thisRect;
    // Use this for initialization
    private float ratioCur;
	void Awake () {
        thisRect = GetComponent<RectTransform>();
        thisRect.anchoredPosition = new Vector3(0, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (ratioCur != imgFill.fillAmount)
        {
            thisRect.anchoredPosition = new Vector3(radius * imgFill.fillAmount,0,0);
            ratioCur = imgFill.fillAmount;
        }
	}
}
