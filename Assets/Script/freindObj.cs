using System;
using UnityEngine;
using UnityEngine.UI;

public class freindObj : MonoBehaviour
{
    public Text username;
    public string id;
    public Image follow_img;
    public Text follow_text;
    public Action<string, freindObj> followButton_action;
    public Action<string, freindObj> unfollowButton_action;
    bool following;
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
        if (following) unfollowButton_action?.Invoke(id, this);
        else followButton_action?.Invoke(id, this);
    }
}
