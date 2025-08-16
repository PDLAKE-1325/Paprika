using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] string fname;
    void Update()
    {
        OnShopVisible();
    }
    void OnShopVisible()
    {
        if (GlobalGameData.Instance.data.screenIndex == 1)
        {
            bool flag = true;
            for (int i = 0; i < FurnitureShopManager.Instance.my_inventory.Count; i++)
            {
                if (FurnitureShopManager.Instance.my_inventory[i] == fname) flag = false;
            }
            button.interactable = flag;
        }
    }
}
