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
        for(int i = 0; i < Reward_Num; i++)
        {
            Sprite temp = jackpotAltas.GetSprite(i.ToString());
            left_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
            left_Icons[i].GetComponent<Image>().sprite = temp;
            right_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
            right_Icons[i].GetComponent<Image>().sprite = temp;
            mid_Icons[i].localPosition = new Vector3(0, Min_Y + i * Interval_Y, 0);
            mid_Icons[i].GetComponent<Image>().sprite = temp;
        }
        canSpin = true;
        text_nothanks = btn_close.GetComponent<Text>();
    }
    const float Min_Y = -486;
    const float Interval_Y = 248;
    static readonly Vector3 Interval = new Vector3(0, Interval_Y, 0);
    static readonly Vector3 selectPos = new Vector3(0, 10, 0);
    private void Update()
    {
        Spin( firsttime, SpinPlace.Left, ref startSpinL, ref needFixPos_L, ref canSelected_L, ref spinSpeed_L, ref hasTime_L, ref frontIndex_L);
        Spin( secondtime, SpinPlace.Middle, ref startSpinM, ref needFixPos_M, ref canSelected_M, ref spinSpeed_M, ref hasTime_M, ref frontIndex_M);
        Spin( thirdtime, SpinPlace.Right, ref startSpinR, ref needFixPos_R, ref canSelected_R, ref spinSpeed_R, ref hasTime_R, ref frontIndex_R);
    }
    bool isSpining = false;
    AudioSource spinAS = null;
    RewardType rewardType = RewardType.Null;
    int[] numArray = new int[Reward_Num] { 5, 500, 25, 10000, 50, 1000, 100, 5000, 10, 0 };
    bool[] isGoldArray = new bool[Reward_Num] { false, true, false, true, false, true, false, true, false, false };
    bool needShowAd = false;
    void StartSpin()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isSpining) return;
        if (needShowAd)
        {
#if UNITY_EDITOR
            OnRewardedCallback();
            return;
#endif
            Ads._instance.SetRewardedCallBack(OnRewardedCallback);
            Ads._instance.adDes = "老虎机的广告";
            Ads._instance.ShowRewardVideo();
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
        startSpinL = true;
        startSpinM = true;
        startSpinR = true;
        canSelected_L = false;
        canSelected_M = false;
        canSelected_R = false;

        needFixPos_L = false;
        needFixPos_M = false;
        needFixPos_R = false;
        hasTime_L = 0;
        hasTime_M = 0;
        hasTime_R = 0;
        spinSpeed_L = 3000;
        spinSpeed_M = 3000;
        spinSpeed_R = 3000;
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
    int firsttime = 2;
    int secondtime = 3;
    const int thirdtime = 4;
    bool startSpinL = false;
    bool startSpinM = false;
    bool startSpinR = false;
    bool needFixPos_L = false;
    bool needFixPos_M = false;
    bool needFixPos_R = false;
    bool canSelected_L = false;
    bool canSelected_M = false;
    bool canSelected_R = false;
    float spinSpeed_L = 0;
    float spinSpeed_M = 0;
    float spinSpeed_R = 0;
    int hasTime_L = 0;
    int hasTime_M = 0;
    int hasTime_R = 0;
    int frontIndex_L = 0;
    int frontIndex_M = 0;
    int frontIndex_R = 0;
    void Spin(int spinTime, SpinPlace spinPlace, ref bool startSpin, ref bool needFixPos,ref bool canSelected,ref float spinSpeed,ref int hasTime,ref int frontIndex)
    {
        Transform[] rewardList;
        int rewardIndex;
        switch (spinPlace)
        {
            case SpinPlace.Left:
                rewardList = left_Icons;
                rewardIndex = rewardIndex_L;
                break;
            case SpinPlace.Middle:
                rewardList = mid_Icons;
                rewardIndex = rewardIndex_M;
                break;
            case SpinPlace.Right:
                rewardList = right_Icons;
                rewardIndex = rewardIndex_R;
                break;
            default:
                Debug.LogError("spinPlace Error");
                return;
        }

        if (needFixPos)
        {
            Transform needTrans = rewardList[rewardIndex];
            needTrans.localPosition = selectPos;
            float selectY = selectPos.y;
            for (int i = 0; i < Reward_Num; i++)
            {
                Transform temp = rewardList[i];
                float offsetY = temp.localPosition.y - selectY;
                if (offsetY > 0)
                {
                    rewardList[i].localPosition = rewardList[i - 1 == -1 ? Reward_Num - 1 : i - 1].localPosition + Interval;
                }
                else if (offsetY < 0)
                {
                    rewardList[i].localPosition = rewardList[i + 1 == Reward_Num ? 0 : i + 1].localPosition - Interval;
                }
            }
            needFixPos = false;
            //结束  获得奖励
            if (spinPlace == SpinPlace.Right)
            {
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
                spinAS.Stop();
                spinAS = null;
                isSpining = false;
            }
        }

        if (!startSpin) return;
        Vector3 delta = Vector3.down * spinSpeed * Time.deltaTime;
        rewardList[frontIndex].localPosition += delta;
        float frontY = rewardList[frontIndex].localPosition.y;
        for (int i = 0; i < Reward_Num; i++)
        {
            Transform temp = rewardList[i];
            float offsetY = temp.localPosition.y - frontY;
            if (offsetY > 0)
            {
                rewardList[i].localPosition = rewardList[i - 1 == -1 ? Reward_Num - 1 : i - 1].localPosition + Interval;
            }
            else if (offsetY < 0)
            {
                rewardList[i].localPosition = rewardList[i + 1 == Reward_Num ? 0 : i + 1].localPosition - Interval;
            }
            if (canSelected && i == rewardIndex)
            {
                float distance = Vector3.Distance(temp.localPosition, selectPos);
                if (distance <= 15)
                {
                    startSpin = false;
                    needFixPos = true;
                }
            }
        }
        if (frontY <= Min_Y)
        {
            if (frontIndex > 0)
                rewardList[frontIndex].localPosition = rewardList[frontIndex - 1].localPosition + Interval;
            else
            {
                rewardList[frontIndex].localPosition = rewardList[Reward_Num - 1].localPosition + Interval;
            }
            if (frontIndex == rewardIndex)
            {
                hasTime++;
                if (hasTime >= spinTime)
                {
                    spinSpeed = 1200;
                    canSelected = true;
                }
            }
            frontIndex++;
            if (frontIndex > Reward_Num - 1)
                frontIndex = 0;
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
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        text_nothanks.color = new Color(1, 1, 1, 0);
        text_nothanks.raycastTarget = false;
        canSpin = true;
        needShowAd = false;
        img_Spin.sprite = spinSprite;
        img_Spin.color = Color.white;
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
        yield return new WaitForSeconds(1);
        float alpha = 0;
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime;
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
