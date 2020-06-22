using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Signin : PanelBase
{
    public DailyReward[] dailyRewards = new DailyReward[7];
    int[] rewards = new int[7] { 2000, 100, 2000, 2000, 100, 100, 100 };
    bool[] isGold = new bool[7] { true, false, true, true, false, false, false };
    float[] rewardmutiples = new float[7] { 3, 1.5f, 1.5f, 5, 1.5f, 1.5f, 5 };
    public Transform handle;
    public Button btn_get;
    public Button btn_nothanks;
    public Image img_Light;
    public Image img_midMutiple;
    Sprite hasgetBg = null;
    Sprite ungetBg = null;
    Sprite todayBg = null;
    Sprite goldGetIcon = null;
    Sprite goldUngetIcon = null;
    Sprite cashGetIcon = null;
    Sprite cashUngetIcon = null;
    Sprite lightA = null;
    Sprite lightB = null;
    Sprite mutipleUnknown = null;
    Sprite mutipleTwo = null;
    SpriteAtlas signinAltas;
    SpriteAtlas rewardAltas;
    protected override void Awake()
    {
        base.Awake();
        signinAltas = Resources.Load<SpriteAtlas>("SigninPanel");
        rewardAltas = Resources.Load<SpriteAtlas>("RewardPanel");
        lightA = rewardAltas.GetSprite("LightA");
        lightB = rewardAltas.GetSprite("LightB");
        mutipleUnknown = rewardAltas.GetSprite("Mutiple_Unknown");
        mutipleTwo = rewardAltas.GetSprite("Mutiple_2");
        hasgetBg = signinAltas.GetSprite("hasgetbg");
        ungetBg = signinAltas.GetSprite("ungetbg");
        todayBg = signinAltas.GetSprite("todaybg");
        goldGetIcon = signinAltas.GetSprite("GoldGetIcon");
        goldUngetIcon = signinAltas.GetSprite("GoldUngetIcon");
        cashGetIcon = signinAltas.GetSprite("CashGetIcon");
        cashUngetIcon = signinAltas.GetSprite("CashUngetIcon");
        btn_get.onClick.AddListener(OnGetClick);
        btn_nothanks.onClick.AddListener(OnNothanksClick);
    }
    int clicktime = 0;
    bool canSign = false;
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        isRandom = false;
        handle.localPosition = new Vector3(posX[2], 0);
        img_midMutiple.sprite = mutipleUnknown;
        canSign = GameManager.Instance.CheckCanSignin();
        btn_get.GetComponent<Image>().color = canSign ? Color.white : Color.grey;
        int nextSigninDay = GameManager.Instance.GetNextSigninDay();
        int maxDay = dailyRewards.Length;
        for(int i = 0; i < maxDay; i++)
        {
            if (i < nextSigninDay)
            {
                if (isGold[i])
                    dailyRewards[i].SetSignState(true, hasgetBg, goldGetIcon, "Day " + (i + 1), "x" + (rewards[i] * rewardmutiples[i]));
                else
                    dailyRewards[i].SetSignState(true, hasgetBg, cashGetIcon, "Day " + (i + 1), "x" + (rewards[i] * rewardmutiples[i] / 100));
            }
            else if (i == nextSigninDay&&canSign)
            {
                if (isGold[i])
                    dailyRewards[i].SetSignState(false, todayBg, goldUngetIcon, "Day " + (i + 1), "x" + rewards[i]);
                else
                    dailyRewards[i].SetSignState(false, todayBg, cashUngetIcon, "Day " + (i + 1), "x ?");
            }
            else
            {
                if (isGold[i])
                    dailyRewards[i].SetSignState(false, ungetBg, goldUngetIcon, "Day " + (i + 1), "x" + rewards[i]);
                else
                    dailyRewards[i].SetSignState(false, ungetBg, cashUngetIcon, "Day " + (i + 1), "x ?");
            }
        }
        if(canSign)
        StartCoroutine("ShakeTodayReawrd", dailyRewards[nextSigninDay].transform.GetChild(1));
        clicktime = 0;
    }
    public override void OnExit()
    {
        base.OnExit();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        StopCoroutine("ShakeTodayReawrd");
    }
    protected override void Close()
    {
        base.Close();
        AudioManager.Instance.PlayerSound("Button");
        if (isRandom)
            return;
        PanelManager.Instance.ClosePanel(PanelType.Signin);
    }
    bool isRandom = false;
    void OnGetClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isRandom) return;
        if (GameManager.Instance.CheckCanSignin())
        {
#if UNITY_EDITOR
            OnAdRewardedCallback();
            return;
#endif
#if UNITY_IOS
            if (!GameManager.Instance.GetShowExchange())
            {
                OnAdRewardedCallback();
                return;
            }
#endif
            clicktime++;
            Ads._instance.SetRewardedCallBack(OnAdRewardedCallback);
            Ads._instance.adDes = "签到多倍奖励";
            Ads._instance.ShowRewardVideo(clicktime);
        }
        else
        {

        }
    }
    void OnAdRewardedCallback()
    {
        img_midMutiple.sprite = mutipleTwo;
        StartCoroutine(StartRandom());
        isRandom = true;
    }
    void OnNothanksClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isRandom) return;
        PanelManager.Instance.ClosePanel(PanelType.Signin);
        if (GameManager.Instance.CheckCanSignin())
        {
            int nextDay = GameManager.Instance.GetNextSigninDay();
            GameManager.Instance.nextRewardNum = rewards[nextDay];
            GameManager.Instance.nextRewardMutiple = 1;
            GameManager.Instance.nextRewardType = isGold[nextDay] ? RewardType.SigninGold : RewardType.SigninCash;
            PanelManager.Instance.ShowPanel(PanelType.Reward);
            GameManager.Instance.Signin(DateTime.Now);
        }
    }
    readonly float[] posX = new float[5] { -559.9956f, -275.9868f, 8.021973f, 284.0308f, 560.0396f };
    readonly float[] mutiples = new float[5] { 1, 1.5f, 2, 3, 5 };
    IEnumerator StartRandom()
    {
        AudioSource spinAS = AudioManager.Instance.PlayerSoundLoop("Spin");
        int result = 0;
        int nextDay = GameManager.Instance.GetNextSigninDay();
        float nextDayMutiply = rewardmutiples[nextDay];
        for (int i = 0; i < 5; i++)
        {
            if (nextDayMutiply == mutiples[i])
            {
                result = i;
                break;
            }
        }
        int handleIndex = 0;
        int maxHandleIndex = posX.Length - 1;
        int turns = 2;
        bool isLightA = false;
        WaitForSeconds interval = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return interval;
            handle.localPosition = new Vector3(posX[handleIndex], 0);
            handleIndex++;
            img_Light.sprite = isLightA ? lightA : lightB;
            isLightA = !isLightA;
            if (handleIndex > maxHandleIndex)
            {
                turns--;
                handleIndex = 0;
            }
            if (turns < 0 && handleIndex == result)
            {
                break;
            }
        }
        spinAS.Stop();
        GameManager.Instance.nextRewardNum = rewards[nextDay];
        GameManager.Instance.nextRewardMutiple = nextDayMutiply;
        GameManager.Instance.nextRewardType = isGold[nextDay] ? RewardType.SigninGold : RewardType.SigninCash;
        GameManager.Instance.Signin(DateTime.Now);
        isRandom = false;
        PanelManager.Instance.ClosePanel(PanelType.Signin);
        PanelManager.Instance.ShowPanel(PanelType.Reward);
    }
    IEnumerator ShakeTodayReawrd(Transform todayTransform)
    {
        float speed = 200;
        int onetimer = 0;
        int oneturns = 4;
        int alltimer = 0;
        int allturns = 61;
        bool shake = true;
        bool hasComp = false;
        while (true)
        {
            yield return null;
            alltimer++;
            if (alltimer >= allturns)
            {
                alltimer = 0;
                shake = !shake;
                hasComp = false;
                todayTransform.rotation = Quaternion.identity;
            }
            if (!shake) continue;
            onetimer++;
            todayTransform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
            if (!hasComp&& onetimer >= oneturns/2)
            {
                speed = -speed;
                onetimer = 0;
                hasComp = true;
            }
            if (onetimer >= oneturns)
            {
                speed = -speed;
                onetimer = 0;
            }
        }
    }
}
