using UnityEngine;

public class Resolution : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution((int)(1080 * 0.5f), (int)(1920 * 0.5f), false);
    }
}
