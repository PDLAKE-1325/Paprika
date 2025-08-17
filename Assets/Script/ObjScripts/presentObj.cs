using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class presentObj : MonoBehaviour
{
    public Text username;
    public Text itemname;
    public string gift_id;
    public string item_id;
    public Image gift_img;
    public Action<string> receive_action;

    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<string> names;

    public void OnButtonClicked()
    {
        receive_action?.Invoke(gift_id);
    }
    public void InitialSet()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            if (item_id == sprites[i].name)
            {
                gift_img.sprite = sprites[i];
                itemname.text = names[i];
            }
        }
    }
}
