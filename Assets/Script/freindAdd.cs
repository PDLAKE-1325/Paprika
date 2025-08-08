using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class freindAdd : MonoBehaviour
{
    public Transform freindInventory;
    public Transform freindAddcontant;
    public Image myImage;
    public TMP_Text myText;
    public GameObject toggle;
    void Start()
    {
        findGameObject();
    }
    void Update()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current.name.Contains("contant(freind)"))
            {
                myImage.color=new Color(255,255,255);
                myText.text="following";
                myText.color=new Color(0,0,0);
                return;
            }
            else
            {
                myImage.color=new Color(0,0,0);
                myText.text="follow";
                myText.color=new Color(255,255,255);
            }
            current = current.parent;
        }
    }

    void findGameObject()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "contant(freind)")
            {
                freindInventory = obj.transform;
            }
            else if(obj.name=="contant(add)")
            {
                freindAddcontant=obj.transform;
            }
        }
        myText = GetComponentInChildren<TMP_Text>();

        if (freindInventory == null)
        {
            Debug.LogWarning("not find");
        }
        if (freindAddcontant == null)
        {
            Debug.LogWarning("not find");
        }
        if (myText == null)
        {
            Debug.LogWarning("not find");
        }
    }
    public void freindadd()
    {   
        Transform myParent = transform.parent;

        if(toggle.activeSelf) myParent.SetParent(freindInventory);
        else myParent.SetParent(freindAddcontant);

        toggle.SetActive(!toggle.activeSelf);
    }
}
