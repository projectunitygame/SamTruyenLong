using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LGameMiniPoker : UILayer
{
    #region Properties
    [Header("--------------------------------------------------")]
    [Space(40)]
    [Header("CONFIG")]
    public int _GAMEID;
    public string _API;
    public string _URL;
    public string _HUBNAME;

    [Space(10)]
    public AudioClip _SCLICK;
    public AudioClip _SFAIL;
    public AudioClip _SWIN;
    public AudioClip _SJACKPOT;
    public AudioClip _SREEL_SPIN;

    [Header("--------------------------------------------------")]
    [Space(20)]
    public RectTransform bgLine1, bgLine3;
    public Image bg;
    public Sprite spriteBg1Line, spriteBg3Line;
    public Button btn1Line, btn3Line;
    public Sprite sprite1LineActive, sprite1LineUnActive, sprite3LineActive, sprite3LineUnactive;
    public VKTextValueChange vkTxtJackpot;
    public List<LineMiniPoker> lines1;
    public List<LineMiniPoker> lines3;

    [Space(20)]
    public List<VKButton> vkBtBets;
    public Transform gLight;
    public SkeletonGraphic canGat;

    [Space(20)]
    public Image imgBtMoneyType;
    public Text txtMoneyType;
    public Sprite[] sprMoney;
    public string[] strMoney;

    [Space(20)]
    public Image imgSound;
    public Sprite[] sprSound;

    [Space(20)]
    public Toggle toggleAuto;
    public Toggle toggleFast;
    public Button btSpin;

    [Space(20)]
    public GameObject gNoti;
    public GameObject[] gNoti3;
    public GameObject gJackpot;
    public Text txtNoti;
    public Text[] txtNoti3;
    public VKTextValueChange txtJackpot;

    [Space(20)]
    [Header("Event")]
    public UIMiniGameEvent _uiEvent;

    [Space(20)]
    public List<int> roomBetValues;

    private MiniPokerSignalRServer _server;
    private MAccountInfo accountInfo;

    private int moneyType;
    private int roomIdSelected;
    private int roomBetValue;
    private bool isAuto;
    private bool isSpinning;
    private DateTime lastUpdateMoney;

    private AssetBundleSettingItem _assetBundleConfig;
    private SettingSoundItem _soundSetting;

    private bool isAbleSpin = true;

    #endregion

    #region Impliment
    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();

        Init();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        _uiEvent.ClearUI();

        _server.HubCallHideSlot();
        SignalRController.Instance.CloseServer((int)_GAMEID);
    }

    public override void Close()
    {
        base.Close();

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
        //UILayerController.Instance.RemoveLayerGame();
        //AssetbundlesManager.Instance.RemoveAssetBundleByKey(_assetBundleConfig.name);
    }
    #endregion

    #region SignalR Serrver
    private void OnSRSEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.ON_CONNECTED:
                HandleConnected();
                break;
            case SRSConst.ON_ERROR:
                HandleConnectError(datas[0].ToString());
                break;
            case SRSConst.ON_CLOSED:
                HandleConnectClose();
                break;
        }
    }
    #endregion

    #region Hub Game
    private void OnSRSHubEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.JACKPOT_MINIPOKER:
                HandleUpdateJackpot(datas);
                break;
            case SRSConst.RESULT_SPIN_MINIPOKER:
                HandleSpinResult(datas);
                break;
        }
    }
    #endregion

    #region Event Update

    #endregion

    #region Listener

    public void ButtonBetClickListener(int roomId)
    {
        if (roomId == roomIdSelected)
            return;

        if (isSpinning)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);

            NotifyController.Instance.Open("Không đổi được mức cược khi đang quay", NotifyController.TypeNotify.Error);
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        UILayerController.Instance.ShowLoading();
        roomIdSelected = roomId;

        ChangeBet();
    }

    public void ButtonChangeMoneyClickListener()
    {
        if (isSpinning)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);

            NotifyController.Instance.Open("Không đổi được loại tiền khi đang quay", NotifyController.TypeNotify.Error);
            return;
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        if (moneyType == (int)MoneyType.GOLD)
        {
            moneyType = (int)MoneyType.COIN;
        }
        else
        {
            moneyType = (int)MoneyType.GOLD;
        }

        ChangeTypeMoney();

        UILayerController.Instance.ShowLoading();
        _server.HubCallGetJackPot((int)moneyType, roomIdSelected);
    }

    public void ButtonChangeSoundClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        AudioAssistant.Instance.MuteSoundGame(_GAMEID);
        ChangeSound();
    }

    public void ButtonSpinClickListener()
    {
        //btSpin.interactable = false;

        if (isAbleSpin == false)
        {
            NotifyController.Instance.Open("Phải quay xong mới được quay tiếp", NotifyController.TypeNotify.Normal);
            return;
        }

        isAbleSpin = false;

        if (moneyType == (int)MoneyType.GOLD)
        {
            if (accountInfo.Gold >= roomBetValue)
            {
                Database.Instance.UpdateUserGold(new MAccountInfoUpdateGold(accountInfo.Gold - roomBetValue));
            }
            else
            {
                isSpinning = false;
                //btSpin.interactable = true;
                isAbleSpin = true;

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                if (numberLine == 1)
                {
                    SetNoti("Số dư của bạn không đủ!!!");
                }
                else
                {
                    SetNoti("Số dư của bạn không đủ!!!",0);
                    SetNoti("Số dư của bạn không đủ!!!",1);
                    SetNoti("Số dư của bạn không đủ!!!",2);
                }

                if (isAuto)
                {
                    isAuto = false;
                    toggleAuto.isOn = false;
                }

                return;
            }
        }
        else
        {
            if (accountInfo.Coin >= roomBetValue)
            {
                Database.Instance.UpdateUserCoin(new MAccountInfoUpdateCoin(accountInfo.Coin - roomBetValue));
            }
            else
            {
                isSpinning = false;
                //btSpin.interactable = true;
                isAbleSpin = true;

                AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SFAIL);
                if (numberLine == 1)
                {
                    SetNoti("Số dư của bạn không đủ!!!");
                }
                else
                {
                    SetNoti("Số dư của bạn không đủ!!!", 0);
                    SetNoti("Số dư của bạn không đủ!!!", 1);
                    SetNoti("Số dư của bạn không đủ!!!", 2);
                }

                if (isAuto)
                {
                    isAuto = false;
                    toggleAuto.isOn = false;
                }

                return;
            }
        }

        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);

        _server.HubCallSpin(moneyType, roomIdSelected, numberLine);
    }
    private int numberLine = 0;

    public void ShowLine(int numberLineTmp)
    {
        if (numberLine == numberLineTmp) return;
        numberLine = numberLineTmp;
        if (numberLine == 1)
        {
            btn1Line.image.sprite = sprite1LineUnActive;
            btn3Line.image.sprite = sprite3LineActive;

            bg.sprite = spriteBg1Line;
            bg.SetNativeSize();

            bgLine1.gameObject.SetActive(true);
            bgLine3.gameObject.SetActive(false);
        }
        else
        {
            btn1Line.image.sprite = sprite1LineActive;
            btn3Line.image.sprite = sprite3LineUnactive;
            
            bg.sprite = spriteBg3Line;
            bg.SetNativeSize();

            bgLine1.gameObject.SetActive(false);
            bgLine3.gameObject.SetActive(true);
        }
        
    }

    public void ButtonClick1Line()
    {
        ShowLine(1);
    }

    public void ButtonClick3Line()
    {
        ShowLine(3);
    }

    public void ButtonAutoClickListener()
    {
        isAuto = toggleAuto.isOn;

        if (isAuto && !isSpinning)
        {
            ButtonSpinClickListener();
        }
        else
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        }
    }

    public void ButtonFastSpinClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        if (numberLine == 1)
        {
            lines1.ForEach(a => a.anim.speed = toggleFast.isOn ? 3 : 1);
        }
        else
        {
            lines3.ForEach(a => a.anim.speed = toggleFast.isOn ? 3 : 1);
        }
    }

    public void ButtonHistoryClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameMiniPokerStatistic, _assetBundleConfig.name,
            (layer) =>
            {
                ((LGameMiniPokerStatistic)layer).InitHistory(_API, moneyType);
            }
        );
    }

    public void ButtonTutorialClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameMiniPokerPopup, _assetBundleConfig.name);
    }

    public void ButtonTopClickListener()
    {
        AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SCLICK);
        UILayerController.Instance.ShowLayer(UILayerKey.LGameMiniPokerStatistic, _assetBundleConfig.name,
            (layer) =>
            {
                ((LGameMiniPokerStatistic)layer).InitRank(_API, moneyType);
            }
        );
    }

    #endregion

    #region Method
    public void Init()
    {
        //UILayerController.Instance.ShowLoading();
        _assetBundleConfig = AssetbundlesManager.Instance.assetSetting.GetItemByGameId(_GAMEID);
        _soundSetting = AudioAssistant.Instance.GetSettingSound(_GAMEID);

        moneyType = (int)MoneyType.GOLD;
        _server = SignalRController.Instance.CreateServer<MiniPokerSignalRServer>((int)_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.SRSInit(_URL, _HUBNAME);

        this.accountInfo = Database.Instance.Account();
        this.roomIdSelected = 1;
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        ChangeSound();
        ChangeTypeMoney();
        toggleAuto.isOn = false;
        toggleFast.isOn = false;
        ButtonFastSpinClickListener();

        var rnd = new System.Random();
        var randomNumbers1 = Enumerable.Range(1, 49).OrderBy(x => rnd.Next()).Take(15).ToList();

        for (int i = 0; i < lines1.Count; i++)
        {
            lines1[i].LoadCard(randomNumbers1[i]);
        }


        for (int i = 0; i < lines3.Count; i++)
        {
            lines3[i].LoadCard(randomNumbers1[i]);
        }
        ShowLine(1);
    }

    public void ChangeBet()
    {
        vkBtBets.ForEach(a => a.VKInteractable = true);
        vkBtBets[roomIdSelected - 1].VKInteractable = false;

        roomBetValue = roomBetValues[roomIdSelected - 1];

        gLight.SetParent(vkBtBets[roomIdSelected - 1].transform);
        gLight.transform.localPosition = Vector3.zero;
        _server.HubCallGetJackPot(moneyType, roomIdSelected);

        _uiEvent.Init(_GAMEID, roomIdSelected);
    }

    public void ChangeTypeMoney()
    {
        txtMoneyType.text = strMoney[moneyType == (int)MoneyType.GOLD ? 0 : 1];
        imgBtMoneyType.sprite = sprMoney[moneyType == (int)MoneyType.GOLD ? 0 : 1];
    }

    public void ChangeSound()
    {
        imgSound.sprite = sprSound[_soundSetting.isMuteSound ? 0 : 1];
    }

    public void SetNoti(string str,int index =0)
    {
        if (numberLine == 1)
        {
            txtNoti.text = str;
            gNoti.SetActive(true);
        }
        else
        {
            txtNoti3[index].text = str;
            gNoti3[index].SetActive(true);
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        UILayerController.Instance.HideLoading();
        ChangeBet();
    }

    public void HandleConnectError(string msg)
    {
        UILayerController.Instance.HideLoading();
        if (string.IsNullOrEmpty(msg))
        {
            LPopup.OpenPopup("Lỗi", msg);
        }
    }

    public void HandleConnectClose()
    {
        UILayerController.Instance.HideLoading();

        StopAllCoroutines();
    }

    public void HandleUpdateJackpot(object[] data)
    {
        UILayerController.Instance.HideLoading();
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSUpdateJackpot result = JsonUtility.FromJson<SRSUpdateJackpot>(json);

        vkTxtJackpot.UpdateNumber(result.Jackpot);
    }

    public void HandleSpinResult(object[] data)
    {
        string json = LitJson.JsonMapper.ToJson(data[0]);
        SRSResultSpinMiniPoker result = JsonUtility.FromJson<SRSResultSpinMiniPoker>(json);
        Debug.Log("HandleSpinResult:" + result.IndexLine + "=>" + result.Cards.Count);
        lastUpdateMoney = DateTime.Now;
        isSpinning = true;
        if (numberLine == 1)
        {
            StartCoroutine(LineSpinResult(result, lines1));
        }
        else
        {
            StartCoroutine(LineSpinResult(result,lines3.GetRange(5*result.IndexLine,5)));
        }
       
    }

    IEnumerator LineSpinResult(SRSResultSpinMiniPoker result,List<LineMiniPoker> listCard)
    {
        if (toggleFast.isOn == false)
        {
            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SREEL_SPIN);
        }
        canGat.AnimationState.SetAnimation(0, "Can_Gat_MiniPoker", false);
        var listCards = result.GetListCard();
        for (int i = 0; i < listCards.Count; i++)
        {
            yield return new WaitForSeconds(toggleFast.isOn ? 0.07f : 0.2f);
            listCard[i].Init(listCards[i], _GAMEID);
        }

        if (numberLine == 1)
        {
            yield return new WaitUntil(() => lines1.All(a => a.isDone = true));
        }
        else
        {
            yield return new WaitUntil(() => lines3.All(a => a.isDone = true));
        }
        string msg = result.PrizeValue > 0 ? "\n+ " + VKCommon.ConvertStringMoney(result.PrizeValue) : "";

        if (Database.Instance.CanUpdateMoney(result.BetType, lastUpdateMoney))
        {
            Database.Instance.UpdateUserMoney(result.BetType, result.Balance);
        }

        vkTxtJackpot.UpdateNumber(result.Jackpot);

        if (result.Cards[0].CardTypeID == 12)
        {
            txtJackpot.UpdateNumber(result.PrizeValue);
            gJackpot.SetActive(true);

            AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SJACKPOT);

            yield return new WaitForSeconds(3);
            gJackpot.SetActive(false);
        }
        else
        {
            switch (result.Cards[0].CardTypeID)
            {
                case 10:
                case 11:
                    break;
                default:
                    SetNoti(VKCommon.ConvertCardTypeId(result.Cards[0].CardTypeID) + msg,result.IndexLine);
                    AudioAssistant.Instance.PlaySoundGame(_GAMEID, _SWIN);
                    break;
            }
        }

        isSpinning = false;

        if (isAuto)
        {
            yield return new WaitForSeconds(1.5f);
            gNoti.SetActive(false);

            isAbleSpin = true;
            ButtonSpinClickListener();
        }
        else
        {
            //btSpin.interactable = true;
            isAbleSpin = true;
        }
    }

    #endregion
}