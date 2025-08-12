using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class TImeText : MonoBehaviour
{
    [SerializeField] Text time_text;
    [SerializeField] Text mmdd_text;
    float refresh_time = 120;
    private DateTime lastServerTime;
    private float cur_time = float.PositiveInfinity;
    void Update()
    {
        cur_time += Time.deltaTime;
        if (cur_time > refresh_time)
        {
            cur_time = 0;
            PlayFabClientAPI.GetTime(new GetTimeRequest(), OnSuccess, OnError);
        }
        else
        {
            lastServerTime = lastServerTime.AddSeconds(Time.deltaTime);
            UpdateUI(lastServerTime);
        }
    }

    void OnSuccess(GetTimeResult result)
    {
        lastServerTime = result.Time.AddHours(9);
        UpdateUI(lastServerTime);
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("시간 받아오는거 에러남 > " + error.GenerateErrorReport());
    }

    void UpdateUI(DateTime time)
    {
        mmdd_text.text = $"{time.Month}.{time.Day}.";
        time_text.text = $"{time.Hour:D2}:{time.Minute:D2}";
    }
}
