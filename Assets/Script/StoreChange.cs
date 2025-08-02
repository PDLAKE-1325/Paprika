using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;

public class StoreChange : MonoBehaviour
{
    public List<GameObject> Store;
    public List<GameObject> StoreButton;
    void Start()
    {
        ChangeStore(0);//시작시 기본 첫번째
    }
    public void ChangeStore(int index){
        for(int i=0;i<Store.Count;i++){
            Store[i].SetActive(i==index);//상점 바꾸기
            if(i==index){//선택한 상점의 버튼 위로 나옴
                Vector2 ButtonPos=StoreButton[i].transform.localPosition;
                ButtonPos.y=320f;
                StoreButton[i].transform.localPosition=ButtonPos;
            }else{//제자리로
                Vector2 ButtonPos=StoreButton[i].transform.localPosition;
                ButtonPos.y=300f;
                StoreButton[i].transform.localPosition=ButtonPos;
            }
        }
        
    }
}
