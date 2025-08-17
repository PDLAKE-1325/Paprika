using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Singleton<Bed>
{
    private const string LastSavedKey = "LastSavedTime";
    [SerializeField] List<StringIntPair> item_n;

    // 게임 끝날 때 저장
    public void SaveEndTime(DateTime now)
    {
        PlayerPrefs.SetString(LastSavedKey, now.ToString("o")); // ISO8601 형식으로 저장
        PlayerPrefs.Save();
    }

    // 게임 시작할 때 지난 분 계산
    public void GetMinutesSinceLastPlay(DateTime now)
    {
        if (!PlayerPrefs.HasKey(LastSavedKey))
        {
            return;
        }

        string savedTimeStr = PlayerPrefs.GetString(LastSavedKey);
        if (DateTime.TryParse(savedTimeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime savedTime))
        {
            TimeSpan diff = now - savedTime;
            int get = Mathf.FloorToInt((float)diff.TotalMinutes);
            StartCoroutine(giveDelay(get));

        }
        else
        {
            Debug.LogWarning("저장된 시간 파싱 실패");
            return;
        }
    }
    IEnumerator giveDelay(int get)
    {
        yield return new WaitForSeconds(2);
        int n = 1;
        foreach (var pair in item_n)
        {
            if (pair.key == GlobalGameData.Instance.data.designSetting[1])
            {
                n = pair.value;
            }
        }
        FurnitureShopManager.Instance.GetCoin(get * n);
        print($"{get}분만에 접속");
    }
}
