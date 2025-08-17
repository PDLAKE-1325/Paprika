using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] List<StringIntPair> item_n;
    [SerializeField] float delay_set = 1;
    [SerializeField] GameObject heart_prefab;
    [SerializeField] Transform heart_parent;
    float delay = 0;
    void Update()
    {
        delay = delay <= 0 ? 0 : delay - Time.deltaTime;
    }
    public void OnStroke()
    {
        if (delay > 0) return;
        delay = delay_set;
        foreach (var pair in item_n)
        {
            if (GlobalGameData.Instance.data.designSetting[2] == pair.key)
            {
                FurnitureShopManager.Instance.GetCoin(pair.value);
                GameObject obj = Instantiate(heart_prefab, heart_parent);
                obj.transform.localPosition = new Vector3(-28, 135);
            }
        }
    }
}
