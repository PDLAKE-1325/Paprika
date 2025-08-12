using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class TImeText : MonoBehaviour
{
    [SerializeField] Text time_text;
    [SerializeField] Text mmdd_text;
    float refresh_time = 3;
    float cur_time;
    void Update()
    {
        // cur_mmdd.text = $"{month}.{day}.";
        // cur_time.text = $"{hour:D2}:{minute:D2}";
        cur_time += Time.deltaTime;
        if (cur_time > refresh_time)
        {
            cur_time = 0;
            PlayFabClientAPI.GetTime(new GetTimeRequest(), OnSuccess, OnError);
        }
    }

    void OnSuccess(GetTimeResult result)
    {
        Debug.Log("UTC 플래이팹 서버 시간 " + result.Time);

        System.DateTime koreaTime = result.Time.ToLocalTime().AddHours(9);
        Debug.Log("한국 시간: " + koreaTime);
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("시간 받아오는거 에러남 > " + error.GenerateErrorReport());
    }
}
