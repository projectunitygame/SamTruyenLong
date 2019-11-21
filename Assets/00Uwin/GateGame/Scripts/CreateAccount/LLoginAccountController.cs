using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LLoginAccountController : UILayer
{
    [SerializeField]
    private InputField inputFieldNameLogin = null, inputFielPass = null;

    [SerializeField]
    private GameObject layerRegister,layerForget;
    [SerializeField]
    private Toggle toggleRemember;

    [SerializeField]
    private Text txtNotice = null;
    public override void StartLayer()
    {
        base.StartLayer();
        ReloadLayer();
        ReloadLayoutCreateAcount();

        InitInfo();
    }

    private void InitInfo()
    {
        string pass = Database.Instance.localData.password;
        string account = Database.Instance.localData.username;


        inputFielPass.text = pass;
        inputFieldNameLogin.text = account;
    }

    private void ReloadLayoutCreateAcount()
    {
        // Reset text InputField
        inputFieldNameLogin.text = "";
        inputFielPass.text = "";

        txtNotice.gameObject.SetActive(false);
    }

    public void OnBtnClose()
    {
        UILayerController.Instance.HideLayer(this);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    public void OnBtnLogin()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        string name = inputFieldNameLogin.text;
        string pass = inputFielPass.text;

        if (name == "")
        {
            LPopup.OpenPopup("Thông báo", "Bạn chưa nhập 'Tên đăng nhập'");
            return;
        }

        if (pass == "")
        {
            LPopup.OpenPopup("Thông báo", "Bạn chưa nhập 'Mật khẩu'");
            return;
        }
        UILayerController.Instance.ShowLoading();

        Database.Instance.accountTemp = name;
        Database.Instance.passTemp = pass;
        SendRequest.SendSignInRequest(name, pass);
        OnBtnClose();
    }

    public void OnBtnRegister()
    {
        if (UILayerController.Instance.IsCachedLayer(UILayerKey.LCreateAccountController)) {
            var layerCreateAccount = UILayerController.Instance.ShowLayer(UILayerKey.LCreateAccountController, layerRegister);
            layerCreateAccount.ReloadLayer();
        }
        else {
            UILayerController.Instance.ShowLayer(UILayerKey.LCreateAccountController, layerRegister);
        }
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        OnBtnClose();
    }

    public void OnBtnForget()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        UILayerController.Instance.ShowLayer(UILayerKey.LForgetPass, layerForget);
        OnBtnClose();
    }

    public void OnBtnLoginFb()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    
    public override void ReloadLayer()
    {
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
    }

    public override void HideLayer()
    {
        base.HideLayer();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    private void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
           
        }
    }
}
