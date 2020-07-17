using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_ExtraReward : MG_UIBase
    {
        public GameObject go_Title;
        public GameObject go_Card;
        public GameObject go_Gift;
        public GameObject go_CashAll;
        public GameObject go_notice;
        public GameObject go_Des2;
        public GameObject go_AdIcon;
        public Transform trans_GiftLight;
        public Transform trans_CashLight;

        public Button btn_Open;
        public Button btn_Get;
        public Button btn_Giveup;
        public Text text_times;
        public Text text_rewardNum;
        public Text text_rewardCashNum;
        public Text text_rewardMutiple;

        public Image img_RewardIcon;
        public Image img_ManyCashIcon;

        Sprite sp_gold;
        Sprite sp_manyCash;
        SpriteAtlas shopSA;
        bool isPackB = false;
        protected override void Awake()
        {
            base.Awake();
            btn_Get.onClick.AddListener(OnGetButtonClick);
            btn_Giveup.onClick.AddListener(OnGiveupButtonClick);
            btn_Open.onClick.AddListener(OnOpenButtonClick);

            isPackB = MG_Manager.Instance.Get_Save_PackB();

            shopSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);
            sp_gold = shopSA.GetSprite("MG_Sprite_Shop_Gold");
            if (isPackB)
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconB");
            else
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconA");
            img_ManyCashIcon.sprite = sp_manyCash;
            go_Des2.SetActive(isPackB);

        }
        int clickTime = 0;
        void OnOpenButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (MG_SaveManager.FirstCome)
            {
                OnOpenAdCallback();
            }
            else
            {
                clickTime++;
                MG_Manager.ShowRV(OnOpenAdCallback, clickTime, "open extra reward");
            }
        }
        void OnOpenAdCallback()
        {
            clickTime = 0;
            go_notice.SetActive(false);
            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.ExtraCash:
                    go_Title.SetActive(false);
                    go_Gift.SetActive(false);
                    go_Card.SetActive(false);
                    go_CashAll.SetActive(true);
                    trans_GiftLight.gameObject.SetActive(false);
                    btn_Open.gameObject.SetActive(false);
                    btn_Get.gameObject.SetActive(true);
                    btn_Giveup.gameObject.SetActive(false);
                    StartCoroutine("AutoScaleLight");
                    break;
                case MG_PopRewardPanel_RewardType.ExtraGold:
                    go_Title.SetActive(true);
                    go_Gift.SetActive(false);
                    go_Card.SetActive(true);
                    btn_Open.gameObject.SetActive(false);
                    btn_Get.gameObject.SetActive(true);
                    btn_Giveup.gameObject.SetActive(false);
                    break;
            }
        }
        void OnGetButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (MG_SaveManager.FirstCome)
            {
                MG_SaveManager.FirstCome = false;
                MG_SaveManager.TodayExtraRewardTimes++;
            }
            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.ExtraGold:
                    MG_Manager.Instance.Add_Save_Gold(_rewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, _rewardNum);
                    MG_SaveManager.TodayExtraRewardTimes--;
                    break;
                case MG_PopRewardPanel_RewardType.ExtraCash:
                    MG_Manager.Instance.Add_Save_Cash(_rewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Cash, _rewardNum);
                    MG_SaveManager.TodayExtraRewardTimes--;
                    break;
                case MG_PopRewardPanel_RewardType.SignGold:
                    int numSigngold = (int)(_rewardNum * _rewardMutiple);
                    MG_Manager.Instance.Add_Save_Gold(numSigngold);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, numSigngold);
                    break;
                case MG_PopRewardPanel_RewardType.SignCash:
                    int numSigncash = (int)(_rewardNum * _rewardMutiple);
                    MG_Manager.Instance.Add_Save_Cash(numSigncash);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Cash, numSigncash);
                    break;
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.ExtraRewardPanel);
        }
        void OnGiveupButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_Manager.ShowIV(OnGiveupPopAdCallback, "extra reward giveup , type : " + _rewardType);
        }
        void OnGiveupPopAdCallback()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.ExtraRewardPanel);
        }
        MG_PopRewardPanel_RewardType _rewardType;
        int _rewardNum;
        float _rewardMutiple;
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            go_Title.SetActive(false);
            go_Card.SetActive(false);
            go_Gift.SetActive(true);
            go_CashAll.SetActive(false);
            go_notice.SetActive(true);
            btn_Open.gameObject.SetActive(true);
            btn_Get.gameObject.SetActive(false);
            btn_Giveup.gameObject.SetActive(true);
            trans_GiftLight.gameObject.SetActive(true);
            text_rewardMutiple.gameObject.SetActive(false);
            StartCoroutine("DelayShowGiveup");
            StartCoroutine("AutoRotateGiftLight");
            _rewardType = MG_Manager.Instance.MG_PopDiceReward_Type;
            _rewardNum = MG_Manager.Instance.MG_PopDiceReward_Num;
            text_times.text = "Remaining:" + MG_SaveManager.TodayExtraRewardTimes.ToString();

            go_AdIcon.SetActive(!MG_SaveManager.FirstCome);

            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.ExtraGold:
                    text_times.gameObject.SetActive(!MG_SaveManager.FirstCome);
                    text_rewardNum.text = _rewardNum.ToString();
                    img_RewardIcon.sprite = sp_gold;
                    break;
                case MG_PopRewardPanel_RewardType.ExtraCash:
                    text_times.gameObject.SetActive(!MG_SaveManager.FirstCome);
                    if (isPackB)
                        text_rewardCashNum.text = "$" + MG_Manager.Get_CashShowText(_rewardNum);
                    else
                        text_rewardCashNum.text = MG_Manager.Get_CashShowText(_rewardNum);
                    break;
                case MG_PopRewardPanel_RewardType.SignGold:
                    _rewardNum = MG_Manager.Instance.MG_SignRewardNum;
                    _rewardMutiple = MG_Manager.Instance.MG_SignRewardMutiple;
                    text_rewardNum.text = _rewardNum.ToString();
                    text_rewardMutiple.text = "<size=150><color=#FFE100>x" + _rewardMutiple + "</color></size>";
                    img_RewardIcon.sprite = sp_gold;
                    text_times.gameObject.SetActive(false);
                    go_Title.SetActive(true);
                    go_Gift.SetActive(false);
                    go_Card.SetActive(true);
                    text_rewardMutiple.gameObject.SetActive(true);
                    btn_Open.gameObject.SetActive(false);
                    btn_Get.gameObject.SetActive(true);
                    btn_Giveup.gameObject.SetActive(false);
                    go_notice.SetActive(false);
                    break;
                case MG_PopRewardPanel_RewardType.SignCash:
                    _rewardNum = MG_Manager.Instance.MG_SignRewardNum;
                    _rewardMutiple = MG_Manager.Instance.MG_SignRewardMutiple;
                    if (isPackB)
                        text_rewardCashNum.text = "$" + MG_Manager.Get_CashShowText((int)(_rewardNum * _rewardMutiple));
                    else
                        text_rewardCashNum.text = MG_Manager.Get_CashShowText((int)(_rewardNum * _rewardMutiple));
                    text_times.gameObject.SetActive(false);
                    go_CashAll.SetActive(true);
                    go_Title.SetActive(false);
                    go_Gift.SetActive(false);
                    go_Card.SetActive(false);
                    btn_Open.gameObject.SetActive(false);
                    btn_Get.gameObject.SetActive(true);
                    btn_Giveup.gameObject.SetActive(false);
                    go_notice.SetActive(false);
                    trans_GiftLight.gameObject.SetActive(false);
                    StartCoroutine("AutoScaleLight");
                    break;

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
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            StopCoroutine("DelayShowGiveup");
            StopCoroutine("AutoRotateGiftLight");
            StopCoroutine("AutoScaleLight");
            if (MG_Manager.Instance.hasGift)
            {
                MG_Manager.Instance.hasGift = false;
                MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
            }
            else if (MG_Manager.Instance.willRateus)
            {
                MG_Manager.Instance.willRateus = false;
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.Rateus);
            }
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {

        }
        IEnumerator DelayShowGiveup()
        {
            Image btn = btn_Giveup.image;
            btn.color = new Color(1, 1, 1, 0);
            btn.raycastTarget = false;
            yield return new WaitForSeconds(Time.timeScale);
            while (true)
            {
                yield return null;
                btn.color += new Color(0, 0, 0, Time.unscaledDeltaTime*2.5f);
                if (btn.color.a >= 0.9f)
                {
                    btn.color = Color.white;
                    btn.raycastTarget = true;
                    yield return null;
                    yield break;
                }
            }
        }
        IEnumerator AutoRotateGiftLight()
        {
            while (true)
            {
                yield return null;
                trans_GiftLight.Rotate(0, 0, -Time.unscaledDeltaTime*4);
            }
        }
        IEnumerator AutoScaleLight()
        {
            float maxScale = 1.3f;
            trans_CashLight.localScale = Vector3.one;
            bool isUp = true;
            while (true)
            {
                yield return null;
                Vector3 offset = new Vector2(Time.unscaledDeltaTime * 0.5f, Time.unscaledDeltaTime * 0.5f);
                trans_CashLight.localScale += isUp ? offset : -offset;
                if (trans_CashLight.localScale.x >= maxScale)
                    isUp = false;
                if (trans_CashLight.localScale.x <= 1)
                    isUp = true;
            }
        }
    }
}
