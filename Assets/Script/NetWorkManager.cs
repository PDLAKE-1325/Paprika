using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetWorkManager : Singleton<NetWorkManager>
{
    private bool heartbeatRunning = false;

    #region Enterance

    public void Register(string email, string password, string nickname)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = nickname,
            DisplayName = nickname,
            RequireBothUsernameAndEmail = false, // 이메일 필수 여부
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            Debug.Log("회원가입 성공!");
        }, error =>
        {
            Debug.LogError("회원가입 실패 : " + error.GenerateErrorReport());
        });
    }

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
        Debug.Log("로그아웃함");
        try
        {
            CallManageSession("logout");
            StopHeartbeat();
        }
        catch
        {
            Debug.Log("[로그아웃] 로그인 되어야 쓸 수 있는 매서드임");
        }
    }

    #endregion

    #region Exit

    void OnApplicationQuit()
    {
        Logout();
        Debug.Log("종료");
    }

    #endregion

    #region HeartBeat

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
                    // 중복 로그인 처리 로직 추가 가능 넣어도되는데 지금 안함
                    Debug.LogWarning("다른 기기에서 이미 로그인 중임");
                }
            }
            else
            {
                Debug.Log($"서버 응답 : {functionResult["result"]}");
                if (action == "login")
                {
                    SceneManager.LoadScene("corecher");
                }
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

    #endregion

    #region Extra

    public void RgButtonPressed()
    {
        // 비번제한 between 6~100자
        // Register("hhdh@gmail.com", "hello123", "micle");
        Register("hhdh2@gmail.com", "hello123", "micle2");
    }
    public void LoginButtonPressed()
    {
        // 비번제한 between 6~100자
        // Login("hhdh@gmail.com", "hello123");
        Login("hhdh2@gmail.com", "hello123");
    }

    // 회원가입 실패 : /Client/RegisterPlayFabUser: Email address not available
    // 회원가입 실패 : /Client/RegisterPlayFabUser: The display name entered is not available.

    #endregion
}
