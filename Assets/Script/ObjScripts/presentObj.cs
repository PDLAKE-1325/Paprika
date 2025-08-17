using System;
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

    public void OnButtonClicked()
    {
        receive_action?.Invoke(gift_id);
    }
    public void InitialSet()
    {
        itemname.text = item_id;
        // 생성되고 거기서실행시킴이거 이미지바뀌는거 해야함
    }
}
