using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_BuyEnergy : MG_UIBase
    {
        public Button btn_Close;
        public Button btn_Buy;
        protected override void Awake()
        {
            base.Awake();
            btn_Close.onClick.AddListener(OnCloseClick);
            btn_Buy.onClick.AddListener(OnBuyClick);
        }
        void OnCloseClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.BuyDiceEnergy);
        }
        int clickTime = 0;
        void OnBuyClick()
        {
            MG_Manager.Play_ButtonClick();
            clickTime++;
            MG_Manager.ShowRV(OnBuyCallback, clickTime, "buy dice energy");
        }
        void OnBuyCallback()
        {
            clickTime = 0;
            MG_Manager.Instance.Add_Save_DiceLife(10);
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.BuyDiceEnergy);
        }
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
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
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
