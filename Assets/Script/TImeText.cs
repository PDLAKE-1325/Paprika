using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TImeText : MonoBehaviour
{
    [SerializeField] Text cur_time;
    [SerializeField] Text cur_mmdd;
    float time;
    void Update()
    {
        // cur_mmdd.text = $"{month}.{day}.";
        // cur_time.text = $"{hour:D2}:{minute:D2}";
        time += Time.deltaTime;
        if (time > 10)
        {
            print("rr");
            time = 0;
        }
    }

}
