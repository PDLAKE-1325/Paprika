using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class GiftManager : Singleton<GiftManager>
{
    [SerializeField] Transform friendsObj_parent;
    [SerializeField] GameObject friendsObj_prefab;
    [SerializeField] Image itemImage;
    List<FriendInfo> currentFriends = new();

    void OnEnable()
    {
        GetFriendsList();
    }

    #region 친구 목록 조회
    public void GetFriendsList()
    {

        var request = new GetFriendsListRequest();

        PlayFabClientAPI.GetFriendsList(request, OnGetFriendsSuccess, OnGetFriendsFailure);
    }

    private void OnGetFriendsSuccess(GetFriendsListResult result)
    {
        currentFriends = result.Friends ?? new List<FriendInfo>();
        Debug.Log($"[FriendManager] 친구 목록 조회 완료: {currentFriends.Count}명");

        ClearFriendList();

        foreach (var friend in currentFriends)
        {
            AddFriendObj(friend.TitleDisplayName, friend.FriendPlayFabId);
        }
    }

    private void OnGetFriendsFailure(PlayFabError error)
    {
        Debug.LogError($"[FriendManager] 친구 목록 조회 실패: {error.GenerateErrorReport()}");
    }

    void ClearFriendList()
    {
        foreach (Transform item in friendsObj_parent)
        {
            Destroy(item.gameObject);
        }
    }

    #endregion

    void AddFriendObj(string name, string id)
    {
        GameObject obj = Instantiate(friendsObj_prefab, friendsObj_parent);
        freindObj obj_script;
        if (obj.TryGetComponent(out obj_script))
        {
            obj_script.is_gift = true;
            obj_script.username.text = name;
            obj_script.id = id;
            obj_script.item_id = itemImage.sprite.name;
            obj_script.gift_action += SendGift;
        }
    }

    #region 선물하기
    public void SendGift(string targetUserId, string itemId, int itemPrice)
    {
        if (FurnitureShopManager.Instance.currentGold < itemPrice)
        {
            Debug.Log("GC 부족함");
            return;
        }

        var subtractRequest = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GC",
            Amount = itemPrice
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(subtractRequest,
            (result) =>
            {
                CreateGiftData(targetUserId, itemId, itemPrice);
                FurnitureShopManager.Instance.GetUserCurrency();
            },
            (error) =>
            {
                Debug.LogError("GC 부족: " + error.GenerateErrorReport());
            });
    }

    private void CreateGiftData(string targetUserId, string itemId, int itemPrice)
    {
        string giftId = "gift_" + System.DateTime.Now.Ticks;

        var giftData = new Dictionary<string, object>
        {
            ["giftId"] = giftId,
            ["fromUserId"] = PlayFabSettings.staticPlayer.PlayFabId,
            ["fromUserName"] = GlobalGameData.Instance.data.my_displayName, // 현재 플레이어 이름
            ["itemId"] = itemId,
            ["itemName"] = itemId,
            ["itemPrice"] = itemPrice,
            ["timestamp"] = System.DateTime.UtcNow.ToString("o"),
            ["status"] = "pending"
        };

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "AddGiftToReceiver",
            FunctionParameter = new
            {
                targetUserId = targetUserId,
                giftData = giftData
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, (result) =>
        {
            print("[선물하기] 보냄");
        }, (error) =>
        {
            Debug.LogError("CloudScript 에러: " + error.GenerateErrorReport());
        });
        gameObject.SetActive(false);
    }
    #endregion
}
