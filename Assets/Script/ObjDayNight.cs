using UnityEngine;

public class ObjDayNight : MonoBehaviour
{
    [SerializeField] bool isAlltimeObj = true;
    [SerializeField] bool isDayObject;
    void Update()
    {
        if (isAlltimeObj) return;
        if (isDayObject)
        {
            gameObject.SetActive(TImeText.Instance.IsDay());
        }
        else
        {
            gameObject.SetActive(!TImeText.Instance.IsDay());
        }
    }
}
