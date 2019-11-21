using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTabEventVip : MonoBehaviour {

    public bool isInited = false;
    public RectTransform content;

    public virtual void Init()
    {
        isInited = true;
    }

    public virtual void Show()
    {
        if (isInited == false)
        {
            Init();
        }
        content.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        content.gameObject.SetActive(false);
    }
}
