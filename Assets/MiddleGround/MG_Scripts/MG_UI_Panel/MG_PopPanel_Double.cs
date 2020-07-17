using MiddleGround.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Double : MG_UIBase
    {
        public Button btn_Close;
        public Button btn_AD;
        public Image img_RewardIcon;
        public Text text_ReawrdNum;
        public Text text_AdDes;
        Transform trans_AdDes;
        public Transform trans_Light;
        public GameObject go_AdIcon;
        SpriteAtlas shopAtlas;
        readonly Dictionary<int, Sprite> dic_type_sp = new Dictionary<int, Sprite>();
        protected override void Awake()
        {
            base.Awake();
            btn_Close.onClick.AddListener(OnCloseButtonClick);
            btn_AD.onClick.AddListener(OnAdButtonClick);
            trans_AdDes = text_AdDes.transform;
            shopAtlas = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);
            Type t = typeof(MG_PopDoublePanel_RewardType);
            for(int i = 0; i < (int)MG_PopDoublePanel_RewardType.TypeNum; i++)
            {
                string fullName = "MG_Sprite_Shop_" + t.GetEnumName(i);
                dic_type_sp.Add(i, shopAtlas.GetSprite(fullName));
            }
        }
        void OnCloseButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (rewardType == MG_PopDoublePanel_RewardType.SignScratchTicket)
            {
                MG_Manager.Instance.Add_Save_ScratchTicket(rewardNum);
                MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Scratch, rewardNum);
                MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DoublePanel);
            }
            else
                MG_Manager.ShowIV(OnPopAdCallback, "reward panel close , type :" + rewardType);
        }
        void OnPopAdCallback()
        {
            if (rewardType == MG_PopDoublePanel_RewardType.Gold)
            {
                MG_Manager.Instance.Add_Save_Gold(rewardNum);
                MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, rewardNum);
            }
            else if (rewardType == MG_PopDoublePanel_RewardType.Scratch)
            {
                MG_Manager.Instance.Add_Save_ScratchTicket(1);
                MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Scratch, 1);
            }
            else if (rewardType == MG_PopDoublePanel_RewardType.Diamond)
            {
                MG_Manager.Instance.Add_Save_Diamond(rewardNum);
                MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Diamond, rewardNum);
            }
            else if (rewardType == MG_PopDoublePanel_RewardType.WheelTicket)
            {
                MG_Manager.Instance.Add_Save_WheelTickets(rewardNum);
                MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.WheelTicket, rewardNum);
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DoublePanel);
        }
        int clickTime = 0;
        void OnAdButtonClick()
        {
            MG_Manager.Play_ButtonClick();

            if (needAd)
            {
                clickTime++;
                MG_Manager.ShowRV(OnAdCallback, clickTime, "get reward" + rewardType);
            }
            else
            {
                OnAdCallback();
            }

        }
        void OnAdCallback()
        {
            clickTime = 0;
            switch (rewardType)
            {
                case MG_PopDoublePanel_RewardType.Gold:
                    int numGold = rewardNum * 2;
                    MG_Manager.Instance.Add_Save_Gold(numGold);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, numGold);
                    break;
                case MG_PopDoublePanel_RewardType.SSS:
                    int reward777Num = GetSpecialRewardByTimes(MG_SaveManager.Get777Times);
                    MG_SaveManager.Get777Times++;
                    MG_Manager.Instance.Add_Save_777(reward777Num);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.SSS, reward777Num);
                    break;
                case MG_PopDoublePanel_RewardType.Orange:
                    int rewardOrangeNum = GetSpecialRewardByTimes(MG_SaveManager.GetFruitsTimes);
                    MG_SaveManager.GetFruitsTimes++;
                    MG_Manager.Instance.Add_Save_Fruits(rewardOrangeNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Orange, rewardOrangeNum);
                    break;
                case MG_PopDoublePanel_RewardType.Cherry:
                    int rewardCherryNum = GetSpecialRewardByTimes(MG_SaveManager.GetFruitsTimes);
                    MG_SaveManager.GetFruitsTimes++;
                    MG_Manager.Instance.Add_Save_Fruits(rewardCherryNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Cherry, rewardCherryNum);
                    break;
                case MG_PopDoublePanel_RewardType.Watermalen:
                    int rewardWatermaleNum = GetSpecialRewardByTimes(MG_SaveManager.GetFruitsTimes);
                    MG_SaveManager.GetFruitsTimes++;
                    MG_Manager.Instance.Add_Save_Fruits(rewardWatermaleNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Watermalen, rewardWatermaleNum);
                    break;
                case MG_PopDoublePanel_RewardType.ScratchTicket:
                    MG_Manager.Instance.Add_Save_ScratchTicket(2);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.ScratchTicket, 2);
                    break;
                case MG_PopDoublePanel_RewardType.Scratch:
                    MG_Manager.Instance.Add_Save_ScratchTicket(2);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Scratch, 2);
                    break;
                case MG_PopDoublePanel_RewardType.Amazon:
                    int rewardAmazonNum = GetSpecialRewardByTimes(MG_SaveManager.GetAmazonTimes);
                    MG_SaveManager.GetAmazonTimes++;
                    MG_Manager.Instance.Add_Save_Amazon(rewardAmazonNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Amazon, rewardAmazonNum);
                    break;
                case MG_PopDoublePanel_RewardType.Diamond:
                    int numDiamond = rewardNum * 2;
                    MG_Manager.Instance.Add_Save_Diamond(numDiamond);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Diamond, numDiamond);
                    break;
                case MG_PopDoublePanel_RewardType.WheelTicket:
                    int numWheel = rewardNum * 2;
                    MG_Manager.Instance.Add_Save_WheelTickets(numWheel);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.WheelTicket, numWheel);
                    break;
                case MG_PopDoublePanel_RewardType.SignScratchTicket:
                    MG_Manager.Instance.Add_Save_ScratchTicket(rewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Scratch, rewardNum);
                    break;
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DoublePanel);
        }
        int GetSpecialRewardByTimes(int times)
        {
            if (times == 0)
                return 3;
            else if (times == 1 || times == 2)
                return 2;
            else
                return 1;
        }
        int rewardNum = 0;
        MG_PopDoublePanel_RewardType rewardType;
        bool needAd = false;
        bool needDouble = false;
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            rewardNum = MG_Manager.Instance.MG_PopDoublePanel_Num;
            rewardType = MG_Manager.Instance.MG_PopDoublePanel_Type;
            img_RewardIcon.sprite = dic_type_sp[(int)rewardType];

            switch (rewardType)
            {
                case MG_PopDoublePanel_RewardType.SSS:
                    SetSpecialShowTextButton(MG_SaveManager.Get777Times);
                    if(!MG_SaveManager.GuidScratch)
                        MG_Manager.Instance.next_GuidType = MG_Guid_Type.ScratchGuid;
                    break;
                case MG_PopDoublePanel_RewardType.Amazon:
                    SetSpecialShowTextButton(MG_SaveManager.GetAmazonTimes);
                    break;
                case MG_PopDoublePanel_RewardType.Cherry:
                case MG_PopDoublePanel_RewardType.Orange:
                case MG_PopDoublePanel_RewardType.Watermalen:
                    SetSpecialShowTextButton(MG_SaveManager.GetFruitsTimes);
                    break;
                case MG_PopDoublePanel_RewardType.Diamond:
                    if(!MG_SaveManager.GuidSlots)
                        MG_Manager.Instance.next_GuidType = MG_Guid_Type.SlotsGuid;

                    go_AdIcon.SetActive(true);
                    needAd = true;
                    needDouble = true;
                    trans_AdDes.localPosition = new Vector2(31.512f, 6.1525f);
                    text_AdDes.text = "GET  x2";
                    break;
                case MG_PopDoublePanel_RewardType.Gold:
                case MG_PopDoublePanel_RewardType.Scratch:
                case MG_PopDoublePanel_RewardType.WheelTicket:
                    go_AdIcon.SetActive(true);
                    needAd = true;
                    needDouble = true;
                    trans_AdDes.localPosition = new Vector2(31.512f, 6.1525f);
                    text_AdDes.text = "GET  x2";
                    break;
                case MG_PopDoublePanel_RewardType.SignScratchTicket:
                    go_AdIcon.SetActive(false);
                    needAd = false;
                    needDouble = false;
                    trans_AdDes.localPosition = new Vector2(0, 6.1525f);
                    text_AdDes.text = "GET";
                    break;
                default:
                    go_AdIcon.SetActive(true);
                    needAd = true;
                    needDouble = false;
                    trans_AdDes.localPosition = new Vector2(31.512f, 6.1525f);
                    text_AdDes.text = "GET";
                    break;
            }

            text_ReawrdNum.text = rewardNum.ToString();
            StartCoroutine("AutoRotateLight");

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
        void SetSpecialShowTextButton(int times)
        {
            if (times == 0)
            {
                go_AdIcon.SetActive(false);
                trans_AdDes.localPosition = new Vector2(0, 6.1525f);
                rewardNum = 3;
                text_AdDes.text = "GET";
                needAd = false;
                needDouble = false;
            }
            else if (times == 1)
            {
                go_AdIcon.SetActive(false);
                trans_AdDes.localPosition = new Vector2(0, 6.1525f);
                rewardNum = 2;
                text_AdDes.text = "GET";
                needAd = false;
                needDouble = false;
            }
            else if (times == 2)
            {
                go_AdIcon.SetActive(true);
                trans_AdDes.localPosition = new Vector2(31.512f, 6.1525f);
                rewardNum = 2;
                text_AdDes.text = "GET";
                needAd = true;
                needDouble = false;
            }
            else
            {
                go_AdIcon.SetActive(true);
                trans_AdDes.localPosition = new Vector2(31.512f, 6.1525f);
                text_AdDes.text = "RANDOM x1~2";
                needAd = true;
                needDouble = false;
            }
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

            StopCoroutine("AutoRotateLight");
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator AutoRotateLight()
        {
            while (true)
            {
                yield return null;
                trans_Light.Rotate(0, 0, -Time.unscaledDeltaTime*4);
            }
        }
    }
}
