using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Jackpot : PanelBase
{
    const int Reward_Num = 10;
    public Button btn_Spin;
    Image img_Spin;
    Sprite spinSprite;
    Sprite adSprite;
    bool canSpin = true;
    public Transform[] left_Icons = new Transform[Reward_Num];
    public Transform[] mid_Icons = new Transform[Reward_Num];
    public Transform[] right_Icons = new Transform[Reward_Num];
    private Text text_nothanks;
    SpriteAtlas jackpotAltas;
    protected override void Awake()
    {
        base.Awake();
        if (jackpotAltas == null)
            jackpotAltas = Resources.Load<SpriteAtlas>("JackpotPanel");
        img_Spin = btn_Spin.GetComponent<Image>();
        spinSprite = jackpotAltas.GetSprite("spin");
        adSprite = jackpotAltas.GetSprite("adspin");
        img_Spin.sprite = spinSprite;
        img_Spin.color = Color.white;
        btn_Spin.onClick.AddListener(StartSpin);
        bool canShowExchange = GameManager.Instance.GetShowExchange();
        for(int i = 0; i < Reward_Num; i++)
        {
            string spriteName = i.ToString();
            if (!isGoldArray[i])
                spriteName = canShowExchange ? i + "B" : i + "A";
            Sprite temp = jackpotAltas.GetSprite(spriteName);
            left_Icons[i].GetComponent<Image>().sprite = temp;
            right_Icons[i].GetComponent<Image>().sprite = temp;
            mid_Icons[i].GetComponent<Image>().sprite = temp;
        }
        canSpin = true;
        text_nothanks = btn_close.GetComponent<Text>();
    }
    const float Min_Y = -486;
    const float Interval_Y = 248;
    static readonly Vector3 Interval = new Vector3(0, Interval_Y, 0);
    static readonly Vector3 selectPos = new Vector3(0, 10, 0);
    bool isSpining = false;
    AudioSource spinAS = null;
    RewardType rewardType = RewardType.Null;
    int[] numArray = new int[Reward_Num] { 5, 500, 0, 25, 10000, 50, 1000, 100, 5000, 10 };
    bool[] isGoldArray = new bool[Reward_Num] { false, true, true, false, true, false, true, false, true, false };
    bool needShowAd = false;
    int clickAdTime = 0;
    void StartSpin()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isSpining) return;
        if (needShowAd)
        {
            ResetRewardIconPos();
#if UNITY_EDITOR
            OnRewardedCallback();
            return;
#endif
#if UNITY_IOS
            if (!GameManager.Instance.GetShowExchange())
            {
                OnRewardedCallback();
                return;
            }
#endif
            clickAdTime++;
            Ads._instance.SetRewardedCallBack(OnRewardedCallback);
            Ads._instance.adDes = "老虎机的广告";
            Ads._instance.ShowRewardVideo(clickAdTime);
        }
        else
        {
            OnRewardedCallback();
        }
    }
    void OnRewardedCallback()
    {
        spinAS = AudioManager.Instance.PlayerSoundLoop("Spin");
        isSpining = true;
        btn_Spin.transform.localPosition += Vector3.down * 40;
        float rewardNum = GameManager.Instance.GetJackpotRandom(out rewardType);
        int index = -1;
        if (rewardType != RewardType.Null)
        {
            for (int i = 0; i < Reward_Num; i++)
            {
                if (rewardNum == numArray[i])
                {
                    if (rewardType == RewardType.Gold && isGoldArray[i])
                    {
                        index = i;
                        break;
                    }
                    else if (rewardType == RewardType.Cash && !isGoldArray[i])
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (index == -1)
            {
                Debug.LogError("jackpot 奖励配置错误，类型 : " + rewardType.ToString() + " , 数量 : " + rewardNum);
                return;
            }
        }
        if (rewardType != RewardType.Null)
        {
            rewardIndex_L = rewardIndex_M = rewardIndex_R = index;
        }
        else
        {
            rewardIndex_L = Random.Range(0, Reward_Num);
            rewardIndex_M = Random.Range(0, Reward_Num);
            if (rewardIndex_L == rewardIndex_M)
            {
                int temp;
                while (true)
                {
                    temp = Random.Range(0, Reward_Num);
                    if (temp != rewardIndex_L)
                        break;
                }
                rewardIndex_R = temp;
            }
            else
            {
                rewardIndex_R = Random.Range(0, Reward_Num);
            }
        }
        StartCoroutine(SpinReward());
    }
    enum SpinPlace
    {
        Left,
        Middle,
        Right
    }
#region spin
    int rewardIndex_L = 0;
    int rewardIndex_M = 0;
    int rewardIndex_R = 0;
    int frontIndex_L = 0;
    int frontIndex_M = 0;
    int frontIndex_R = 0;
    IEnumerator SpinReward()
    {
        float time = 0;
        float maxTime_L = 1;
        float maxTime_M = 1.4f;
        float maxTime_R = 1.8f;
        float spinSpeed = 8000;
        Vector3 bottomPos = new Vector3(0, Min_Y, 0);
        bool hasSetRewardPos_L = false;
        bool hasSetRewardPos_M = false;
        bool hasSetRewardPos_R = false;
        while (true)
        {
            yield return null;
            time += Time.deltaTime/2;

            Transform bottom_L_Trans = left_Icons[frontIndex_L];
            Transform bottom_M_Trans = mid_Icons[frontIndex_M];
            Transform bottom_R_Trans = right_Icons[frontIndex_R];
            if (time < maxTime_L)
                bottom_L_Trans.localPosition += Vector3.down * spinSpeed * Time.deltaTime/2;
            else
            {
                if (!hasSetRewardPos_L)
                {
                    hasSetRewardPos_L = true;
                    foreach(Transform temp in left_Icons)
                    {
                        temp.localPosition = bottomPos;
                    }
                    int frontRewardIndex = rewardIndex_L - 1 < 0 ? Reward_Num - 1 + rewardIndex_L : rewardIndex_L - 1;
                    int behindRewardIndex = rewardIndex_L + 1 > Reward_Num - 1 ? rewardIndex_L + 1 - Reward_Num : rewardIndex_L + 1;
                    left_Icons[rewardIndex_L].localPosition = selectPos;
                    left_Icons[frontRewardIndex].localPosition = selectPos - Interval;
                    left_Icons[behindRewardIndex].localPosition = selectPos + Interval;
                }
            }
            if (time < maxTime_M)
                bottom_M_Trans.localPosition += Vector3.down * spinSpeed * Time.deltaTime/2;
            else
            {
                if (!hasSetRewardPos_M)
                {
                    hasSetRewardPos_M = true;
                    foreach (Transform temp in mid_Icons)
                    {
                        temp.localPosition = bottomPos;
                    }
                    int frontRewardIndex = rewardIndex_M - 1 < 0 ? Reward_Num - 1 + rewardIndex_M : rewardIndex_M - 1;
                    int behindRewardIndex = rewardIndex_M + 1 > Reward_Num - 1 ? rewardIndex_M + 1 - Reward_Num : rewardIndex_M + 1;
                    mid_Icons[rewardIndex_M].localPosition = selectPos;
                    mid_Icons[frontRewardIndex].localPosition = selectPos - Interval;
                    mid_Icons[behindRewardIndex].localPosition = selectPos + Interval;
                }
            }
            if (time < maxTime_R)
                bottom_R_Trans.localPosition += Vector3.down * spinSpeed * Time.deltaTime/2;
            else
            {
                if (!hasSetRewardPos_R)
                {
                    hasSetRewardPos_R = true;
                    foreach (Transform temp in right_Icons)
                    {
                        temp.localPosition = bottomPos;
                    }
                    int frontRewardIndex = rewardIndex_R - 1 < 0 ? Reward_Num - 1 + rewardIndex_R : rewardIndex_R - 1;
                    int behindRewardIndex = rewardIndex_R + 1 > Reward_Num - 1 ? rewardIndex_R + 1 - Reward_Num : rewardIndex_R + 1;
                    right_Icons[rewardIndex_R].localPosition = selectPos;
                    right_Icons[frontRewardIndex].localPosition = selectPos - Interval;
                    right_Icons[behindRewardIndex].localPosition = selectPos + Interval;
                }
            }
            float frontY_L = left_Icons[frontIndex_L].localPosition.y;
            float frontY_M = mid_Icons[frontIndex_M].localPosition.y;
            float frontY_R = right_Icons[frontIndex_R].localPosition.y;
            for(int i = 0; i < Reward_Num; i++)
            {
                Transform temp_L_Trans = left_Icons[i];
                Transform temp_M_Trans = mid_Icons[i];
                Transform temp_R_Trans = right_Icons[i];
                float offsetY_L = temp_L_Trans.localPosition.y - frontY_L;
                float offsetY_M = temp_M_Trans.localPosition.y - frontY_M;
                float offsetY_R = temp_R_Trans.localPosition.y - frontY_R;

                if (!hasSetRewardPos_L)
                {
                    if (offsetY_L > 0)
                        temp_L_Trans.localPosition = left_Icons[i - 1 == -1 ? Reward_Num - 1 : i - 1].localPosition + Interval;
                    else if (offsetY_L < 0)
                        temp_L_Trans.localPosition = left_Icons[i + 1 == Reward_Num ? 0 : i + 1].localPosition - Interval;
                }

                if (!hasSetRewardPos_M)
                {
                    if (offsetY_M > 0)
                        temp_M_Trans.localPosition = mid_Icons[i - 1 == -1 ? Reward_Num - 1 : i - 1].localPosition + Interval;
                    else if (offsetY_M < 0)
                        temp_M_Trans.localPosition = mid_Icons[i + 1 == Reward_Num ? 0 : i + 1].localPosition - Interval;
                }

                if (!hasSetRewardPos_R)
                {
                    if (offsetY_R > 0)
                        temp_R_Trans.localPosition = right_Icons[i - 1 == -1 ? Reward_Num - 1 : i - 1].localPosition + Interval;
                    else if (offsetY_R < 0)
                        temp_R_Trans.localPosition = right_Icons[i + 1 == Reward_Num ? 0 : i + 1].localPosition - Interval;
                }
            }


            if (frontY_L <= Min_Y&&!hasSetRewardPos_L)
            {
                if (frontIndex_L > 0)
                    bottom_L_Trans.localPosition = left_Icons[frontIndex_L - 1].localPosition + Interval;
                else
                    bottom_L_Trans.localPosition = left_Icons[Reward_Num - 1].localPosition + Interval;
                frontIndex_L++;
                if (frontIndex_L > Reward_Num - 1)
                    frontIndex_L = 0;
            }

            if (frontY_M <= Min_Y&&!hasSetRewardPos_M)
            {
                if (frontIndex_M > 0)
                    bottom_M_Trans.localPosition = mid_Icons[frontIndex_M - 1].localPosition + Interval;
                else
                    bottom_M_Trans.localPosition = mid_Icons[Reward_Num - 1].localPosition + Interval;
                frontIndex_M++;
                if (frontIndex_M > Reward_Num - 1)
                    frontIndex_M = 0;
            }

            if (frontY_R <= Min_Y&&!hasSetRewardPos_R)
            {
                if (frontIndex_R > 0)
                    bottom_R_Trans.localPosition = right_Icons[frontIndex_R - 1].localPosition + Interval;
                else
                    bottom_R_Trans.localPosition = right_Icons[Reward_Num - 1].localPosition + Interval;
                frontIndex_R++;
                if (frontIndex_R > Reward_Num - 1)
                    frontIndex_R = 0;
            }

            if (hasSetRewardPos_L && hasSetRewardPos_M && hasSetRewardPos_R)
            {
                break;
            }
        }
        spinAS.Stop();
        spinAS = null;
        yield return new WaitForSeconds(2f);

        if (rewardIndex_L == rewardIndex_M && rewardIndex_L == rewardIndex_R)
        {
            img_Spin.color = Color.grey;
            canSpin = false;
            PanelManager.Instance.ClosePanel(PanelType.Jackpot);
            //GetReward
            PanelManager.Instance.ShowPanel(PanelType.Reward);
        }
        else
        {
            img_Spin.sprite = adSprite;
            needShowAd = true;
            StartCoroutine("DelayShowNothanks");
        }
        btn_Spin.transform.localPosition -= Vector3.down * 40;
        isSpining = false;
    }
    void ResetRewardIconPos()
    {
        frontIndex_L = 0;
        frontIndex_M = 0;
        frontIndex_R = 0;
        for (int i = 0; i < Reward_Num; i++)
        {
            left_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
            right_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
            mid_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
        }
    }
#endregion
    protected override void Close()
    {
        base.Close();
        if (isSpining) return;
        Ads._instance.adDes = "老虎机的nothanks";
        Ads._instance.ShowInterstialAd();
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ClosePanel(PanelType.Jackpot);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        clickAdTime = 0;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        text_nothanks.color = new Color(1, 1, 1, 0);
        text_nothanks.raycastTarget = false;
        canSpin = true;
        needShowAd = false;
        img_Spin.sprite = spinSprite;
        img_Spin.color = Color.white;
        ResetRewardIconPos();
    }
    public override void OnExit()
    {
        base.OnExit();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        if (GameManager.Instance.canGetExtraBonus && canSpin == true)
        {
            GameManager.Instance.GetExtraBonus();
            GameManager.Instance.canGetExtraBonus = false;
        }
        else
            GameManager.Instance.canRollDice = true;
    }
    IEnumerator DelayShowNothanks()
    {
        yield return new WaitForSeconds(2);
        float alpha = 0;
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime/2;
            if (alpha >= 0.95f)
            {
                text_nothanks.color = Color.white;
                text_nothanks.raycastTarget = true;
                yield break;
            }
            text_nothanks.color = new Color(1, 1, 1, alpha);
        }
    }
}
