using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabDoiDiemVip : BaseTabEventVip
{

    [SerializeField]
    private Text txtLevel = null, txtVipPoint = null, txtRateExchange = null, txtPointExchange = null, txtBigReceive = null, txtCapche = null;

    [SerializeField]
    private Image imgCapche = null;

    [SerializeField]
    private InputField inputFieldRateExchange = null,inputCapcha = null;

    [SerializeField]
    private ButtonGray buttonGrayChange = null;

    [SerializeField]
    private LEventVipPoint lEventVipPoint = null;
    [SerializeField]
    private UIItemVip[] arrUIItemLevel = null, arrUIItemRate = null;
    private MCaptchaResponse captchaData;
    private MAccountVipPoint mAccountVipPoint;
    private int numberVipInput;
    public override void Show()
    {
        base.Show();
        inputFieldRateExchange.onValueChanged.AddListener(OnUpdateInputVip);
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        GetCaptcha();
        InitInfo();
    }

    private void OnUpdateInputVip(string content)
    {
        if (content == "")
        {
            inputFieldRateExchange.text = "";
            return;
        }
        numberVipInput = int.Parse(content);
        numberVipInput = Mathf.Min(Database.Instance.AccountVipPoint().Point, numberVipInput);
        inputFieldRateExchange.text = numberVipInput.ToString();
        UpdateBigReceive(numberVipInput);
    }



    private void InitInfo()
    {
        mAccountVipPoint = Database.Instance.AccountVipPoint();
        txtLevel.text = (mAccountVipPoint.LevelVip).ToString();
        txtVipPoint.text = mAccountVipPoint.Point.ToString();
        txtRateExchange.text = "1 Vip = <color=#D39273>" + DatabaseServer.ListVipPointDatabasee[mAccountVipPoint.LevelVip-1].RatioExchange + " Big</color>";
        txtBigReceive.text = "_";
        inputFieldRateExchange.text = "";
        inputCapcha.text = "";
        buttonGrayChange.IsActive = false;


    }

    private void UpdateBigReceive(int vipPointInput)
    {
        txtBigReceive.text = vipPointInput * DatabaseServer.ListVipPointDatabasee[mAccountVipPoint.LevelVip-1].RatioExchange + "";
        buttonGrayChange.IsActive = vipPointInput >= 1 ? true : false;
    }

    public override void Hide()
    {
        base.Hide();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GenCaptcha:
                if (status == WebServiceStatus.Status.OK)
                {
                    captchaData = JsonUtility.FromJson<MCaptchaResponse>(data);
                    StartCoroutine(VKCommon.LoadImageFromBase64(imgCapche, captchaData.Data, 0f));
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Không lấy được Captcha. Hãy thử lại!");
                }
                break;
            case WebServiceCode.Code.ExchangeVipPoint:
                MAccountVipPoint mAccountVipPointResponse = JsonUtility.FromJson<MAccountVipPoint>(data);
                if (mAccountVipPointResponse.ResponseStatus == -1)
                {
                    ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup("THÔNG BÁO", "Capcha không chính xác!", "", "Hủy Bỏ");
                    return;
                }
                Database.Instance.SetAccountVipPointInfo(mAccountVipPointResponse);
                EffectController.Instance.ShowEffect(8,EffectController.TypeEffectItem.Gold,txtBigReceive.transform.position,()=> {
                    Database.Instance.UpdateUserGold(mAccountVipPointResponse.Gold);
                });
                lEventVipPoint.Close();
                break;

        }
    }
    public override void Init()
    {
        base.Init();
        captchaData = new MCaptchaResponse();
        for (int i = 0; i < arrUIItemLevel.Length; i++)
        {
            arrUIItemLevel[i].Init(DatabaseServer.ListVipPointDatabasee[i + 1].LevelVip);
            arrUIItemRate[i].Init(DatabaseServer.ListVipPointDatabasee[i + 1].RatioExchange);
        }
    }

    public void GetCaptcha()
    {
        imgCapche.color = new Color(0f, 0f, 0f, 0f);
        SendRequest.SendGenCaptchaRequest();
    }

    public void OnBtnChangePoint()
    {
        MAccountVipPoint mAccountVipPoint = Database.Instance.AccountVipPoint();
        if (mAccountVipPoint.LevelVip <= 1)
        {
            ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup("TTHÔNG BÁO", "Chưa đủ cấp độ Vip để thao tác!","","Hủy Bỏ");
            return;
        }
        if (numberVipInput <= 0)
        {
            ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup("THÔNG BÁO", "Chưa nhập số lượng Vip cần đổi!", "", "Hủy Bỏ");
            return;
        }
        if (inputCapcha.text == "")
        {
            ((LPopup)UILayerController.Instance.ShowLayer(UILayerKey.LPopupTop)).ShowPopup("THÔNG BÁO", "Chưa nhập Capcha!", "", "Hủy Bỏ");
            return;
        }
        SendRequest.SendExchangeVipPoint(numberVipInput,inputCapcha.text, captchaData.Token);
    }
}
