using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;

// Unity 6 호환을 위한 JSON 데이터 구조체들
[System.Serializable]
public class UserData
{
    public string id;
    public string displayName;

    public UserData() { }

    public UserData(string id, string displayName)
    {
        this.id = id;
        this.displayName = displayName;
    }
}

[System.Serializable]
public class CloudScriptSearchResult
{
    public bool success;
    public string message;
    public List<UserData> results;
    public string searchText;
    public int totalFound;
    public string error;

    public CloudScriptSearchResult()
    {
        results = new List<UserData>();
    }
}

[System.Serializable]
public class CloudScriptUpsertResult
{
    public bool success;
    public string message;
    public int totalUsers;
    public string error;
}

public class FriendManager : MonoBehaviour
{
    // 현재 검색 결과
    private List<UserData> currentSearchResults = new List<UserData>();

    // 현재 친구 목록
    private List<FriendInfo> currentFriends = new List<FriendInfo>();

    #region 서버 사용자 리스트 관리

    // 현재 사용자를 서버 검색 리스트에 등록/업데이트
    // 게임 시작 시 또는 닉네임 변경 시 호출해야함
    public void UpsertSelfToServerList()
    {
        Debug.Log("[FriendManager] 서버 사용자 리스트 등록 시작");

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        },
        profileResult =>
        {
            string displayName = profileResult.PlayerProfile?.DisplayName ?? "UnknownPlayer";

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpsertUserToList",
                FunctionParameter = new { displayName = displayName },
                GeneratePlayStreamEvent = false,
                RevisionSelection = CloudScriptRevisionOption.Latest
            };

            PlayFabClientAPI.ExecuteCloudScript(request, OnUpsertSuccess, OnUpsertFailure);
        },
        profileError =>
        {
            Debug.LogWarning($"[FriendManager] DisplayName 가져오기 실패, 기본값 사용: {profileError.GenerateErrorReport()}");

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpsertUserToList",
                FunctionParameter = new { displayName = "Player" },
                GeneratePlayStreamEvent = false,
                RevisionSelection = CloudScriptRevisionOption.Latest
            };

            PlayFabClientAPI.ExecuteCloudScript(request, OnUpsertSuccess, OnUpsertFailure);
        });
    }

    private void OnUpsertSuccess(ExecuteCloudScriptResult result)
    {
        try
        {
            string jsonResult = result.FunctionResult?.ToString();
            Debug.Log($"[FriendManager] Upsert Raw JSON: {jsonResult}");

            if (string.IsNullOrEmpty(jsonResult))
            {
                Debug.LogWarning("[FriendManager] 서버 응답이 비어있음.");
                return;
            }

            var upsertResult = JsonUtility.FromJson<CloudScriptUpsertResult>(jsonResult);

            if (upsertResult != null && upsertResult.success)
            {
                Debug.Log($"[FriendManager] 서버 등록 성공: {upsertResult.message} (총 사용자: {upsertResult.totalUsers})");
            }
            else
            {
                Debug.LogWarning($"[FriendManager] 서버 등록 실패: {upsertResult?.message ?? "알 수 없는 오류"}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[FriendManager] Upsert 결과 파싱 오류: {e.Message}");
        }
    }

    private void OnUpsertFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 서버 등록 실패: {error.GenerateErrorReport()}");
    }

    #endregion

    #region 사용자 검색

    // 사용자 검색
    public void SearchUsers(InputField search)
    {
        string searchText = search.text;
        ClearFoundList();

        if (string.IsNullOrEmpty(searchText?.Trim()))
        {
            Debug.LogWarning("[FriendManager] 검색어가 비어있음.");
            return;
        }

        Debug.Log($"[FriendManager] 사용자 검색 시작: '{searchText}'");

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SearchUsersByName",
            FunctionParameter = new { searchText = searchText.Trim() },
            GeneratePlayStreamEvent = false,
            RevisionSelection = CloudScriptRevisionOption.Latest
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnSearchSuccess, OnSearchFailure);
    }

    private void OnSearchSuccess(ExecuteCloudScriptResult result)
    {
        try
        {
            string jsonResult = result.FunctionResult?.ToString();
            Debug.Log($"[FriendManager] Search Raw JSON: {jsonResult}");

            if (string.IsNullOrEmpty(jsonResult))
            {
                Debug.LogWarning("[FriendManager] 검색 응답이 비어있습니다.");
                return;
            }

            var searchResult = JsonUtility.FromJson<CloudScriptSearchResult>(jsonResult);

            if (searchResult != null && searchResult.success)
            {
                currentSearchResults = searchResult.results ?? new List<UserData>();
                Debug.Log($"[FriendManager] 검색 완료: '{searchResult.searchText}' → {searchResult.totalFound}명 발견");

                foreach (var user in currentSearchResults)
                {
                    Debug.Log($"[검색결과] {user.displayName} (ID: {user.id})");
                    if (FilterSearch(user.displayName, user.id))
                        AddFoundFriendObj(user.displayName, user.id);
                }
            }
            else
            {
                Debug.LogWarning($"[FriendManager] 검색 실패: {searchResult?.message ?? "알 수 없는 오류"}");
                currentSearchResults.Clear();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[FriendManager] 검색 결과 파싱 오류: {e.Message}");
            currentSearchResults.Clear();
        }
    }

    private bool FilterSearch(string name, string id)
    {
        return GlobalGameData.Instance.data.my_displayName != name;
    }

    private void OnSearchFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 사용자 검색 실패: {error.GenerateErrorReport()}");
        currentSearchResults.Clear();
    }

    #endregion

    #region 친구 추가

    // PlayFabId로 친구 추가해야함
    freindObj cur_follow_obj;
    public void AddFriend(string playFabId, freindObj friendObj)
    {
        if (string.IsNullOrEmpty(playFabId))
        {
            Debug.LogWarning("[FriendManager] PlayFabId가 비어있습니다.");
            return;
        }

        Debug.Log($"[FriendManager] 친구 추가 시작: {playFabId}");

        cur_follow_obj = friendObj;

        var request = new AddFriendRequest
        {
            FriendPlayFabId = playFabId
        };

        PlayFabClientAPI.AddFriend(request, OnAddFriendSuccess, OnAddFriendFailure);
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("[FriendManager] 친구 추가 성공!");

        GetFriendsList();
        if (cur_follow_obj != null) cur_follow_obj.FollowState(true);
    }

    private void OnAddFriendFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 친구 추가 실패: {error.GenerateErrorReport()}");
    }

    #endregion

    #region 친구 삭제

    // 삭제도 플레이팹 아이디로
    freindObj cur_unfollow_obj;
    public void RemoveFriend(string playFabId, freindObj friendObj)
    {
        if (string.IsNullOrEmpty(playFabId))
        {
            Debug.LogWarning("[FriendManager] PlayFabId가 비어있습니다.");
            return;
        }

        Debug.Log($"[FriendManager] 친구 삭제 시작: {playFabId}");

        cur_unfollow_obj = friendObj;

        var request = new RemoveFriendRequest
        {
            FriendPlayFabId = playFabId
        };

        PlayFabClientAPI.RemoveFriend(request, OnRemoveFriendSuccess, OnRemoveFriendFailure);
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("[FriendManager] 친구 삭제 성공");

        GetFriendsList();
        if (cur_unfollow_obj != null) cur_unfollow_obj.FollowState(false);
    }

    private void OnRemoveFriendFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 친구 삭제 실패: {error.GenerateErrorReport()}");
    }

    #endregion

    #region 친구 목록 조회

    // 친구 목록 조회
    public void GetFriendsList()
    {
        Debug.Log("[FriendManager] 친구 목록 조회 시작...");

        var request = new GetFriendsListRequest();

        PlayFabClientAPI.GetFriendsList(request, OnGetFriendsSuccess, OnGetFriendsFailure);
    }

    private void OnGetFriendsSuccess(GetFriendsListResult result)
    {
        currentFriends = result.Friends ?? new List<FriendInfo>();
        Debug.Log($"[FriendManager] 친구 목록 조회 완료: {currentFriends.Count}명");

        ClearFollowingList();

        // 친구 목록 로그 출력
        foreach (var friend in currentFriends)
        {
            Debug.Log($"[친구] {friend.TitleDisplayName} (ID: {friend.FriendPlayFabId})");
            AddFollowingFriendObj(friend.TitleDisplayName, friend.FriendPlayFabId);
        }
    }

    private void OnGetFriendsFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 친구 목록 조회 실패: {error.GenerateErrorReport()}");
        currentFriends.Clear();
    }

    #endregion

    #region 유틸리티 메서드

    // 현재 검색 결과 가져오기
    public List<UserData> GetCurrentSearchResults()
    {
        return new List<UserData>(currentSearchResults);
    }

    // 현재 친구 목록 가져오기
    public List<FriendInfo> GetCurrentFriends()
    {
        return new List<FriendInfo>(currentFriends);
    }

    public bool IsAlreadyFriend(string playFabId)
    {
        return currentFriends.Exists(f => f.FriendPlayFabId == playFabId);
    }

    #endregion

    #region 유니티 매서드

    void Start()
    {
        UpsertSelfToServerList();
        GetFriendsList();
    }

    #endregion

    #region UI 매서드

    [SerializeField] GameObject playerObj_prefab;
    [SerializeField] Transform foundPlayerObj_parent;
    [SerializeField] Transform followingList_parent;

    void AddFoundFriendObj(string name, string id)
    {
        GameObject obj = Instantiate(playerObj_prefab, foundPlayerObj_parent);
        freindObj obj_script;
        if (obj.TryGetComponent(out obj_script))
        {
            obj_script.FollowState(IsAlreadyFriend(id));
            obj_script.username.text = name;
            obj_script.id = id;
            obj_script.followButton_action += AddFriend;
            obj_script.unfollowButton_action += RemoveFriend;
        }
    }
    void AddFollowingFriendObj(string name, string id)
    {
        GameObject obj = Instantiate(playerObj_prefab, followingList_parent);
        freindObj obj_script;
        if (obj.TryGetComponent(out obj_script))
        {
            obj_script.FollowState(true);
            obj_script.username.text = name;
            obj_script.id = id;
            obj_script.followButton_action += AddFriend;
            obj_script.unfollowButton_action += RemoveFriend;
        }
    }
    void ClearFollowingList()
    {
        foreach (Transform item in followingList_parent)
        {
            Destroy(item.gameObject);
        }
    }

    void ClearFoundList()
    {
        foreach (Transform item in foundPlayerObj_parent)
        {
            Destroy(item.gameObject);
        }
    }

    #endregion
}