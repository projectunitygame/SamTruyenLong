using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemVip : MonoBehaviour {

    [SerializeField]
    private Text txtValue = null;

    public void Init(int num)
    {
        txtValue.text = VKCommon.ConvertStringMoney(num);
    }
}
