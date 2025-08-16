using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class MainChange : MonoBehaviour
{
    [SerializeField] RectTransform playObjRect;
    public Design design;
    public List<GameObject> screen;
    public Image eye_open;
    public Image eye_close;
    void Start()
    {
        ChangeScreen(0);//시작시 기본 첫번째
        design.designReset();
    }
    public void ChangeScreen(int index)
    {
        if (index != 2)//일반적인 경우
        {
            for (int i = 0; i < screen.Count; i++)
            {
                screen[i].SetActive(i == index);//스크린 바꾸기
            }
        }
        else if (index == 2)//2번째인 친구 창은 메인 창과 같이 띄우기
        {
            for (int i = 0; i < screen.Count; i++)
            {
                screen[i].SetActive(i == index);//스크린 바꾸기
                // bool form = false;
                // if (i == 0 || i == 2) form = true;//기본은 false,0과2는 true
                // screen[i].SetActive(form);
            }
        }

        if (index < 0)//UI없애는 눈 기능
        {
            GlobalGameData.Instance.data.UI_on_off = GlobalGameData.Instance.data.UI_on_off ? false : true; //정반대로 바꾸기

            if (GlobalGameData.Instance.data.UI_on_off = GlobalGameData.Instance.data.UI_on_off)//2번 누르면 다시 함수 호출해서 원래 화면 보이게하기
            {
                ChangeScreen(GlobalGameData.Instance.data.screenIndex);
                eye_open.gameObject.SetActive(true);
                eye_close.gameObject.SetActive(false);
                playObjRect.offsetMin = new Vector2(playObjRect.offsetMin.x, 850f);
            }
            else
            {
                eye_open.gameObject.SetActive(false);
                eye_close.gameObject.SetActive(true);
                playObjRect.offsetMin = new Vector2(playObjRect.offsetMin.x, 150f);
            }
        }
        else
        {
            GlobalGameData.Instance.data.screenIndex = index;//지금 스크린 미리 저장
        }
    }
}
