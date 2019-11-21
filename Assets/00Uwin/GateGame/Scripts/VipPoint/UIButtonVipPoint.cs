using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonVipPoint : MonoBehaviour {

    public void OnBtnClick()
    {
        if (Database.Instance.islogin == false)
        {
            LPopup.OpenPopup("Thông báo!", "Chức năng này cần đăng nhập mới sử dụng được");
            return;
        }


        UILayerController.Instance.ShowLayer(UILayerKey.LHistory); ;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }
}
