using System;
using System.Collections.Generic;

[Serializable]
public class StringIntPair
{
    public string key;
    public int value;
}

public class DataForm : Singleton<DataForm>
{
    public Dictionary<string, int> item_prices { get; private set; }
    Dictionary<string, int> GetItemPrices()
    {
        Dictionary<string, int> dict = new();
        List<StringIntPair> data = GlobalGameData.Instance.data.itemPrices;
        foreach (var pair in data)
        {
            dict[pair.key] = pair.value;
        }
        return dict;
    }
    void Start()
    {
        item_prices = GetItemPrices();
    }
}