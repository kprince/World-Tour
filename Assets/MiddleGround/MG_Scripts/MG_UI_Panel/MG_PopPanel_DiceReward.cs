using MiddleGround.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_DiceReward : MG_UIBase
    {
        public Image img_RewardIcon;
        public Image img_RandomLight;
        public Image img_Close;
        public Image img_NormalGiveup;
        public Image img_midRandomNum;
        private Image img_RandomSelect;
        public Image img_ManyCashIcon;

        public Transform trans_CardLight;
        public Transform trans_CashLight;
        public Transform trans_RandomSelect;
        public Transform[] trans_AllRanomNum = new Transform[5];
        readonly float[] mutiples = new float[5] { 2, 3, 5, 1, 1.5f };

        public Button btn_Spin;
        private Button btn_Close;
        private Button btn_NormalGiveup;

        public Text text_RewardNum;
        public Text text_RewardCashNum;

        public GameObject go_CashAll;
        public GameObject go_NotCashAll;
        public GameObject go_des2;

        Sprite sp_gold;
        Sprite sp_cash;
        Sprite sp_lightA;
        Sprite sp_lightB;
        SpriteAtlas shopSA;
        SpriteAtlas randomSA;

        protected override void Awake()
        {
            base.Awake();
            img_RandomSelect = trans_RandomSelect.GetComponent<Image>();
            btn_Close = img_Close.GetComponent<Button>();
            btn_NormalGiveup = img_NormalGiveup.GetComponent<Button>();

            btn_Spin.onClick.AddListener(OnSpinButtonClick);
            btn_Close.onClick.AddListener(OnCloseButtonClick);
            btn_NormalGiveup.onClick.AddListener(OnCloseButtonClick);

            shopSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);
            randomSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.Random);
            sp_gold = shopSA.GetSprite("MG_Sprite_Shop_Gold");
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            if (packB)
                sp_cash = shopSA.GetSprite("MG_Sprite_Shop_CashB");
            else
                sp_cash = shopSA.GetSprite("MG_Sprite_Shop_CashA");
            sp_lightA = randomSA.GetSprite("MG_Sprite_RandomLightA");
            sp_lightB = randomSA.GetSprite("MG_Sprite_RandomLightB");

            Sprite sp_manyCash;
            if (packB)
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconB");
            else
                sp_manyCash = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.DiceRewardPanel).GetSprite("MG_Sprite_CashPop_CashIconA");
            img_ManyCashIcon.sprite = sp_manyCash;
            go_des2.SetActive(packB);

        }
        int clickTime = 0;
        bool isSpining = false;
        void OnSpinButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isSpining) return;
            clickTime++;
            MG_Manager.ShowRV(OnSpinAdCallback, clickTime, "dice reward ad :" + _rewardType);
        }
        void OnSpinAdCallback()
        {
            clickTime = 0;
            StopCoroutine("AutoShineSelect");
            img_RandomSelect.color = Color.white;
            isSpining = true;
            StartCoroutine(SpinStart());
        }
        void OnCloseButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_Manager.ShowIV(OnNormalGiveupButtonClick, "dice marquee giveup, type:" + _rewardType);
        }
        void OnNormalGiveupButtonClick()
        {
            if (isSpining) return;
            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.Gold:
                case MG_PopRewardPanel_RewardType.ExtraGold:
                    MG_Manager.Instance.Add_Save_Gold(_rewardNum);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, _rewardNum);
                    break;
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DiceRewardPanel);

        }
        MG_PopRewardPanel_RewardType _rewardType;
        int _rewardNum;
        float _rewardMutiple;
        public override IEnumerator OnEnter()
        {
            clickTime = 0;
            trans_RandomSelect.localPosition = trans_AllRanomNum[0].localPosition;

            _rewardType = MG_Manager.Instance.MG_PopDiceReward_Type;
            _rewardNum = MG_Manager.Instance.MG_PopDiceReward_Num;
            _rewardMutiple = MG_Manager.Instance.MG_PopDiceReward_Mutiple;
            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.Gold:
                    go_CashAll.SetActive(false);
                    go_NotCashAll.SetActive(true);
                    text_RewardNum.text = _rewardNum.ToString();
                    btn_Close.gameObject.SetActive(true);
                    btn_NormalGiveup.gameObject.SetActive(false);
                    img_RewardIcon.sprite = sp_gold;
                    StartCoroutine(DelayShow(img_Close));
                    StartCoroutine("AutoRotateCardLight");
                    break;
                case MG_PopRewardPanel_RewardType.Cash:
                    go_CashAll.SetActive(true);
                    go_NotCashAll.SetActive(false);
                    bool packB = MG_Manager.Instance.Get_Save_PackB();
                    text_RewardCashNum.text = (packB ? "$" : "") + MG_Manager.Get_CashShowText(_rewardNum);
                    btn_Close.gameObject.SetActive(false);
                    btn_NormalGiveup.gameObject.SetActive(true);
                    img_RewardIcon.sprite = sp_cash;
                    StartCoroutine(DelayShow(img_NormalGiveup));
                    StartCoroutine("AutoScaleCardLight");
                    break;
            }
            StartCoroutine("AutoShineSelect");

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

            StopCoroutine("AutoRotateCardLight");
            StopCoroutine("AutoScaleCardLight");
            StopCoroutine("AutoShineSelect");
            if (MG_Manager.Instance.hasGift)
            {
                MG_Manager.Instance.hasGift = false;
                MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
            }
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
        IEnumerator AutoRotateCardLight()
        {
            while (true)
            {
                yield return null;
                trans_CardLight.Rotate(0, 0, -Time.unscaledDeltaTime * 3f);
            }
        }
        IEnumerator AutoScaleCardLight()
        {
            float maxScale = 1.3f;
            trans_CashLight.localScale = Vector3.one;
            bool isUp = true;
            while (true)
            {
                yield return null;
                trans_CashLight.localScale += isUp ? new Vector3(Time.unscaledDeltaTime / 2, Time.unscaledDeltaTime / 2) : -new Vector3(Time.unscaledDeltaTime / 2, Time.unscaledDeltaTime / 2);
                if (trans_CashLight.localScale.x >= maxScale)
                    isUp = false;
                if (trans_CashLight.localScale.x <= 1)
                    isUp = true;
            }
        }
        IEnumerator AutoShineSelect()
        {
            bool ligthOn = true;
            img_RandomSelect.color = Color.white;
            while (true)
            {
                yield return new WaitForSeconds(0.1f * Time.timeScale);
                ligthOn = !ligthOn;
                img_RandomSelect.color = ligthOn ? Color.white : Color.clear;
            }
        }
        IEnumerator SpinStart()
        {
            int turn = 3;
            int length = trans_AllRanomNum.Length;
            bool isLightA = true;
            img_RandomLight.sprite = sp_lightA;
            AudioSource as_spin = MG_AudioManager.Instance.PlayOneShot(MG_PlayAudioType.SpinSlots);
            while (turn > 0)
            {
                for(int i = 0; i < length; i++)
                {
                    yield return new WaitForSeconds(0.1f * Time.timeScale);
                    isLightA = !isLightA;
                    img_RandomLight.sprite = isLightA ? sp_lightA : sp_lightB;
                    trans_RandomSelect.localPosition = trans_AllRanomNum[i].localPosition;
                    if (mutiples[i] == _rewardMutiple)
                    {
                        turn--;
                        if (turn == 0)
                        {
                            yield return null;
                            break;
                        }
                    }
                }
            }
            as_spin.Stop();
            yield return new WaitForSeconds(0.5f * Time.timeScale);
            switch (_rewardType)
            {
                case MG_PopRewardPanel_RewardType.Gold:
                    int numG = (int)(_rewardNum * _rewardMutiple);
                    MG_Manager.Instance.Add_Save_Gold(numG);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.OneGold, numG);
                    break;
                case MG_PopRewardPanel_RewardType.Cash:
                    int numC = (int)(_rewardNum * _rewardMutiple);
                    MG_Manager.Instance.Add_Save_Cash(numC);
                    MG_UIManager.Instance.FlyEffectTo_MenuTarget(img_RewardIcon.transform.position, MG_MenuFlyTarget.Cash,numC);
                    break;
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.DiceRewardPanel);
            isSpining = false;
        }
        IEnumerator DelayShow(Image btn)
        {
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
    }
}
