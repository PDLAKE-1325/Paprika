using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class Buyscreen : MonoBehaviour
{
    public GameObject buywindow;
    public Image buyImage;
    public List<Sprite> Item;
    public Text buytext;
    public void buywindowON(bool buywindowWhether)
    {
        buywindow.SetActive(buywindowWhether);//사는 창 열고 닫기
    }
    public void ItemDivision(string name)
    {// 버튼 마다 item 구분을 위한 함수
        for (int i = 0; i < Item.Count; i++)
        {
            if (Item[i].name == name)
            {
                buyImage.sprite = Item[i];
                buyImage.rectTransform.sizeDelta = GetSizeByIndex(i);
            }
        }
    }

    public Vector2 GetSizeByIndex(int index)//이미지 마다 크기가 다른 걸 고려한 함수
    {
        if (index < 5) return new Vector2(532, 386); // 앞이 가려진 침대
        if (index < 8) return new Vector2(518, 370); // 앞이 열린 침대
        if (index < 12) return new Vector2(309, 315); // 기본 소파
        if (index < 16) return new Vector2(377, 314); // 야외 의자
        if (index < 19) return new Vector2(447, 348); // 사각 테이블
        if (index < 24) return new Vector2(408, 303); // 반원 테이블
        if (index < 28||index==36) return new Vector2(1080 / 2, 1081 / 2); // 집
        // 아래는 각각 고정
        Vector2[] Flower_Pet_Sizes = {
            new Vector2(96*2, 183*2),  // 나팔꽃 i==28
            new Vector2(89*2, 177*2),  // 파프리카 i==29
            new Vector2(90*2, 200*2),  // 장미 i==30
            new Vector2(90*2, 233*2),  // 해바라기 i==31
            new Vector2(107*2, 46*2),  //강아지 i==32
            new Vector2(111*2, 67*2),  //래서 판다 i==33
            new Vector2(107*2, 63*2),   //우파루파 i==34
            new Vector2(100*2,95*2)     //고양이 i==35
        };

        if (index - 28 >= 0)
            return Flower_Pet_Sizes[index - 28];

        return new Vector2(100, 100); // 기본값
    }
    public void BuyText(string furniture)
    {
        buytext.text = furniture + " 을(를) 구매 하시겠습니까?";
    }

}
