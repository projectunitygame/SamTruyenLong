using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEventVipPoint : UILayer
{

    public enum TypeTabTypeVipPoint
    {
        TabNone = -1,
        TabDoiThuong = 0,
        TabEventThang = 1,
        TabEventSinhNhat = 2,
        TabHuongDan
    }

    private TypeTabTypeVipPoint typeTabCurrent = TypeTabTypeVipPoint.TabNone;

    [SerializeField]
    private UIButtonEventVip[] arrButtonTabEvent = null;
    [SerializeField]
    private BaseTabEventVip[] arrTabEvent = null;


    public override void OnLayerOpenDone()
    {
        base.OnLayerOpenDone();
    }
    public override void ShowLayer()
    {
        base.ShowLayer();
       
    }

    public void ShowTab(int typeTab)
    {
        if ((int)typeTabCurrent == typeTab) return;
        
        if (typeTabCurrent >= 0)
        {
            arrButtonTabEvent[(int)typeTabCurrent].Hide();
            arrTabEvent[(int)typeTabCurrent].Hide();
        }

        typeTabCurrent = (TypeTabTypeVipPoint)typeTab;
        arrButtonTabEvent[(int)typeTabCurrent].Show();
        arrTabEvent[(int)typeTabCurrent].Show();
    }

    public override void Close()
    {
        for (int i = 0; i < arrButtonTabEvent.Length; i++)
        {
            arrButtonTabEvent[i].Hide();
            arrTabEvent[i].Hide();
            typeTabCurrent = TypeTabTypeVipPoint.TabNone;
        }
        base.Close();
    }
}
