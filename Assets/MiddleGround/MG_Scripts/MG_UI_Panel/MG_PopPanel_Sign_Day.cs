using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Sign_Day : MonoBehaviour
    {
        Image img_rewardIcon;
        Image img_bg;
        GameObject go_mask;
        GameObject go_sure;
        Text text_rewardNum;
        Text text_day;
        private void Awake()
        {
            img_bg = GetComponent<Image>();
            img_rewardIcon = transform.GetChild(1).GetComponent<Image>();
            text_rewardNum = transform.GetChild(2).GetComponent<Text>();
            text_day = transform.GetChild(0).GetComponent<Text>();
            go_sure = transform.GetChild(3).gameObject;
            go_mask = transform.GetChild(4).gameObject;
        }
        public void SetDay(int day,int today, Sprite bgSp,Sprite rewardSp,string rewardNum,bool canSign)
        {
            text_day.text = "Day" + day;
            img_bg.sprite = bgSp;
            img_rewardIcon.sprite = rewardSp;
            text_rewardNum.text = rewardNum;
            day--;
            if (day < today)
            {
                go_sure.SetActive(true);
                go_mask.SetActive(false);
            }
            else if (day == today&&canSign)
            {
                go_mask.SetActive(false);
                go_sure.SetActive(false);
            }
            else
            {
                go_mask.SetActive(true);
                go_sure.SetActive(false);
            }
        }
    }
}
