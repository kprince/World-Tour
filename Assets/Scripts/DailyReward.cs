using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    private Image img_daybg;
    private Image img_icon;
    private Text text_day;
    private Text text_rewardnum;
    private GameObject go_get;
    private void Awake()
    {
        img_daybg = GetComponent<Image>();
        img_icon = transform.GetChild(1).GetComponent<Image>();
        text_day = transform.GetChild(0).GetComponent<Text>();
        text_rewardnum = transform.GetChild(2).GetComponent<Text>();
        go_get = transform.GetChild(3).gameObject;
    }
    public void SetSignState(bool state,Sprite daybg,Sprite icon,string day,string num)
    {
        go_get.SetActive(state);
        img_daybg.sprite = daybg;
        img_icon.sprite = icon;
        text_day.text = day;
        text_rewardnum.text = num;
    }
}
