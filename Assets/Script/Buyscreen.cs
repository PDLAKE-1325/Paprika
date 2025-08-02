using UnityEngine;

public class Buyscreen : MonoBehaviour
{
    public GameObject buywindow;
    public void buywindowON(bool buywindowWhether){
        buywindow.SetActive(buywindowWhether);//사는 창 열고 닫기
    }
    public void ItemDivision(string name){// 버튼 마다 item 구분을 위한 함수
        Debug.Log(name); 
    }
}
