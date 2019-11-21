using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabEventMonthVipPoint : BaseTabEventVip
{
    [SerializeField]
    private UIItemVip[] arrUIItemVip = null;

    [SerializeField]
    private Text txtTitle = null, txtReward = null, txtStatus = null, txtRequire = null;

    public override void Init()
    {
        base.Init();

        for (int i = 0; i < arrUIItemVip.Length; i++)
        {
            arrUIItemVip[i].Init(DatabaseServer.ListVipPointDatabasee[i].RewardMonth);
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
