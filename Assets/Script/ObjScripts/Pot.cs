using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    [SerializeField] List<StringIntPair> item_n;
    float elapsedTime;
    void Update()
    {
        SetAddPercent();
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 1)
        {
            elapsedTime = 0;
            RollRandom();
        }
    }
    void SetAddPercent()
    {
        foreach (var pair in item_n)
        {
            if (GlobalGameData.Instance.data.designSetting[4] == pair.key)
            {
                GlobalGameData.Instance.data.luckyPercentageAdd = pair.value;
            }
        }
    }
    bool Check(int n)
    {
        int rand = Random.Range(0, 100);
        return rand < n;
    }
    void RollRandom()
    {
        if (Check(GlobalGameData.Instance.data.luckyCoinPercentage
        + GlobalGameData.Instance.data.luckyPercentageAdd))
        {
            print("랜덤 롤 성공");
            FurnitureShopManager.Instance.GetCoin(100);
        }
        else
        {
            print("랜덤 롤 실패");
        }
    }
}
