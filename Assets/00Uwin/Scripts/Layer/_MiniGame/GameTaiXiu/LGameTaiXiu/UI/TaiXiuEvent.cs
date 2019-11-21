﻿using UnityEngine;
using UnityEngine.UI;

public class TaiXiuEvent : MonoBehaviour {

    public Text txtMaxWin;
    public Text txtMaxLose;

    public void Show(SRSTaiXiuEvent data)
    {
        gameObject.SetActive(true);
        Debug.Log("Showdasdasdsa:"+data.MaxWin+"-"+data.MaxLose);
        txtMaxWin.text = data.MaxWin.ToString();
        txtMaxLose.text = data.MaxLose.ToString();
    }

	public void Hide()
    {
        gameObject.SetActive(false);
    }
}
