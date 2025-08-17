using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;

[Serializable]
public class GiftData
{
    public string giftId;
    public string fromUserId;
    public string fromUserName;
    public string itemId;
    public string itemName;
    public int itemPrice;
    public string timestamp;
    public string status; // "pending", "claimed"
}

[Serializable]
public class ClaimGiftResult
{
    public bool success;
    public int receivedGC;
    public string receivedItem;
    public string message;
}

public class GiftReceiver : MonoBehaviour
{
    private List<GiftData> currentGifts = new List<GiftData>();
    // 이벤트 콜백들
    public Action<ClaimGiftResult> OnGiftClaimedSuccess;
    public Action<string> OnGiftError;

    #region 선물 조회
    void OnEnable()
    {
        LoadGiftBox();
    }
    public void LoadGiftBox()
    {
        Debug.Log("선물함을 불러오는 중...");

        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "GiftBox" }
        };

        PlayFabClientAPI.GetUserData(request, OnGiftBoxLoaded, OnError);
    }

    private void OnGiftBoxLoaded(GetUserDataResult result)
    {
        try
        {
            if (result.Data.ContainsKey("GiftBox") && !string.IsNullOrEmpty(result.Data["GiftBox"].Value))
            {
                string giftBoxJson = result.Data["GiftBox"].Value;
                var allGifts = JsonConvert.DeserializeObject<List<GiftData>>(giftBoxJson);

                // pending 상태인 선물만 필터링
                currentGifts = allGifts.Where(g => g.status == "pending").ToList();

                Debug.Log($"받을 수 있는 선물 {currentGifts.Count}개를 찾았습니다.");

                DisplayGifts(currentGifts);
            }
            else
            {
                Debug.Log("받을 선물이 없습니다.");
                currentGifts.Clear();
                DisplayGifts(currentGifts);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"선물함 데이터 파싱 실패: {e.Message}");
            OnError(new PlayFabError { ErrorMessage = "선물함 데이터를 불러오는데 실패했습니다." });
        }
    }
    #endregion

    #region 선물 수령

    public void ClaimGift(string giftId)
    {
        Debug.Log($"선물을 받는 중... ID: {giftId}");

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ClaimGift",
            FunctionParameter = new { giftId = giftId }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnGiftClaimed, OnError);
    }

    private void OnGiftClaimed(ExecuteCloudScriptResult result)
    {
        try
        {
            if (result.Error != null)
            {
                Debug.LogError($"Cloud Script 실행 오류: {result.Error.Error}");
                OnGiftError?.Invoke($"선물 받기 실패: {result.Error.Message}");
                return;
            }

            // 이 부분을 추가
            Debug.Log($"Raw result: {result.FunctionResult}");

            var resultData = JsonConvert.DeserializeObject<ClaimGiftResult>(result.FunctionResult.ToString());

            // 이 부분도 추가
            Debug.Log($"Parsed success: {resultData?.success}");
            Debug.Log($"Parsed receivedGC: {resultData?.receivedGC}");
            Debug.Log($"Parsed receivedItem: {resultData?.receivedItem}");

            if (resultData.success)
            {
                Debug.Log("선물을 성공적으로 받았습니다!");

                if (resultData.receivedGC > 0)
                {
                    Debug.Log($"이미 보유중인 아이템이므로 GC {resultData.receivedGC}을 받았습니다.");
                    resultData.message = $"이미 보유중인 아이템이므로 GC {resultData.receivedGC}을 받았습니다.";
                }
                else if (!string.IsNullOrEmpty(resultData.receivedItem))
                {
                    Debug.Log($"아이템 '{resultData.receivedItem}'을 받았습니다.");
                    resultData.message = $"아이템 '{resultData.receivedItem}'을 받았습니다.";
                }

                OnGiftClaimedSuccess?.Invoke(resultData);

                // 새로고침
                FurnitureShopManager.Instance.GetUserCurrency();
                FurnitureShopManager.Instance.GetUserInventory();
                LoadGiftBox();
            }
            else
            {
                Debug.LogError("선물 받기에 실패했습니다.");
                OnGiftError?.Invoke("선물을 받는데 실패했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"선물 받기 결과 파싱 실패: {e.Message}");
            OnGiftError?.Invoke("선물 받기 결과를 처리하는데 실패했습니다.");
        }
    }
    #endregion

    [SerializeField] Transform receiveObj_parent;
    [SerializeField] GameObject receiveObj_prefab;

    private void DisplayGifts(List<GiftData> gifts)
    {
        foreach (Transform item in receiveObj_parent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in gifts)
        {
            AddPresentObj(item.fromUserName, item.itemId, item.giftId);
        }
    }

    void AddPresentObj(string from_user, string item_id, string gift_id)
    {
        GameObject obj = Instantiate(receiveObj_prefab, receiveObj_parent);
        presentObj obj_script;
        if (obj.TryGetComponent(out obj_script))
        {
            obj_script.username.text = from_user;
            obj_script.gift_id = gift_id;
            obj_script.item_id = item_id;
            obj_script.receive_action += ClaimGift;
            obj_script.InitialSet();
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError($"PlayFab 오류: {error.GenerateErrorReport()}");
        OnGiftError?.Invoke($"오류가 발생했습니다: {error.ErrorMessage}");
    }

    // 선물 받는부분 하고있었음 지금 칼루드 켜보고 확인해.
    // 코드 이해 좀 하고 이제 선물 띄우고 받기 만들고 테스트해 보내기랑 같이
}