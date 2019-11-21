﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTransfer : AbsShop
{
    public Text txtQuantityGold;
    public InputField inputFieldNameDisplay;
    public InputField inputFieldNameDisplayAgain;
    public InputField inputFielQuanityGoldTransfer;
    public InputField inputfielReason;
     

    [Space(10)]
    public Text txtQuantityGoldGive;
    public Text txtQuantityFee;
    public Text txtQuantityGoldTrans;

    [Space(10)]
    public InputField inputFielCaptcha;
    public Button btGetCaptcha;
    public Button btTransfer;
    public Button btShowAgency;

    public int fee = 2;
    public long minTransfer = 11000;

    private long quantityTransfer = 0;

    private MCaptchaResponse captchaData;



    #region Implement

    public override void Init(object shop)
    {
        base.Init(shop);

        inputFielQuanityGoldTransfer.onEndEdit.AddListener(delegate { EndInputQuantityGoldTransfer(); });
        btGetCaptcha.onClick.AddListener(ClickBtGetCaptcha);
        btShowAgency.onClick.AddListener(ClickBtShowAgency);
        btTransfer.onClick.AddListener(ClickBtTransfer);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent += OnUpdateGold;

        txtQuantityGold.text = VKCommon.ConvertStringMoney(Database.Instance.Account().Gold);
        txtQuantityFee.text = fee.ToString()+"%";
        txtQuantityGoldGive.text = "0";
        txtQuantityGoldTrans.text = "0";

    }
 

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        Database.Instance.OnUserUpdateGoldEvent -= OnUpdateGold;
    }

    #endregion

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.ReceiveOTP:
                if (Helper.CheckStatucSucess(status))
                {
                    UILayerController.Instance.HideLoading();

                    var opCode = int.Parse(data);
                    if (Helper.CheckResponseSuccess(opCode))
                    {
                        SendGetOTPSuccess();
                    }
                }
                break;
            case WebServiceCode.Code.RequestTransfer:
                {
                    UILayerController.Instance.HideLoading();

                    if (status == WebServiceStatus.Status.OK)
                    {
                        long balance = long.Parse(data);
                        if (balance > 0)
                        {

                            LPopup.OpenPopupTop("Thông báo", "Chuyển Thành công");
                            Database.Instance.UpdateUserGold(balance);
                        }
                        else
                        {
                            var error = Helper.GetStringError((int)balance);
                            LPopup.OpenPopupTop("Thông báo", error);
                        }
                    }
                    else
                    {
                        LPopup.OpenPopupTop("Thông báo", "Lỗi kết nôi! Hãy kiểm tra lại");
                    }
                    break;
                }
        }
    }


    #region Listener

    private void ClickBtGetCaptcha()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        SendRequest.SendGetOTP();
    }

    private void ClickBtTransfer()
    {
        if (string.IsNullOrEmpty(inputFieldNameDisplay.text) || string.IsNullOrEmpty(inputFieldNameDisplayAgain.text))
        {
            NotifyController.Instance.Open("Hãy nhập tên người bạn muốn chuyển", NotifyController.TypeNotify.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputFielQuanityGoldTransfer.text))
        {
            NotifyController.Instance.Open("Hãy nhập số tiền muốn chuyển", NotifyController.TypeNotify.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputfielReason.text))
        {
            NotifyController.Instance.Open("Hãy nhập lý do muốn chuyển", NotifyController.TypeNotify.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputFielCaptcha.text))
        {
            NotifyController.Instance.Open("Hãy nhập mã OTP muốn chuyển", NotifyController.TypeNotify.Error);
            return;
        }

        if (inputFieldNameDisplay.text != inputFieldNameDisplayAgain.text)
        {
            NotifyController.Instance.Open("Hai tên không trùng nhau", NotifyController.TypeNotify.Error);
            return;
        }

        if (long.Parse(inputFielQuanityGoldTransfer.text) < minTransfer)
        {
            NotifyController.Instance.Open("Số tiền chuyển phải lớn hơn" + minTransfer.ToString(), NotifyController.TypeNotify.Error);
            return;
        }

        string strNotice = "Số tiền chuyển" + "<color=\"yellow\">" + VKCommon.ConvertStringMoney(long.Parse(inputFielQuanityGoldTransfer.text)) + "</color>" + " Tới tài khoản " + inputFieldNameDisplay.text;

        LPopup.OpenPopupTop("Thông báo", strNotice, "Chuyển", "Hủy bỏ", (value) => { SendRequestTransfer(value); }, true);
    }

    private void ClickBtShowAgency()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LCustomerCare, DataResourceLobby.instance.listObjLayer[IndexSourceGate.LCustomerCare]);
    }

    private void EndInputQuantityGoldTransfer()
    {
        if (inputFielQuanityGoldTransfer.text == null || inputFielQuanityGoldTransfer.text.Length < 1)
        {
            return;
        }
        quantityTransfer = long.Parse(inputFielQuanityGoldTransfer.text);
        long quantityTransferGoldGive = quantityTransfer - (long)(quantityTransfer * 0.02f);
        txtQuantityGoldGive.text = VKCommon.ConvertStringMoney(quantityTransferGoldGive.ToString());
        //quantityTransfer += (int)(quantityTransfer * 0.02f);
        Debug.Log("quantityTransfer:"+ quantityTransfer);
        txtQuantityGoldTrans.text = VKCommon.ConvertStringMoney(quantityTransfer.ToString());
    }

    #endregion

    #region Methed

    private void SendGetOTPSuccess()
    {
        LPopup.OpenPopupTop("THÔNG BÁO!", "Đã gửi mã OTP về số điện thoại của bạn hãy kiểm tra");

        var goldCurrent = Database.Instance.Account().Gold - 1000;
        Database.Instance.UpdateUserGold(goldCurrent);
    }

    private void OnUpdateGold(MAccountInfoUpdateGold info)
    {
        txtQuantityGold.text = VKCommon.ConvertStringMoney(info.Gold);
    }

    private void SendRequestTransfer(bool isSend)
    {
        if (isSend)
        {
            UILayerController.Instance.ShowLoading();
            SendRequest.SendRequestTransfer(inputFieldNameDisplay.text, quantityTransfer.ToString(), inputfielReason.text, inputFielCaptcha.text);
        }
    }
    #endregion



}
