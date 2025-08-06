using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;

public class StoreChange : MonoBehaviour
{
    public List<GameObject> Store;
    void Start()
    {
        ChangeStore(0);//시작시 기본 첫번째
    }
    public void ChangeStore(int index)
    {
        for(int i=0;i<Store.Count;i++)
        {
            Store[i].SetActive(i==index);//상점 바꾸기
        } 
    }
}
