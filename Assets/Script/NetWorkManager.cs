using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetWorkManager : MonoBehaviour
{
    private bool heartbeatRunning = false;

    public void Login(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            Debug.Log("로그인함");
            CallManageSession("login");
            StartHeartbeat();
        }, error =>
        {
            Debug.LogError("로그인 실패함 : " + error.GenerateErrorReport());
        });
    }

    public void Logout()
    {
        CallManageSession("logout");
        StopHeartbeat();
    }

    private void CallManageSession(string action)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ManageSession",
            FunctionParameter = new { action = action },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            var functionResult = result.FunctionResult as IDictionary<string, object>;
            if (functionResult.ContainsKey("error"))
            {
                Debug.LogWarning($"서버 에러남 : {functionResult["error"]}");
                if (action == "login" && functionResult["error"].ToString().Contains("Already logged in"))
                {
                    // 중복 로그인 처리 로직 추가 가능
                    Debug.LogWarning("다른 기기에서 이미 로그인 중임");
                }
            }
            else
            {
                Debug.Log($"서버 응답 : {functionResult["result"]}");
            }
        }, error =>
        {
            Debug.LogError("Cloud Script 호출 실패 : " + error.GenerateErrorReport());
        });
    }

    private void StartHeartbeat()
    {
        if (!heartbeatRunning)
        {
            heartbeatRunning = true;
            StartCoroutine(HeartbeatRoutine());
        }
    }

    private void StopHeartbeat()
    {
        heartbeatRunning = false;
        StopAllCoroutines();
    }

    private IEnumerator HeartbeatRoutine()
    {
        while (heartbeatRunning)
        {
            yield return new WaitForSeconds(60f); // 60초 간격
            CallManageSession("heartbeat");
        }
    }
}
