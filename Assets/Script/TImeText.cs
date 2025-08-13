using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class TImeText : MonoBehaviour
{
    [Header("시간 텍스트 컴포넌트")]
    [SerializeField] Text time_text;
    [SerializeField] Text mmdd_text;

    [Header("시간에 따른 색상")]
    [SerializeField] private Image targetImage;

    // 시간대별 색상 정의 (예: 새벽, 아침, 낮, 저녁, 밤)
    [SerializeField] private Color dawnColor = new Color(0.7f, 0.7f, 1f);      // 새벽 (4~6시)
    [SerializeField] private Color morningColor = new Color(1f, 0.9f, 0.7f);   // 아침 (6~12시)
    [SerializeField] private Color dayColor = new Color(1f, 1f, 1f);           // 낮 (12~17시)
    [SerializeField] private Color eveningColor = new Color(1f, 0.6f, 0.4f);   // 저녁 (17~20시)
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.4f);   // 밤 (20~4시)

    float refresh_time = 120;
    private DateTime lastServerTime;
    private float cur_time = float.PositiveInfinity;
    bool loggedIn;

    void Start()
    {
        lastServerTime = DateTime.Now;
        CheckLoginStatus();
    }
    public void CheckLoginStatus()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            GlobalGameData.Instance.data.my_displayName = result.AccountInfo.TitleInfo.DisplayName;
            Debug.Log("로그인 되어있음 : " + GlobalGameData.Instance.data.my_displayName);
            loggedIn = true;
        }, error =>
        {
            Debug.LogError("로그인 상태 아니거나 세션 만료 (신경 안써도됨)");
        });
    }
    void Update()
    {
        cur_time += Time.deltaTime;
        if (cur_time > refresh_time && loggedIn)
        {
            cur_time = 0;
            if (loggedIn)
                PlayFabClientAPI.GetTime(new GetTimeRequest(), OnSuccess, OnError);
            else
                lastServerTime = DateTime.Now;
        }
        else
        {
            lastServerTime = lastServerTime.AddSeconds(Time.deltaTime);
            UpdateColorByTime(lastServerTime);
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

    public void UpdateColorByTime(DateTime currentTime)
    {
        time_text.color = new Color(0.196f, 0.196f, 0.196f);
        mmdd_text.color = new Color(0.196f, 0.196f, 0.196f);

        int hour = currentTime.Hour;

        if (hour >= 4 && hour < 6)
            targetImage.color = dawnColor;
        else if (hour >= 6 && hour < 12)
            targetImage.color = morningColor;
        else if (hour >= 12 && hour < 17)
            targetImage.color = dayColor;
        else if (hour >= 17 && hour < 20)
            targetImage.color = eveningColor;
        else
        {
            targetImage.color = nightColor;
            time_text.color = new Color(0.934f, 0.934f, 0.934f);
            mmdd_text.color = new Color(0.934f, 0.934f, 0.934f);
        }
    }
}
