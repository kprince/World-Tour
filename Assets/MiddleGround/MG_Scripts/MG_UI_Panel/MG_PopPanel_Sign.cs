
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Sign : MG_UIBase
    {
        public List<MG_PopPanel_Sign_Day> list_alldays = new List<MG_PopPanel_Sign_Day>();
        int[] rewards = new int[7] { 2000, 100, 2000, 2000, 100, 100, 100 };
        bool[] isGold = new bool[7] { true, false, true, true, false, false, false };
        float[] rewardmutiples = new float[7] { 3, 1.5f, 1.5f, 5, 1.5f, 1.5f, 5 };
        Sprite sp_gold;
        Sprite sp_cash;
        Sprite sp_scratchTicket;
        Sprite sp_lastBg;
        Sprite sp_thisBg;
        Sprite sp_nextBg;
        SpriteAtlas signSA;
        public Button btn_Sign;
        public Button btn_Nothanks;
        protected override void Awake()
        {
            base.Awake();
            signSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.SignPanel);
            sp_gold = signSA.GetSprite("MG_Sprite_Sign_Gold");
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            sp_cash = signSA.GetSprite("MG_Sprite_Sign_Cash" + (packB ? "B" : "A"));
            sp_scratchTicket = signSA.GetSprite("MG_Sprite_Sign_ScratchTicket");
            sp_lastBg = signSA.GetSprite("MG_Sprite_Sign_Signed");
            sp_thisBg = signSA.GetSprite("MG_Sprite_Sign_Signing");
            sp_nextBg = sp_lastBg;
            btn_Sign.onClick.AddListener(OnSignButtonClick);
            btn_Nothanks.onClick.AddListener(OnNothanksClick);
        }
        int clickTime = 0;
        void OnSignButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (MG_Manager.Instance.Get_Save_WetherSign())
            {
                clickTime++;
                MG_Manager.ShowRV(OnAdCallback, clickTime, "signin ad");
            }
            else
            {
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SignPanel);
                MG_Manager.Instance.Show_PopTipsPanel("You have signed today.");
            }
        }
        void OnAdCallback()
        {
            clickTime = 0;
            int day = MG_Manager.Instance.Get_Save_NextSignDay();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SignPanel);
            MG_Manager.Instance.Signin(true);
            if (day < 7)
                MG_Manager.Instance.Show_SignRewardPanel_Reward(isGold[day] ? MG_PopRewardPanel_RewardType.SignGold : MG_PopRewardPanel_RewardType.SignCash, rewards[day], rewardmutiples[day]);
            else
                MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.SignScratchTicket, 5);
            MG_UIManager.Instance.UpdateSignRP();
        }
        void OnNothanksClick()
        {
            MG_Manager.Play_ButtonClick();
            if (MG_Manager.Instance.Get_Save_WetherSign())
            {
                int day = MG_Manager.Instance.Get_Save_NextSignDay();
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SignPanel);
                MG_Manager.Instance.Signin(false);
                if (day < 7)
                    MG_Manager.Instance.Show_SignRewardPanel_Reward(isGold[day] ? MG_PopRewardPanel_RewardType.SignGold : MG_PopRewardPanel_RewardType.SignCash, rewards[day], 1);
                else
                    MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.SignScratchTicket, 1);
                MG_UIManager.Instance.UpdateSignRP();
            }
            else
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SignPanel);
        }
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            bool canSign = MG_Manager.Instance.Get_Save_WetherSign();
            if (list_alldays.Count != rewards.Length)
                Debug.LogError("Set MG_Sign Reward Error : exist is not match config.");
            else
            {
                int lastSignDay = MG_Manager.Instance.Get_Save_NextSignDay();
                string signState = MG_Manager.Instance.Get_Save_SignStatePerDay();
                bool changeScratchTicket = false;
                if (lastSignDay >= 7)
                {
                    changeScratchTicket = true;
                    lastSignDay %= 7;
                }
                for (int i = 0; i < 7; i++)
                {
                    Sprite bg;
                    char state = signState[i];
                    bool getAd = state == '1';
                    if (i == lastSignDay&&canSign)
                        bg = sp_thisBg;
                    else if (i < lastSignDay)
                        bg = sp_lastBg;
                    else
                        bg = sp_nextBg;
                    if (changeScratchTicket)
                    {
                        if (i < lastSignDay)
                            list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_scratchTicket, getAd ? "5" : "1", canSign);
                        else
                            list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_scratchTicket, "?", canSign);
                    }
                    else
                    {
                        if (isGold[i])
                        {
                            if (i < lastSignDay)
                                list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_gold, getAd ? (rewards[i] * rewardmutiples[i]).ToString() : rewards[i].ToString(), canSign);
                            else
                                list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_gold, rewards[i].ToString(), canSign);
                        }
                        else
                        {
                            if (i < lastSignDay)
                                list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_cash, MG_Manager.Get_CashShowText(getAd ? (int)(rewards[i] * rewardmutiples[i]) : (int)rewards[i]), canSign);
                            else
                                list_alldays[i].SetDay(i + 1, lastSignDay, bg, sp_cash, "?", canSign);
                        }
                    }
                }
            }

            Transform transAll = transform.GetChild(1);
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = true;
            while (transAll.localScale.x < 1)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale += new Vector3(addValue, addValue);
                canvasGroup.alpha += addValue;
            }
            transAll.localScale = Vector3.one;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;

        }

        public override IEnumerator OnExit()
        {
            Transform transAll = transform.GetChild(1);
            canvasGroup.interactable = false;
            while (transAll.localScale.x > 0.8f)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale -= new Vector3(addValue, addValue);
                canvasGroup.alpha -= addValue;
            }
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            MG_UIManager.Instance.UpdateWheelRP();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
