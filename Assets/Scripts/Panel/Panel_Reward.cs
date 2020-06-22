using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Reward : PanelBase
{
    public CanvasGroup rewardCG;
    public CanvasGroup bounusNoticeCG;
    private SpriteAtlas Atlas;
    public GameObject slider;
    public Transform trans_handle;
    public Image img_title;
    public Image img_mid;
    private RectTransform rect_mid;
    public Image img_midMutiple;
    public Image img_Light;
    public Transform trans_bgLight;
    public Transform trans_giftLight;

    public Image img_rewardicon;
    public Text text_rewardNum;
    public Text text_nothanks1;
    public Text text_nothanks2;
    public Button btn_adGet;
    public Button btn_adOpen;
    public Button btn_get;
    private int rewardNum = 0;
    private RewardType rewardType = RewardType.Null;
    private float rewardMutiple = 0;
    const string Atlas_UnknownMutiple_Key = "Mutiple_Unknown";
    const string Atlas_TwoMutiple_Key = "Mutiple_2";
    const string Atlas_ManyGold_Key = "ManyGold";
    const string Atlas_ManyCash_A_Key = "ManyCashA";
    const string Atlas_ManyCash_B_Key = "ManyCashB";
    const string Atlas_Light_A_Key = "LightA";
    const string Atlas_Light_B_Key = "LightB";
    const string Atlas_GoldIcon_Key = "Gold";
    const string Atlas_CashAIcon_Key = "CashA";
    const string Atlas_CashBIcon_Key = "CashB";
    protected override void Awake()
    {
        base.Awake();
        btn_adGet.onClick.AddListener(OnAdGetClick);
        btn_adOpen.onClick.AddListener(OnOpenClick);
        btn_get.onClick.AddListener(OnGetClick);
        rect_mid = img_mid.GetComponent<RectTransform>();

        text_nothanks1.GetComponent<Button>().onClick.AddListener(Close);
        text_nothanks2.GetComponent<Button>().onClick.AddListener(Close);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        clickAdTime = 0;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (Atlas is null)
            Atlas = Resources.Load<SpriteAtlas>("RewardPanel");
        rewardType = GameManager.Instance.nextRewardType;
        rewardNum = GameManager.Instance.nextRewardNum;
        rewardMutiple = GameManager.Instance.nextRewardMutiple;
        isSliding = false;
        hasGet = false;
        text_nothanks1.color = new Color(1, 1, 1, 0);
        text_nothanks2.color = new Color(1, 1, 1, 0);
        text_nothanks2.raycastTarget = false;
        img_midMutiple.sprite = Atlas.GetSprite(Atlas_UnknownMutiple_Key);
        img_Light.sprite = Atlas.GetSprite(Atlas_Light_A_Key);
        btn_close.gameObject.SetActive(false);
        StartCoroutine("RotateLight");
        bool canShowExchange = GameManager.Instance.GetShowExchange();
        switch (rewardType)
        {
            case RewardType.Gold:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_mid.sprite = Atlas.GetSprite(Atlas_ManyGold_Key);
                img_rewardicon.sprite= Atlas.GetSprite(Atlas_GoldIcon_Key);
                text_rewardNum.text = rewardNum.ToString();
                btn_adGet.gameObject.SetActive(true);
                btn_get.gameObject.SetActive(false);
                StartCoroutine(DelayShowCloseBtn());
                slider.SetActive(true);
                break;
            case RewardType.Cash:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_mid.sprite = Atlas.GetSprite(canShowExchange ? Atlas_ManyCash_B_Key : Atlas_ManyCash_A_Key);
                img_rewardicon.sprite = Atlas.GetSprite(canShowExchange ? Atlas_CashBIcon_Key : Atlas_CashAIcon_Key);

                string cashString = rewardNum.ToString();
                if (rewardNum < 10)
                    cashString = cashString.Insert(0, "0.0");
                else if (rewardNum < 100)
                    cashString = cashString.Insert(0, "0.");
                else
                    cashString = cashString.Insert(cashString.Length - 2, ".");
                text_rewardNum.text = cashString;

                btn_adGet.gameObject.SetActive(true);
                btn_get.gameObject.SetActive(false);

                StartCoroutine(DelayShowNothanks(text_nothanks1, 0.53f));
                slider.SetActive(true);
                break;
            case RewardType.ExtraBonusGold:
                rewardCG.alpha = 0;
                rewardCG.blocksRaycasts = false;
                bounusNoticeCG.alpha = 1;
                bounusNoticeCG.blocksRaycasts = true;
                img_mid.sprite = Atlas.GetSprite(Atlas_ManyGold_Key);
                img_rewardicon.sprite = Atlas.GetSprite(Atlas_GoldIcon_Key);
                text_rewardNum.text = rewardNum.ToString();
                btn_adGet.gameObject.SetActive(false);
                btn_get.gameObject.SetActive(true);
                slider.SetActive(false);
                StartCoroutine(DelayShowNothanks(text_nothanks2));
                break;
            case RewardType.ExtraBonusCash:
                rewardCG.alpha = 0;
                rewardCG.blocksRaycasts = false;
                bounusNoticeCG.alpha = 1;
                bounusNoticeCG.blocksRaycasts = true;
                img_mid.sprite = Atlas.GetSprite(canShowExchange ? Atlas_ManyCash_B_Key : Atlas_ManyCash_A_Key);
                img_rewardicon.sprite = Atlas.GetSprite(canShowExchange ? Atlas_CashBIcon_Key : Atlas_CashAIcon_Key);

                string cashString1 = rewardNum.ToString();
                if (rewardNum < 10)
                    cashString1 = cashString1.Insert(0, "0.0");
                else if (rewardNum < 100)
                    cashString1 = cashString1.Insert(0, "0.");
                else
                    cashString1 = cashString1.Insert(cashString1.Length - 2, ".");
                text_rewardNum.text = cashString1;

                btn_adGet.gameObject.SetActive(false);
                btn_get.gameObject.SetActive(true);
                slider.SetActive(false);
                if (!GameManager.Instance.CheckFirstSignin(true))
                    StartCoroutine(DelayShowNothanks(text_nothanks2));
                break;
            case RewardType.NoticeBonus:
                break;
            case RewardType.SigninGold:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_mid.sprite = Atlas.GetSprite(Atlas_ManyGold_Key);
                img_rewardicon.sprite = Atlas.GetSprite(Atlas_GoldIcon_Key);
                text_rewardNum.text = string.Format("{0}  <size=150><color=#FFE100>x{1}</color></size>", rewardNum, rewardMutiple);

                rewardNum = (int)(rewardNum * rewardMutiple);

                btn_adGet.gameObject.SetActive(false);
                btn_get.gameObject.SetActive(true);
                slider.SetActive(false);
                break;
            case RewardType.SigninCash:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_mid.sprite = Atlas.GetSprite(canShowExchange ? Atlas_ManyCash_B_Key : Atlas_ManyCash_A_Key);
                img_rewardicon.sprite = Atlas.GetSprite(canShowExchange ? Atlas_CashBIcon_Key : Atlas_CashAIcon_Key);

                string cashString2 = (rewardNum * rewardMutiple).ToString();
                if (rewardNum < 10)
                    cashString2 = cashString2.Insert(0, "0.0");
                else if (rewardNum < 100)
                    cashString2 = cashString2.Insert(0, "0.");
                else
                    cashString2 = cashString2.Insert(cashString2.Length - 2, ".");
                text_rewardNum.text = cashString2;

                rewardNum = (int)(rewardNum * rewardMutiple);

                btn_adGet.gameObject.SetActive(false);
                btn_get.gameObject.SetActive(true);
                slider.SetActive(false);
                break;
            default:
                PanelManager.Instance.ClosePanel(PanelType.Reward);
                break;
        }
        trans_handle.localPosition = new Vector3(posX[2], 0);
        img_mid.SetNativeSize();
        rect_mid.sizeDelta *= 1.5f;
    }
    bool isSliding = false;
    bool hasGet = false;
    int clickAdTime = 0;
    void OnAdGetClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isSliding) return;
#if UNITY_EDITOR
        OnadGetRewardedCallBack();
        return;
#endif
#if UNITY_IOS
        if (!GameManager.Instance.GetShowExchange())
        {
            OnadGetRewardedCallBack();
            return;
        }
#endif
        clickAdTime++;
        Ads._instance.SetRewardedCallBack(OnadGetRewardedCallBack);
        Ads._instance.adDes = rewardType.ToString() + "的倍数获得广告";
        Ads._instance.ShowRewardVideo(clickAdTime);
    }
    void OnadGetRewardedCallBack()
    {
        isSliding = true;
        if (!hasGet)
        {
            hasGet = true;
            img_midMutiple.sprite = Atlas.GetSprite(Atlas_TwoMutiple_Key);
            StartCoroutine(StartSliding());
        }
    }
    void OnNothanksClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isSliding) return;
#if !UNITY_EDITOR
        Ads._instance.adDes = rewardType.ToString() + "的nothanks";
        Ads._instance.ShowInterstialAd();
#endif
        if (!hasGet && rewardType == RewardType.Gold) 
        {
            rewardMutiple = 1;
            GetReward();
            hasGet = true;
        }
        PanelManager.Instance.ClosePanel(PanelType.Reward);
    }
    void OnOpenClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (GameManager.Instance.CheckFirstSignin(false))
        {
            OnAdopenRewardedCallback();
            return;
        }
#if UNITY_EDITOR
        OnAdopenRewardedCallback();
        return;
#endif
#if UNITY_IOS
        if (!GameManager.Instance.GetShowExchange())
        {
            OnAdopenRewardedCallback();
            return;
        }
#endif
        clickAdTime++;
        Ads._instance.SetRewardedCallBack(OnAdopenRewardedCallback);
        Ads._instance.adDes = "惊喜礼盒打开";
        Ads._instance.ShowRewardVideo(clickAdTime);
    }
    void OnAdopenRewardedCallback()
    {
        rewardCG.alpha = 1;
        rewardCG.blocksRaycasts = true;
        bounusNoticeCG.alpha = 0;
        bounusNoticeCG.blocksRaycasts = false;
    }
    void OnGetClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (hasGet) return;
        rewardMutiple = 1;
        GetReward();
        hasGet = true;
        PanelManager.Instance.ClosePanel(PanelType.Reward);
    }
    float GetReward()
    {
        int result = (int)(rewardNum * rewardMutiple);
        switch (rewardType)
        {
            case RewardType.Gold:
                GameManager.Instance.AddGold(result);
                break;
            case RewardType.Cash:
                GameManager.Instance.AddCash(result);
                break;
            case RewardType.ExtraBonusGold:
                GameManager.Instance.AddGold(result);
                break;
            case RewardType.ExtraBonusCash:
                GameManager.Instance.AddCash(result);
                break;
            case RewardType.SigninGold:
                GameManager.Instance.AddGold(result);
                break;
            case RewardType.SigninCash:
                GameManager.Instance.AddCash(result);
                break;
            default:
                break;
        }
        return result;
    }

    readonly float[] mutiples = new float[5] { 1, 1.5f, 2, 3, 5 };
    readonly float[] posX = new float[5] { -535.1305f, -267.3916f, 0.3474121f, 268.0863f, 535.8253f };
    IEnumerator StartSliding()
    {
        AudioSource spinAS = AudioManager.Instance.PlayerSoundLoop("Spin");
        bool mutipleConfigCorrect = false;
        int index = 0;
        for (int i = 0; i < mutiples.Length; i++)
        {
            if (rewardMutiple == mutiples[i])
            {
                mutipleConfigCorrect = true;
                index = i;
                break;
            }
        }
        if (!mutipleConfigCorrect)
        {
            Debug.LogError("倍率配置错误 : " + rewardMutiple);
            yield break;
        }
        int handleIndex = 0;
        int maxHandleIndex = posX.Length - 1;
        int turns = 2;
        WaitForSeconds interval = new WaitForSeconds(0.2f);
        Coroutine lighting = StartCoroutine(StartShiningLight());
        while (true)
        {
            yield return interval;
            trans_handle.localPosition = new Vector3(posX[handleIndex], 0);
            handleIndex++;
            if (handleIndex > maxHandleIndex)
            {
                turns--;
                handleIndex = 0;
            }
            if (turns < 0 && handleIndex == index)
            {
                break;
            }
        }
        spinAS.Stop();
        StopCoroutine(lighting);
        spinAS = null;
        yield return new WaitForSeconds(1);
        GetReward();
        PanelManager.Instance.ClosePanel(PanelType.Reward);
    }
    IEnumerator DelayShowNothanks(Text needFadeText,float endAlpha=1)
    {
        yield return new WaitForSeconds(1);
        float alpha = 0;
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime;
            if (alpha >= endAlpha)
            {
                needFadeText.color = new Color(1, 1, 1, endAlpha);
                needFadeText.raycastTarget = true;
                yield break;
            }
            needFadeText.color = new Color(1, 1, 1, alpha);
        }
    }
    IEnumerator DelayShowCloseBtn()
    {
        yield return new WaitForSeconds(1);
        btn_close.gameObject.SetActive(true);
    }
    IEnumerator RotateLight()
    {
        float rotateSpeed = 25;
        while (true)
        {
            yield return null;
            trans_bgLight.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
            trans_giftLight.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }
    IEnumerator StartShiningLight()
    {
        Sprite lightA = Atlas.GetSprite(Atlas_Light_A_Key);
        Sprite lightB = Atlas.GetSprite(Atlas_Light_B_Key);
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        bool isLightA = false;
        while (true)
        {
            yield return wait;
            img_Light.sprite = isLightA ? lightA : lightB;
            isLightA = !isLightA;
        }
    }
    protected override void Close()
    {
        base.Close();
        OnNothanksClick();
    }
    public override void OnExit()
    {
        base.OnExit();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        if (GameManager.Instance.canGetExtraBonus)
        {
            GameManager.Instance.GetExtraBonus();
            GameManager.Instance.canGetExtraBonus = false;
        }
        else if (GameManager.Instance.needRateUs)
        {
            PanelManager.Instance.ShowPanel(PanelType.RateUs);
        }
        else
            GameManager.Instance.canRollDice = true;
    }
}
