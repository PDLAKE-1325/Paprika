using System;
using UnityEngine;
using UnityEngine.UI;

public class freindObj : MonoBehaviour
{
    public Text username;
    public string id;
    public string item_id;
    public Image follow_img;
    public Text follow_text;
    public Action<string, freindObj> followButton_action;
    public Action<string, freindObj> unfollowButton_action;
    public Action<string, string, int> gift_action;
    bool following;
    public bool is_gift;
    public void FollowState(bool following)
    {
        this.following = following;
        if (following)
        {
            follow_img.color = new Color(255, 255, 255);
            follow_text.text = "following";
            follow_text.color = new Color(0, 0, 0);
        }
        else
        {
            follow_img.color = new Color(0, 0, 0);
            follow_text.text = "follow";
            follow_text.color = new Color(255, 255, 255);
        }
    }
    public void OnButtonClicked()
    {
        if (is_gift)
        {
            gift_action?.Invoke(id, item_id, DataForm.Instance.item_prices[item_id]);
            return;
        }
        if (following) unfollowButton_action?.Invoke(id, this);
        else followButton_action?.Invoke(id, this);
    }
}
