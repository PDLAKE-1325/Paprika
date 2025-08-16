using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FurnitureShopManager : Singleton<FurnitureShopManager>
{
    [SerializeField] Text CurGold_text;

    private const string CurrencyCode = "GC";
    private const string CatalogVersion = "MainCatalog";

    public int currentGold = 0;
    private int unsyncedGold = 0;

    public List<string> my_inventory { get; private set; } = new();

    Design design_script;

    void Start()
    {
        GetUserCurrency();
        StartCoroutine(AutoSyncGold());
    }

    void Update()
    {
        CurGold_text.text = $"{currentGold}";
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetCoin(50);
        }
    }

    // 초기 세팅
    public void InitialFurnitureSet(Design design)
    {
        string[] initialFurnitures = { "basic house day_0",
            "green modern bed_0", "green mordern sofa_0",
            "green mordern table_0", "chestnut trumpet flower_0",
            "cat sitting(1)_0"};

        design_script = design;

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            if (result.Inventory.Count != 0)
            {
                GetUserInventory();
                LoadFurnitureDatas();
                return;
            }
            Debug.Log("초기 가구 세팅중");
            my_inventory = new();
            for (int i = 0; i < 6; i++)
            {
                bool flag = false;
                for (int j = 0; j < result.Inventory.Count; j++)
                {
                    for (int k = 0; k < initialFurnitures.Length; k++)
                    {
                        if (result.Inventory[j].ItemId == initialFurnitures[i])
                        {
                            flag = true;
                        }
                    }
                }
                design.place(initialFurnitures[i]);
                if (flag) continue;
                PurchaseFurniture(initialFurnitures[i], 0);
            }
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    // 돈 조회
    public void GetUserCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            if (result.VirtualCurrency.ContainsKey(CurrencyCode))
            {
                currentGold = result.VirtualCurrency[CurrencyCode];
                Debug.Log($"현재 GC 잔액: {currentGold}");
            }
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    //  가구 구입
    public void PurchaseFurniture(string itemId, int price)
    {
        if (currentGold < price)
        {
            Debug.Log("GC 부족함");
            return;
        }

        Debug.Log($"구매 시도: {itemId}, 남은 GC: {currentGold}");

        var request = new PurchaseItemRequest
        {
            CatalogVersion = CatalogVersion,
            ItemId = itemId,
            VirtualCurrency = CurrencyCode,
            Price = price
        };

        PlayFabClientAPI.PurchaseItem(request, result =>
        {
            Debug.Log($"구매 성공: {itemId}");
            // SubtractGoldFromServer(price);
            GetUserCurrency();
            GetUserInventory();
        },
        error =>
        {
            Debug.LogError($"구매 실패: {error.GenerateErrorReport()}");
        });
    }

    // 서버에서 GC 차감
    private void SubtractGoldFromServer(int amount)
    {
        PlayFabClientAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = CurrencyCode,
            Amount = amount
        },
        result => { Debug.Log($"서버 GC 차감 완료: -{amount}"); },
        error => { Debug.LogError($"서버 GC 차감 실패: {error.GenerateErrorReport()}"); });
    }

    // 인벤 조회
    public void GetUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            Debug.Log("현재 인벤토리:");
            my_inventory = new();
            foreach (var item in result.Inventory)
            {
                Debug.Log($"- {item.ItemId}");
                my_inventory.Add(item.ItemId);
            }
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    // 골드 획득
    public void GetCoin(int amount)
    {
        print($"코인 획득 {amount}");
        currentGold += amount;
        unsyncedGold += amount;
    }

    // 2.5f초마다 서버 싱크
    private IEnumerator AutoSyncGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            SyncGoldToServer();
        }
    }

    // 서버에 돈 반영
    private void SyncGoldToServer()
    {
        if (unsyncedGold > 0)
        {
            int amount = unsyncedGold;
            unsyncedGold = 0;

            PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = CurrencyCode,
                Amount = amount
            },
            result => Debug.Log($"서버 싱크 완료: +{amount} GC"),
            error =>
            {
                Debug.LogError($"GC 싱크 실패: {error.GenerateErrorReport()}");
                unsyncedGold += amount;
            });
        }
    }

    // 가구데이터 저장
    public void SaveFurnitureDatas(Dictionary<string, string> data)
    { // string[] keys = { "home", "bed", "sofa", "table", "pot", "pet" };
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = data
        },
        result => Debug.Log("유저 슬롯 저장 완료"),
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    // 가구데이터 불러오기
    public void LoadFurnitureDatas()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data == null || design_script == null) return;
            string[] keys = { "home", "bed", "sofa", "table", "pot", "pet" };
            for (int i = 0; i < keys.Length; i++)
            {
                if (result.Data != null && result.Data.ContainsKey(keys[i]))
                {
                    design_script.place(result.Data[keys[i]].Value);
                }
            }

        }, error => Debug.LogError(error.GenerateErrorReport()));

    }
}
