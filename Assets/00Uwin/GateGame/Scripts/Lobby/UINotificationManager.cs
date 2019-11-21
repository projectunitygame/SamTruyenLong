using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationManager :MonoBehaviour{

    private const float TIME_TWEEN = 7;
    private const float TIME_TWEEN_CONTENT = 0.3f;
    [SerializeField]
    private Text txtNotification = null;

    [SerializeField]
    private RectTransform contentRect = null;


    private List<string> contentList = new List<string>();
    private RectTransform txtRect;
    private bool isRunning;
    protected void Awake()
    {
        txtRect = txtNotification.GetComponent<RectTransform>();
    }

    public void DeleteAll()
    {
        contentList = new List<string>();
        isRunning = false;
        contentRect.localScale = new Vector2(1,0);
        LeanTween.cancel(contentRect.gameObject);
        LeanTween.cancel(txtRect.gameObject);
    }

    public void AddContent(string content)
    {
        contentList.Add(content);
        StartCoroutine(runContent());
    }

    private IEnumerator runContent()
    {
        if (contentList.Count > 0)
        {
            //con content de chay
            if (isRunning == false)
            {
                //chay lan dau tien => hien thanh mau hien
                isRunning = true;
                LeanTween.cancel(contentRect.gameObject);
                LeanTween.scaleY(contentRect.gameObject,1, TIME_TWEEN_CONTENT).setEaseOutBack();
            }
            if (LeanTween.isTweening(txtRect.gameObject) == false)
            {
                string contentCur = contentList[0];
                contentList.RemoveAt(0);
                txtNotification.text = contentCur;
                yield return null;
                float widthContent = txtNotification.preferredWidth;
                
                Vector2 pos = Vector2.zero;
                float timeTween = (widthContent / (contentRect.sizeDelta.x/12.0f)) + TIME_TWEEN;
                pos.x = contentRect.sizeDelta.x / 2 + widthContent / 2 + 50;
                txtRect.anchoredPosition = pos;
                Debug.Log("TimeRunNotice timeTween:" + timeTween);
                LeanTween.moveLocalX(txtRect.gameObject, -contentRect.sizeDelta.x / 2 - widthContent / 2 - 50,timeTween).setEaseLinear().setOnComplete(()=> {
                    Debug.Log("TimeRunNotice Complete");
                    StartCoroutine(runContent());
                });
                
            }
        }
        else
        {
            //get content de chay
            LeanTween.scaleY(contentRect.gameObject, 0, TIME_TWEEN_CONTENT).setEaseLinear().setOnComplete(() =>
            {
                isRunning = false;
            });
            
        }
    }

    
    
}
