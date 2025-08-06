using UnityEngine;
using System.Collections.Generic;
public class MainChange : MonoBehaviour
{
    public List<GameObject> screen;
    void Start()
    {
        ChangeScreen(GlobalGameData.Instance.data.screenIndex);//시작시 기본 첫번째
    }
    public void ChangeScreen(int index)
    {
        if(index!=2)//일반적인 경우
        {
            for(int i=0;i<screen.Count;i++)
            {
                screen[i].SetActive(i==index);//스크린 바꾸기
            }
        }
        else if(index==2)//2번째인 친구 창은 메인 창과 같이 띄우기
        {
           for(int i=0;i<screen.Count;i++){
                bool form=false;
                if(i==0||i==2) form=true;//기본은 false,0과2는 true
                screen[i].SetActive(form);     
            }
        }

        if(index<0)//UI없애는 눈 기능
        {
            GlobalGameData.Instance.data.UI_on_off=GlobalGameData.Instance.data.UI_on_off?false:true; //정반대로 바꾸기

            if(GlobalGameData.Instance.data.UI_on_off=GlobalGameData.Instance.data.UI_on_off)//2번 누르면 다시 함수 호출해서 원래 화면 보이게하기
            {
                ChangeScreen(GlobalGameData.Instance.data.screenIndex);
            }
        }
        else
        {
            GlobalGameData.Instance.data.screenIndex=index;//지금 스크린 미리 저장
        }
    }
}
