using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabEventRewardLevel : BaseTabEventVip {
    public LEventVipPoint lEventVipPoint;
    public List<UIItemEventRewardVipPoint> listItemReward;
    public GameObject txtBigReceive;
    public override void Init()
    {
        base.Init();
        for (int i = 0; i < listItemReward.Count; i++)
        {
            listItemReward[i].Init(i);
        }
    }

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ReceiveLevelVipPoint:
                
                MAccountVipPoint mAccountVipPointResponse = JsonUtility.FromJson<MAccountVipPoint>(data);
                if (mAccountVipPointResponse.ResponseStatus <= -1)
                {
                    ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup("THÔNG BÁO", Helper.GetStringError(mAccountVipPointResponse.ResponseStatus),"Hủy Bỏ");
                    return;
                }
                Database.Instance.SetAccountVipPointInfo(mAccountVipPointResponse);
               
                EffectController.Instance.ShowEffect(8, EffectController.TypeEffectItem.Gold, txtBigReceive.transform.position, () => {
                    Database.Instance.UpdateUserGold(mAccountVipPointResponse.Gold);
                });
                lEventVipPoint.Close();
                Init();
                break;
            
        }
    }
    public override void Show()
    {
        base.Show();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void Hide()
    {
        base.Hide();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }
}
