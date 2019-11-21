using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LGameSlot25LineMiniGameController : MonoBehaviour {

    public Text textTime;
    public GameObject allPieces;
    private DateTime timeRemain;
    private Coroutine co;
	public void ReloadMiniGame(List<int> CollectedPieces, DateTime time)
    {
        if (time.Ticks > 0)
        {
            timeRemain = new System.DateTime((time.AddHours(4) - DateTime.Now).Ticks);
            if (co == null)
            {
                co = StartCoroutine(Counter());
            }
        }
        for (int i = 0; i < allPieces.transform.childCount; i++)
        {
            allPieces.transform.GetChild(i).GetComponent<Image>().enabled = false;
        }
        if(CollectedPieces.Count > 0)
        {
            foreach (var item in CollectedPieces)
            {
                allPieces.transform.GetChild(item-1).GetComponent<Image>().enabled = true;
            }
        }
    }
    IEnumerator Counter()
    {
        while (true)
        {
            timeRemain =  timeRemain.AddSeconds(-1);
            textTime.text = timeRemain.ToString("hh:mm:ss");
            yield return new WaitForSeconds(1);
        }
    }
}
