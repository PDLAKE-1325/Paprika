using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using JetBrains.Annotations;

public class Design : MonoBehaviour
{
   public Buyscreen buyscreen;
   public Image house;
   public Image bad;
   public Image sopa;
   public Image table;
   public Image pot;
   public Image pet;
   public void place(string name)
    {// 버튼 마다 item 구분을 위한 함수
        for (int i = 0; i < buyscreen.Item.Count; i++)
        {
            if (buyscreen.Item[i].name == name)
            {   
               type(i);
            }
        }
    }
    public void type(int index)
    {
        if(index<8)
        {
            bad.sprite=buyscreen.Item[index];
            bad.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        } 
        else if(index<16)
        {
            sopa.sprite=buyscreen.Item[index];
            sopa.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        }
        else if(index<24)
        {
            table.sprite=buyscreen.Item[index];
            table.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        } 
        else if(index<28||index==36)
        {
            house.sprite=buyscreen.Item[index];
            house.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        } 
        else if(index<32)
        {
            pot.sprite=buyscreen.Item[index];
            pot.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        } 
        else if(index<36)
        {
            pet.sprite=buyscreen.Item[index];
            pet.rectTransform.sizeDelta=buyscreen.GetSizeByIndex(index);
        }
        else return;
    }
}
