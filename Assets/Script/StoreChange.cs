using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;
using NUnit.Framework.Constraints;

public class StoreChange : MonoBehaviour
{
    public List<GameObject> Store;
    public GameObject blackbar;
    void Start()
    {
        ChangeStore(0);//시작시 기본 첫번째
    }
    public void ChangeStore(int index)
    {
        blackbar.transform.localPosition=new Vector2(-440+index*140,250);
        for(int i=0;i<Store.Count;i++)
        {
            Store[i].SetActive(i==index);//상점 바꾸기
        } 
    }
}
