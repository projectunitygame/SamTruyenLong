using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LVipPoint : UILayer
{
    private static int _GAMEID = 1;
    [Header("VipPoint")]
    [SerializeField]
    private Text[] arrTxtVipPoint = null;


    [SerializeField]
    private RectTransform[] arrRectFill = null;

    [SerializeField]
    private Image[] arrImgFill = null;

    [SerializeField]
    private Sprite[] arrSpriteIconVip = null;

    [SerializeField]
    private Image imgIconVip = null;

    [SerializeField]
    private RectTransform vuongMiengRect = null;
    [SerializeField]
    private Text txtLevel = null, txtExp = null, txtVipPoint = null, txtBirthDay = null, txtVipPointCurrent = null;

    private int vipPointCurrent;
    private List<MVipPointDatabase> vipPointDatabaseList = null;
    private bool isLoadFirst = false;
    private AssetBundleSettingItem _assetBundleConfig;
    public override void StartLayer()
    {
        base.StartLayer();
    }


    public override void ShowLayer()
    {
        _assetBundleConfig = Database.Instance.serverConfig.assetbundle.preload[_GAMEID];
        UILayerController.Instance.ShowLoading();
        
        base.ShowLayer();
        gContentAll.SetActive(false);
        if (vipPointDatabaseList == null)
        {
            SendRequest.GetVipPointDataBase();
        }
        else
        {
            SendRequest.GetShortInfoVipPoint();
        }

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    private void InitDataBase(List<MVipPointDatabase> vipPointDatabaseList)
    {
        DatabaseServer.ListVipPointDatabasee = this.vipPointDatabaseList = vipPointDatabaseList;


        for (int i = 1; i < vipPointDatabaseList.Count; i++)
        {
            arrTxtVipPoint[i].text = vipPointDatabaseList[i].VipPoint.ToString();
        }

    }

    private void InitShortInfoVipPoint(int level, long exp, int vipPoint)
    {
        vipPointCurrent = vipPoint;
        txtLevel.text = (level).ToString();
        txtExp.text = VKCommon.ConvertStringMoney(exp)+"/"+ VKCommon.ConvertStringMoney(1000000)+" Big";
        txtVipPoint.text = VKCommon.ConvertStringMoney(vipPoint);
        txtVipPointCurrent.text = VKCommon.ConvertStringMoney(vipPoint);
        txtBirthDay.text = VKCommon.ConvertStringMoney(1000000) + " Big = 1 Vip";
        imgIconVip.sprite = arrSpriteIconVip[level];
        imgIconVip.SetNativeSize();
        for (int i = 0; i < arrImgFill.Length; i++)
        {
            arrImgFill[i].fillAmount = 0;
        }
        for (int i = 0; i < vipPointDatabaseList.Count; i++)
        {
            if (vipPointDatabaseList[i].VipPoint <= vipPointCurrent)
            {
                arrImgFill[i].fillAmount = 1;
            }
            else
            {
                float ratio = 1;
                if (i == vipPointDatabaseList.Count - 1)
                {
                    arrImgFill[i].fillAmount = 1;
                }
                else if (i == 0)
                {
                    ratio = (float)(vipPointCurrent) / (float)(vipPointDatabaseList[i].VipPoint);
                    arrImgFill[i].fillAmount = ratio;
                }
                else
                {
                    ratio = (float)(vipPointCurrent - vipPointDatabaseList[i - 1].VipPoint) / (float)(vipPointDatabaseList[i].VipPoint - vipPointDatabaseList[i - 1].VipPoint);
                    arrImgFill[i].fillAmount = ratio;
                }
                vuongMiengRect.anchoredPosition = new Vector2(arrRectFill[i].anchoredPosition.x + ratio * arrImgFill[i].rectTransform.sizeDelta.x, vuongMiengRect.anchoredPosition.y);
                return;
            }
        }
    }

    public void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetVipPointDataBase:
                Debug.Log("GetVipPointDataBase:"+data);
                List<MVipPointDatabase> fullDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MVipPointDatabase>>(data);
                InitDataBase(fullDataList);

                SendRequest.GetShortInfoVipPoint();
                UILayerController.Instance.HideLoading();
                break;
            case WebServiceCode.Code.GetShortInfoVipPoint:
                Database.Instance.SetAccountVipPointInfo(Newtonsoft.Json.JsonConvert.DeserializeObject<MAccountVipPoint>(data));
                MAccountVipPoint mAccountVipPoint = Database.Instance.AccountVipPoint();
                InitShortInfoVipPoint(mAccountVipPoint.LevelVip, mAccountVipPoint.Exp, mAccountVipPoint.Point);
                UILayerController.Instance.HideLoading();
                gContentAll.SetActive(true);
                break;
        }
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    public void OnBtnReceiveGift()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LEventVipPoint, _assetBundleConfig.name,(layer)=> {
            UILayerController.Instance.GetLayer(UILayerKey.LEventVipPoint).GetComponent<LEventVipPoint>().ShowTab((int)LEventVipPoint.TypeTabTypeVipPoint.TabDoiThuong);
        });
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        Close();
    }

    public void OnBtnOpenBirthDay()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LEventVipPoint, _assetBundleConfig.name, (layer) => {
            UILayerController.Instance.GetLayer(UILayerKey.LEventVipPoint).GetComponent<LEventVipPoint>().ShowTab((int)LEventVipPoint.TypeTabTypeVipPoint.TabEventSinhNhat);
        });
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        
        Close();

    }

   
}
