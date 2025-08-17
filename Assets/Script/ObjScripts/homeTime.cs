using System;
using UnityEngine;
using UnityEngine.UI;

public class homeTime : MonoBehaviour
{
    Image img;
    [SerializeField] Sprite[] sprites;
    [SerializeField] AudioClip dayBGM;
    [SerializeField] AudioClip nightBGM;
    void Start()
    {
        img = GetComponent<Image>();
    }
    void Update()
    {
        DateTime now = TImeText.Instance.lastServerTime;
        int hour = now.Hour;

        if (hour >= 7 && hour < 22)
        {
            SoundManager.Instance.PlayBgm(dayBGM);
            Day();
        }
        else
        {
            SoundManager.Instance.PlayBgm(nightBGM);
            Night();
        }
    }
    void Day()
    {
        if (img.sprite.name == "basic house night_0") img.sprite = sprites[0];
        else if (img.sprite.name == "black house night_0") img.sprite = sprites[2];
        else if (img.sprite.name == "starlight house night_0") img.sprite = sprites[4];
        else if (img.sprite.name == "white house night_0") img.sprite = sprites[6];
    }
    void Night()
    {
        if (img.sprite.name == "basic house day_0") img.sprite = sprites[1];
        else if (img.sprite.name == "black house day_0") img.sprite = sprites[3];
        else if (img.sprite.name == "starlight house day_0") img.sprite = sprites[5];
        else if (img.sprite.name == "white house day_0") img.sprite = sprites[7];
    }
}
