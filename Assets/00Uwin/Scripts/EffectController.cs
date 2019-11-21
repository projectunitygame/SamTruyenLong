using Module.Nst.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectController : SingletonMonoDontDestory<EffectController> {
    public enum TypeEffectItem
    {
        Gold
    }
    [SerializeField]
    private Image[] boxTargetList = null;

    [SerializeField]
    private GameObject[] effectPrefabList = null;

    public void ShowEffect(int number, TypeEffectItem typeEffectItem, Vector3 posWorldSpawn, Action callback)
    {
        for (int i = 0; i < number; i++)
        {
            int index = i;
            RectTransform rectItem = GameObject.Instantiate(effectPrefabList[(int)typeEffectItem], transform).GetComponent<RectTransform>();


            rectItem.localScale = Vector3.one;
            rectItem.position = posWorldSpawn;

            Vector3 posSpawn = new Vector3(UnityEngine.Random.Range(posWorldSpawn.x - 2, posWorldSpawn.x + 2), UnityEngine.Random.Range(posWorldSpawn.y - 2, posWorldSpawn.y + 2), posWorldSpawn.z);
            LeanTween.move(rectItem.gameObject,posSpawn,1).setEaseOutBack().setOnComplete(()=> {
                Vector3 posTarget = boxTargetList[(int)typeEffectItem].rectTransform.position;


                float distance = Vector2.Distance(posSpawn, posTarget);
                float timeDelay = 8.3f / distance;
                LeanTween.move(rectItem.gameObject, posTarget,1).setEaseInBack().setDelay(index*0.07f).setOnComplete(()=> {
                    if (index == 0)
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    }
                    DestroyImmediate(rectItem.gameObject);
                });
                
            });
            
        }
    }
}
