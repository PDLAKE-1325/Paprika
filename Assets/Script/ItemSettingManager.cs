using System.Collections.Generic;
using UnityEngine;

public class ItemSettingManager : MonoBehaviour
{
    [SerializeField] List<GameObject> buttons;
    [SerializeField] float CheckActivityTime = 1;
    [SerializeField] float SaveDataTime = 5;
    float elapsedTime = 0;
    float elapsedTime1 = 0;
    void Update()
    {
        elapsedTime += Time.deltaTime;
        elapsedTime1 += Time.deltaTime;
        if (elapsedTime > CheckActivityTime)
        {
            elapsedTime = 0;
            SetActivity();
        }
        if (elapsedTime1 > SaveDataTime)
        {
            elapsedTime1 = 0;
            SaveData();
        }
    }

    void SetActivity()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(false);
            for (int j = 0; j < FurnitureShopManager.Instance.my_inventory.Count; j++)
            {
                if (FurnitureShopManager.Instance.my_inventory[j] == buttons[i].name)
                {
                    buttons[i].SetActive(true);
                }
            }
        }
    }
    string[] keys = { "home", "bed", "sofa", "table", "pot", "pet" };
    void SaveData()
    {
        Dictionary<string, string> dict = new();
        for (int i = 0; i < keys.Length; i++)
        {
            dict.Add(keys[i], GlobalGameData.Instance.data.designSetting[i]);
        }
        FurnitureShopManager.Instance.SaveFurnitureDatas(dict);
    }

}
