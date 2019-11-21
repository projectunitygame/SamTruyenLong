using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGray : MonoBehaviour
{
    [SerializeField]
    private Image[] imgList = null;

    [SerializeField]
    private Text[] txtList = null;

    [SerializeField]
    private Text txtNormal = null, txtGray = null;

    private bool isActive;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            if (isActive)
            {
                for (int i = 0; i < imgList.Length; i++)
                {
                    imgList[i].material = null;
                }

                for (int i = 0; i < txtList.Length; i++)
                {
                    txtList[i].material = null;
                }
                if (txtNormal)
                {
                    txtNormal.gameObject.SetActive(true);
                }
                if (txtGray)
                {
                    txtGray.gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < imgList.Length; i++)
                {
                    imgList[i].material = MaterialController.Instance.GreyNormal;
                }

                for (int i = 0; i < txtList.Length; i++)
                {
                    txtList[i].material = MaterialController.Instance.GreyNormal;
                }
                if (txtGray)
                {
                    txtGray.text = txtNormal.text;
                    txtGray.gameObject.SetActive(true);
                }
                if (txtNormal)
                {
                    txtNormal.gameObject.SetActive(false);
                }
            }
        }
    }
}
