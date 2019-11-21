using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabEventBirthDayVipPoint : BaseTabEventVip
{

    [SerializeField]
    private Dropdown dropDownDay = null, dropDownMonth = null, dropDownYear = null;

    public override void Init()
    {
        base.Init();
        InitListDay();
        InitListMonth();
        InitListYear();
    }

    private void InitListDay()
    {
        List<string> list = new List<string>();
        for (int i = 1; i <= 30; i++)
        {
            list.Add(i.ToString());
        }
        dropDownDay.AddOptions(list);
    }

    private void InitListMonth()
    {
        List<string> list = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            list.Add(i.ToString());
        }
        dropDownMonth.AddOptions(list);
    }

    private void InitListYear()
    {
        List<string> list = new List<string>();
        for (int i = 1970; i <= 2019; i++)
        {
            list.Add(i.ToString());
        }
        dropDownYear.AddOptions(list);
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
