using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class Design : MonoBehaviour
{
   public Buyscreen buyscreen;
   public List<Image> design;
   public Image house;
   public Image bad;
   public Image sopa;
   public Image table;
   public Image pot;
   public Image pet;
    public void designReset()
    {
        for(int i=0;i<design.Count;i++)
        {
            for(int j=0;j<buyscreen.Item.Count;j++)
            {
                if(GlobalGameData.Instance.data.designSetting[i]==buyscreen.Item[j].name)
                {
                    design[i].sprite=buyscreen.Item[j];
                    design[i].rectTransform.sizeDelta=buyscreen.GetSizeByIndex(j);
                }   
            }
        }
    }
    public void place(string name)// 버튼 마다 item 구분을 위한 함수
    {
        for (int i = 0; i < buyscreen.Item.Count; i++)
        {
            if (buyscreen.Item[i].name == name)
            {
               int index=types(i);
               GlobalGameData.Instance.data.designSetting[index]=name;
               designReset();
            }
        }
    }

    public int types(int index)
    {
        if(index<8)return 1;
        else if(index<16)return 2;
        else if(index<24)return 3;
        else if(index<28||index==36)return 0;
        else if(index<32)return 4;
        else if(index<36)return 5;
        else return -1;
    }
}
