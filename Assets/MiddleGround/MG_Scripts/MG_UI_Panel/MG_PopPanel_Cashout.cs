using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Cashout : MG_UIBase                //only scratch & slots panel
    {
        public Image img_cashicon;
        public Text text_cashNum;
        public Text text_adtime;
        public Button btn_savewallet;
        public Transform trans_light;
        public GameObject go_des2;
        bool isPackB;
        protected override void Awake()
        {
            base.Awake();
            btn_savewallet.onClick.AddListener(OnSaveWalletClick);
            isPackB = MG_Manager.Instance.Get_Save_PackB();
            Sprite sp_manyCash;
            if (isPackB)
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconB");
            else
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconA");
            img_cashicon.sprite = sp_manyCash;
            go_des2.SetActive(isPackB);
        }
        int cashNum = 0;
        int clickTime = 0;
        void OnSaveWalletClick()
        {
            MG_Manager.Play_ButtonClick();
            clickTime++;
            MG_Manager.ShowRV(OnAdCallback, clickTime, "get" + (MG_SaveManager.Current_GamePanel == 0 ? "Scratch" : "Slots") + "  cash");
        }
        void OnAdCallback()
        {
            clickTime = 0;
            MG_Manager.Instance.Add_Save_Cash(cashNum);
            MG_SaveManager.TodayExtraRewardTimes--;
            MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_cashicon.transform.position, MG_MenuFlyTarget.Cash, cashNum);
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.CashoutPanel);
        }
        public override IEnumerator OnEnter()
        {

            clickTime = 0;
            cashNum = MG_Manager.Instance.MG_PopCashPanel_Num;
            text_cashNum.text = (isPackB ? "$" : "") + MG_Manager.Get_CashShowText(cashNum);
            text_adtime.text = "Remaining:" + MG_SaveManager.TodayExtraRewardTimes;
            StartCoroutine("AutoScaleLight");

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
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            StopCoroutine("AutoScaleLight");
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator AutoScaleLight()
        {
            float maxScale = 1.3f;
            trans_light.localScale = Vector3.one;
            bool isUp = true;
            Vector3 offset;
            while (true)
            {
                yield return null;
                offset = new Vector2(Time.unscaledDeltaTime * 0.15f, Time.unscaledDeltaTime * 0.15f);
                trans_light.localScale += isUp ? offset : -offset;
                if (trans_light.localScale.x >= maxScale)
                    isUp = false;
                if (trans_light.localScale.x <= 1)
                    isUp = true;
            }
        }
    }
}
