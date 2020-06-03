using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Reward : PanelBase
{
    public CanvasGroup rewardCG;
    public CanvasGroup bounusNoticeCG;
    private SpriteAtlas rewardAtlas;
    public GameObject slider;
    public Transform slidervalue1;
    public Transform slidervalue2;
    public Image img_title;
    public Image img_mid;
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
    private const float twoSliderInterval = 1046;
    protected override void Awake()
    {
        base.Awake();
        slidervalue1.localPosition = Vector3.zero;
        slidervalue2.localPosition = slidervalue1.localPosition + Vector3.right * twoSliderInterval;
        btn_adGet.onClick.AddListener(OnAdGetClick);
        btn_adOpen.onClick.AddListener(OnOpenClick);
        btn_get.onClick.AddListener(OnGetClick);

        text_nothanks1.GetComponent<Button>().onClick.AddListener(OnNothanksClick);
        text_nothanks2.GetComponent<Button>().onClick.AddListener(Close);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (rewardAtlas is null)
            rewardAtlas = Resources.Load<SpriteAtlas>("RewardPanel");
        rewardType = GameManager.Instance.nextRewardType;
        rewardNum = GameManager.Instance.nextRewardNum;
        rewardMutiple = GameManager.Instance.nextRewardMutiple;
        isSliding = false;
        hasGet = false;
        text_nothanks1.color = new Color(1, 1, 1, 0);
        text_nothanks1.raycastTarget = false;
        text_nothanks2.color = new Color(1, 1, 1, 0);
        text_nothanks2.raycastTarget = false;
        bool canShowExchange = GameManager.Instance.GetShowExchange();
        switch (rewardType)
        {
            case RewardType.Gold:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_title.sprite = rewardAtlas.GetSprite("congratulations");
                img_mid.sprite = rewardAtlas.GetSprite(canShowExchange ? "manygoldB" : "manygoldA");
                img_rewardicon.sprite= rewardAtlas.GetSprite("gold");
                text_rewardNum.text = rewardNum.ToString();
                text_nothanks1.text = "No , thanks";
                btn_adGet.gameObject.SetActive(true);
                btn_get.gameObject.SetActive(false);
                slider.SetActive(true);
                StartCoroutine("DelayShowNothanks1");
                break;
            case RewardType.Cash:
                rewardCG.alpha = 1;
                rewardCG.blocksRaycasts = true;
                bounusNoticeCG.alpha = 0;
                bounusNoticeCG.blocksRaycasts = false;
                img_title.sprite = rewardAtlas.GetSprite("congratulations");
                img_mid.sprite = rewardAtlas.GetSprite(canShowExchange ? "manycashB" : "manycashA");
                img_rewardicon.sprite = rewardAtlas.GetSprite(canShowExchange ? "cashB" : "cashA");

                string cashString = rewardNum.ToString();
                if (rewardNum < 10)
                    cashString = cashString.Insert(0, "0.0");
                else if (rewardNum < 100)
                    cashString = cashString.Insert(0, "0.");
                else
                    cashString = cashString.Insert(cashString.Length - 2, ".");
                text_rewardNum.text = cashString;

                text_nothanks1.text = "Give up";
                btn_adGet.gameObject.SetActive(true);
                btn_get.gameObject.SetActive(false);
                slider.SetActive(true);
                StartCoroutine("DelayShowNothanks1");
                break;
            case RewardType.ExtraBonusGold:
                rewardCG.alpha = 0;
                rewardCG.blocksRaycasts = false;
                bounusNoticeCG.alpha = 1;
                bounusNoticeCG.blocksRaycasts = true;
                img_title.sprite = rewardAtlas.GetSprite("extrabonus");
                img_mid.sprite = rewardAtlas.GetSprite(canShowExchange ? "manygoldB" : "manygoldA");
                img_rewardicon.sprite = rewardAtlas.GetSprite("gold");
                text_rewardNum.text = rewardNum.ToString();
                btn_adGet.gameObject.SetActive(false);
                btn_get.gameObject.SetActive(true);
                slider.SetActive(false);
                StartCoroutine("DelayShowNothanks2");
                break;
            case RewardType.ExtraBonusCash:
                rewardCG.alpha = 0;
                rewardCG.blocksRaycasts = false;
                bounusNoticeCG.alpha = 1;
                bounusNoticeCG.blocksRaycasts = true;
                img_title.sprite = rewardAtlas.GetSprite("extrabonus");
                img_mid.sprite = rewardAtlas.GetSprite(canShowExchange ? "manycashB" : "manycashA");
                img_rewardicon.sprite = rewardAtlas.GetSprite(canShowExchange ? "cashB" : "cashA");

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
                StartCoroutine("DelayShowNothanks2");
                break;
            case RewardType.NoticeBonus:
                rewardCG.alpha = 0;
                rewardCG.blocksRaycasts = false;
                bounusNoticeCG.alpha = 1;
                bounusNoticeCG.blocksRaycasts = true;
                break;
            default:
                PanelManager.Instance.ClosePanel(PanelType.Reward);
                break;
        }
    }
    bool isSliding = false;
    bool hasGet = false;
    void OnAdGetClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isSliding) return;
#if UNITY_EDITOR
        OnadGetRewardedCallBack();
        return;
#endif
        if (!GameManager.Instance.GetShowExchange())
        {
            OnadGetRewardedCallBack();
            return;
        }
        Ads._instance.SetRewardedCallBack(OnadGetRewardedCallBack);
        Ads._instance.adDes = rewardType.ToString() + "的倍数获得广告";
        Ads._instance.ShowRewardVideo();
    }
    void OnadGetRewardedCallBack()
    {
        isSliding = true;
        if (!hasGet)
        {
            hasGet = true;
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
#if UNITY_EDITOR
        OnAdopenRewardedCallback();
        return;
#endif
        if (!GameManager.Instance.GetShowExchange())
        {
            OnAdopenRewardedCallback();
            return;
        }
        Ads._instance.SetRewardedCallBack(OnAdopenRewardedCallback);
        Ads._instance.adDes = "惊喜礼盒打开";
        Ads._instance.ShowRewardVideo();
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
            default:
                break;
        }
        return result;
    }

    readonly float[] mutiples = new float[5] { 1, 1.5f, 2, 3, 5 };
    readonly float[] posX = new float[5] { 430, 215, 0, -215, -428 };
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
        float endX = posX[index];
        int turns = 2;
        int slideSpeed = 2000;
        while (true)
        {
            yield return null;
            slidervalue1.localPosition += Vector3.left * Time.deltaTime * slideSpeed;
            slidervalue2.localPosition += Vector3.left * Time.deltaTime * slideSpeed;
            if (slidervalue1.localPosition.x <= -1000)
                slidervalue1.localPosition = slidervalue2.localPosition + Vector3.right * twoSliderInterval;
            if (slidervalue2.localPosition.x <= -1000)
            {
                slidervalue2.localPosition = slidervalue1.localPosition + Vector3.right * twoSliderInterval;
                turns--;
                if (turns == 0)
                {
                    slideSpeed = 1000;
                }
            }
            if (turns <= 0 && (Mathf.Abs(slidervalue2.localPosition.x - endX) <= 10f || Mathf.Abs(slidervalue1.localPosition.x - endX) <= 10f))
                break;
        }
        GetReward();
        spinAS.Stop();
        spinAS = null;
        yield return new WaitForSeconds(0.3f);
        PanelManager.Instance.ClosePanel(PanelType.Reward);
    }
    IEnumerator DelayShowNothanks1()
    {
        yield return new WaitForSeconds(1);
        float alpha = 0;
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime;
            if (alpha >= 0.95f)
            {
                text_nothanks1.color = Color.white;
                text_nothanks1.raycastTarget = true;
                yield break;
            }
            text_nothanks1.color = new Color(1, 1, 1, alpha);
        }
    }
    IEnumerator DelayShowNothanks2()
    {
        yield return new WaitForSeconds(1);
        float alpha = 0;
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime;
            if (alpha >= 0.95f)
            {
                text_nothanks2.color = Color.white;
                text_nothanks2.raycastTarget = true;
                yield break;
            }
            text_nothanks2.color = new Color(1, 1, 1, alpha);
        }
    }
    protected override void Close()
    {
        base.Close();
        PanelManager.Instance.ClosePanel(PanelType.Reward);
    }
    public override void OnExit()
    {
        base.OnExit();
        StopCoroutine("DelayShowNothanks");
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        if (GameManager.Instance.canGetExtraBonus)
        {
            GameManager.Instance.GetExtraBonus();
            GameManager.Instance.canGetExtraBonus = false;
        }
        else
            GameManager.Instance.canRollDice = true;
    }
}
