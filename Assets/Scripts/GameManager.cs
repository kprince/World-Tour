using MiddleGround;
using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    Null = 0,
    Jackpot = 1,
    Cash = 2,
    Gold = 3,
    ExtraBonusGold = 4,
    ExtraBonusCash = 5,
    NoticeBonus = 6,
    SigninGold = 7,
    SigninCash = 8,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private SaveManager save;
    private PanelManager panelManager;
    private new AudioManager audio;
    public string notice = string.Empty;
    public Transform car_Trans;
    public ParticleSystem car_exhaustPS_L;
    public ParticleSystem car_exhaustPS_R;
    private readonly static Vector4[] car_shadows = new Vector4[maxSceneIndex + 1]
    {
        new Vector4(0.29f,0.68f,-0.46f,-0.4f),
        new Vector4(0.22f,0.53f,-0.46f,-0.4f),
        new Vector4(1.63f,2.41f,-0.34f,-0.4f),
        new Vector4(1.24f,2.38f,-1.19f,-0.4f),
    };
    public Transform dice_Trans;
    private Rigidbody dice_Rig;
    private Animator car_Animator;
    private Material car_material;
    static readonly Vector3[] bricks_Pos = new Vector3[28]
    {
        new Vector3(-9.79f,0,-5.67f),//0
        new Vector3(-11.9f,0,-7.96f),//1
        new Vector3(-14.17f,0,-10.13f),//2
        new Vector3(-16.52f,0,-12.37f),//3
        new Vector3(-18.87f,0,-13.62f),//4 左中+z
        new Vector3(-21.26f,0,-12.48f),//5
        new Vector3(-23.48f,0,-10.23f),//6
        new Vector3(-25.77f,0,-7.98f),//7
        new Vector3(-27.92f,0,-5.8f),//8
        new Vector3(-30.23f,0,-3.52f),//9
        new Vector3(-32.43f,0,-1.32f),//10
        new Vector3(-33.62f,0,1.129f),//11 中上+x
        new Vector3(-32.48f,0,3.503f),//12
        new Vector3(-30.201f,0,5.715f),//13
        new Vector3(-27.903f,0,8.003f),//14
        new Vector3(-25.7f,0,10.215f),//15
        new Vector3(-23.421f,0,12.522f),//16
        new Vector3(-21.209f,0,14.715f),//17
        new Vector3(-18.825f,0,16f),//18右中 -z
        new Vector3(-16.403f,0,14.791f),//19
        new Vector3(-14.134f,0,12.531f),//20
        new Vector3(-11.855f,0,10.252f),//21
        new Vector3(-9.728f,0,8.077f),//22
        new Vector3(-7.468f,0,5.855f),//23
        new Vector3(-5.208f,0,3.605f),//24
        new Vector3(-4.04f,0,1.174f),//25中下-x
        new Vector3(-5.265f,0,-1.168f),//26
        new Vector3(-7.527f,0,-3.43f)//27
    };
    static readonly Quaternion[] car_step_rotation = new Quaternion[28]
    {
        new Quaternion(0,-0.9f,0,0.4f ),//左下
        new Quaternion(0,-0.9f,0,0.4f ),//左下
        new Quaternion(0,-0.9f,0,0.4f ),//左下
        new Quaternion(0,-0.9f,0,0.4f ),//左下
        new Quaternion(0,-0.7f,0,0.7f) ,//左中
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,-0.4f,0,0.9f) ,//左上
        new Quaternion(0,0,0,1) ,         //中上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.4f,0,0.9f) ,//右上
        new Quaternion(0,0.7f,0,0.7f) ,//右中
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-0.9f,0,-0.4f) ,//右下
        new Quaternion(0,-1,0,0) ,         //中下
        new Quaternion(0,-0.9f,0,0.4f ) ,//左下
        new Quaternion(0,-0.9f,0,0.4f ) ,//左下
    };
    private readonly Dictionary<int, int> brick_reward_Dic = new Dictionary<int, int>();
    private readonly Dictionary<int, GameObject> brick_rewardGo_Dic = new Dictionary<int, GameObject>();
    public Transform specialBrickParent;
    private int[] brick_reward;
    private bool[] brick_reward_get;
    private GameObject prefab_gold;
    private readonly GameObject[] go_golds = new GameObject[8];//5-8
    private int goldgoIndex = 0;
    private GameObject prefab_cash;
    private readonly GameObject[] go_cashs = new GameObject[2];//1-2
    private int cashgoIndex = 0;
    private GameObject prefab_jackpot;
    private readonly GameObject[] go_jackpots = new GameObject[2];//1-2
    private int jackpotsgoIndex = 0;
    [HideInInspector]
    public bool canGetExtraBonus = false;
    [HideInInspector]
    public RewardType nextRewardType = RewardType.Null;
    [HideInInspector]
    public int nextRewardNum = 0;
    [HideInInspector]
    public float nextRewardMutiple = 0;
    [HideInInspector]
    public bool coinCollectIsGold = false;
    [HideInInspector]
    public Transform goldTrans;
    [HideInInspector]
    public Transform cashTrans;
    public GameObject coinCollect;
    [HideInInspector]
    public bool loadEnd = false;
    [HideInInspector]
    public bool needRateUs = false;
    private void Awake()
    {
        Instance = this;
        Time.timeScale =6;
        panelManager = GetComponent<PanelManager>();
        config = Resources.Load<Config>("Config");
        save = GetComponent<SaveManager>();
        save.Init();
        save.player.gameTimes++;
        panelManager.ShowPanel(PanelType.Loading);
        Application.targetFrameRate = 60;
        audio = GetComponent<AudioManager>();
        dice_Rig = dice_Trans.GetComponent<Rigidbody>();
        car_Animator = car_Trans.GetComponent<Animator>();
        car_material = car_Trans.GetComponentInChildren<MeshRenderer>().material;
        audio.Init(save.player.musicOn);
        panelManager.PreLoadPanel(PanelType.Game);
    }
    private void Start()
    {
        car_Trans.localScale = player_OriginScale;
        currentStep = save.player.step;
        car_Trans.position = bricks_Pos[currentStep];
        car_Trans.rotation = car_step_rotation[currentStep];
        dice_Trans.gameObject.SetActive(false);
        brick_reward = save.player.brickReward;
        brick_reward_get = save.player.brickRewardGet;
        car_Animator.SetFloat("Speed", IdleAnimationSpeed);
        prefab_gold = Resources.Load<GameObject>("SpecialBrick/GoldBrick");
        prefab_cash = Resources.Load<GameObject>("SpecialBrick/CashBrick");
        prefab_jackpot = Resources.Load<GameObject>("SpecialBrick/JackpotBrick");

        for (int i = 0; i < go_golds.Length; i++)
        {
            go_golds[i] = Instantiate(prefab_gold, specialBrickParent);
            go_golds[i].SetActive(false);
        }



        for(int i = 0; i < go_cashs.Length; i++)
        {
            go_cashs[i] = Instantiate(prefab_cash, specialBrickParent);
            go_cashs[i].SetActive(false);
        }
        for(int i = 0; i < go_jackpots.Length; i++)
        {
            go_jackpots[i] = Instantiate(prefab_jackpot, specialBrickParent);
            go_jackpots[i].SetActive(false);
        }

        bool needRandom = true;
        for (int i = 1; i < brick_reward.Length; i++)
        {
            if (brick_reward[i] != 0)
            {
                needRandom = false;
                break;
            }
        }
        if (needRandom)
        {
            RandomReward();
        }
        else
            for (int i = 1; i < brick_reward.Length; i++)
            {
                int rewardType = brick_reward[i];
                switch (rewardType)
                {
                    case 1:
                        if (jackpotsgoIndex >= go_jackpots.Length)
                        {
                            Debug.LogError("jackpots is more than max");
                            break;
                        }
                        if (brick_reward_get[i])
                            break;
                        GameObject tempJackpot = go_jackpots[jackpotsgoIndex];
                        tempJackpot.transform.position = bricks_Pos[i];
                        tempJackpot.SetActive(true);
                        brick_reward_Dic.Add(i, 1);
                        brick_rewardGo_Dic.Add(i, tempJackpot);
                        jackpotsgoIndex++;
                        break;
                    case 2:
                        if (cashgoIndex >= go_cashs.Length)
                        {
                            Debug.LogError("cashs is more than max");
                            break;
                        }
                        if (brick_reward_get[i])
                            break;
                        GameObject tempCash = go_cashs[cashgoIndex];
                        tempCash.transform.position = bricks_Pos[i];
                        tempCash.SetActive(true);
                        brick_reward_Dic.Add(i, 2);
                        brick_rewardGo_Dic.Add(i, tempCash);
                        cashgoIndex++;
                        break;
                    case 3:
                        if (goldgoIndex >= go_golds.Length)
                        {
                            Debug.LogError("golds is more than max");
                            break;
                        }
                        if (brick_reward_get[i])
                            break;
                        GameObject tempGold = go_golds[goldgoIndex];
                        tempGold.transform.position = bricks_Pos[i];
                        tempGold.SetActive(true);
                        brick_reward_Dic.Add(i, 3);
                        brick_rewardGo_Dic.Add(i, tempGold);
                        goldgoIndex++;
                        break;
                    default:
                        break;
                }
            }

        if (save.player.sceneIndex > maxSceneIndex)
            save.player.sceneIndex = 0;
        img_SceneBg.sprite = Resources.Load<Sprite>("Scenes/Scene" + save.player.sceneIndex);
        img_SceneMid.sprite = Resources.Load<Sprite>("Scenes/SceneMid" + save.player.sceneIndex);
        img_SceneTop.sprite = Resources.Load<Sprite>("Scenes/SceneTop" + save.player.sceneIndex);
        car_material.SetVector("_LightDir", car_shadows[save.player.sceneIndex]);
    }
    public void SetCashBrickTex()
    {
        Texture2D cashTex = Resources.Load<Texture2D>("SpecialBrick/" + (GetShowExchange() ? "coinB" : "coinA"));

        for (int i = 0; i < go_golds.Length; i++)
        {
            go_golds[i].GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", cashTex);
        }
        for (int i = 0; i < go_cashs.Length; i++)
        {
            go_cashs[i].GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", cashTex);
        }
    }
    static readonly Vector3 player_OriginScale = new Vector3(1.48f, 1.48f, 1.48f);
    void RandomReward()
    {
        brick_reward_Dic.Clear();
        brick_rewardGo_Dic.Clear();
        for(int i = 0; i < brick_reward_get.Length; i++)
        {
            brick_reward_get[i] = false;
        }
        for (int i = 0; i < go_golds.Length; i++)
        {
            go_golds[i].SetActive(false);
        }
        for (int i = 0; i < go_cashs.Length; i++)
        {
            go_cashs[i].SetActive(false);
        }
        for (int i = 0; i < go_jackpots.Length; i++)
        {
            go_jackpots[i].SetActive(false);
        }
        goldgoIndex = 0;
        cashgoIndex = 0;
        jackpotsgoIndex = 0;
        int goldNum = Random.Range(5, 9);
        int cashNum = Random.Range(1, 3);
        int jackpotNum = Random.Range(1, 3);
        int totalBrick = bricks_Pos.Length;

        for (int i = 0; i < brick_reward.Length; i++)
            brick_reward[i] = (int)RewardType.Null;

        for (int i = 0; i < goldNum; i++)
        {
            int rewardPos;
            while (true)
            {
                rewardPos = Random.Range(1, totalBrick);
                if (!brick_reward_Dic.ContainsKey(rewardPos))
                {
                    GameObject goldGo = go_golds[goldgoIndex];
                    brick_reward_Dic.Add(rewardPos, (int)RewardType.Gold);
                    brick_rewardGo_Dic.Add(rewardPos, goldGo);
                    brick_reward[rewardPos] = (int)RewardType.Gold;
                    //实例化
                    goldGo.transform.position = bricks_Pos[rewardPos];
                    goldGo.SetActive(true);
                    goldgoIndex++;
                    break;
                }
            }
        }

        for(int i = 0; i < cashNum; i++)
        {
            int rewardPos;
            while (true)
            {
                rewardPos = Random.Range(1, totalBrick);
                if (!brick_reward_Dic.ContainsKey(rewardPos))
                {
                    GameObject cashGo = go_cashs[cashgoIndex];
                    brick_reward_Dic.Add(rewardPos, (int)RewardType.Cash);
                    brick_rewardGo_Dic.Add(rewardPos, cashGo);
                    brick_reward[rewardPos] = (int)RewardType.Cash;
                    //实例化
                    cashGo.transform.position = bricks_Pos[rewardPos];
                    cashGo.SetActive(true);
                    cashgoIndex++;
                    break;
                }
            }
        }

        for (int i = 0; i < jackpotNum; i++)
        {
            int rewardPos;
            while (true)
            {
                rewardPos = Random.Range(1, totalBrick);
                if (!brick_reward_Dic.ContainsKey(rewardPos))
                {
                    GameObject jackpotGo = go_jackpots[jackpotsgoIndex];
                    brick_reward_Dic.Add(rewardPos, (int)RewardType.Jackpot);
                    brick_rewardGo_Dic.Add(rewardPos, jackpotGo);
                    brick_reward[rewardPos] = (int)RewardType.Jackpot;
                    //实例化
                    jackpotGo.transform.position = bricks_Pos[rewardPos];
                    jackpotGo.SetActive(true);
                    jackpotsgoIndex++;
                    break;
                }
            }
        }
    }
    int currentStep = 0;
    const float IdleAnimationSpeed = 1;
    const float MoveAnimationSpeed = 1.5f;
    const float OneStepNeedStep = 0.25f;
    IEnumerator MoveByStep(int step)
    {
        car_Animator.SetFloat("Speed", MoveAnimationSpeed);
        float timer;
        Vector3 startPos;
        Vector3 endPos;
        Quaternion startRot;
        Quaternion endRot;

        for(int i = 0; i < step; i++)
        {
            timer = 0;
            if (currentStep > bricks_Pos.Length - 1)
                currentStep = 0;
            int nextStep = currentStep + 1;
            if (nextStep > bricks_Pos.Length - 1)
                nextStep = 0;
            startPos = bricks_Pos[currentStep];
            startRot = car_step_rotation[currentStep];
            endPos = bricks_Pos[nextStep];
            endRot = car_step_rotation[nextStep];
            while (timer<OneStepNeedStep)
            {
                yield return null;
                timer += Time.deltaTime/ Time.timeScale;
                float progress = timer / OneStepNeedStep;
                car_Trans.position = Vector3.Lerp(startPos, endPos, progress);
                car_Trans.rotation = Quaternion.Lerp(startRot, endRot, progress);
            }
            car_Trans.position = endPos;
            car_Trans.rotation = endRot;
            currentStep = nextStep;
        }
        if (currentStep ==0)
        {
            GetExtraBonus();
            RandomReward();
            currentStep = 0;
            ChangeSceneBg();
            if (!save.player.hasRateUs && save.player.showExchange)
            {
                needRateUs = true;
                save.player.hasRateUs = true;
            }
        }
        yield return new WaitForSeconds(0.3f * Time.timeScale);
        //获得奖励
        int rewardType = brick_reward[currentStep];
        if (brick_rewardGo_Dic.ContainsKey(currentStep))
        {
            brick_rewardGo_Dic[currentStep].SetActive(false);
            brick_reward_get[currentStep] = true;
        }
        switch (rewardType)
        {
            case 0:
                if (canGetExtraBonus && currentStep != 0)
                {
                    GetExtraBonus();
                    canGetExtraBonus = false;
                }
                else
                    canRollDice = true;
                break;
            case 1:
                panelManager.ShowPanel(PanelType.Jackpot, 0.3f);
                break;
            case 2:
                GetSpecialPropsRandom(false);
                panelManager.ShowPanel(PanelType.Reward, 0.3f);
                break;
            case 3:
                GetSpecialPropsRandom(true);
                panelManager.ShowPanel(PanelType.Reward, 0.3f);
                break;
            default:
                break;
        }
        car_Animator.SetFloat("Speed", IdleAnimationSpeed);
    }
    readonly List<int> nullList = new List<int>();
    readonly List<int> jackpotList = new List<int>();
    readonly List<int> cashList = new List<int>();
    readonly List<int> goldList = new List<int>();
    int RandomStep()
    {
        int step = 0;
        nullList.Clear();
        jackpotList.Clear();
        cashList.Clear();
        goldList.Clear();
        bool hasNull = false;
        bool hasGold = false;
        bool hasCash = false;
        bool hasJackpot = false;
        int maxWeight = 0;
        int minNull = -1;
        int maxNull = -1;
        int minJackpot = -1;
        int maxJackpot = -1;
        int minCash = -1;
        int maxCash = -1;
        int minGold = -1;
        int maxGold = -1;
        if (currentStep < bricks_Pos.Length - 6)
        {
            for (int i = currentStep + 1; i <= currentStep + 6; i++)
            {
                int rewardType = brick_reward[i];
                if (rewardType == 0)
                {
                    nullList.Add(i);
                    hasNull = true;
                }
                else if (rewardType == 1)
                {
                    jackpotList.Add(i);
                    hasJackpot = true;
                }
                else if (rewardType == 2)
                {
                    cashList.Add(i);
                    hasCash = true;
                }
                else if (rewardType == 3)
                {
                    goldList.Add(i);
                    hasGold = true;
                }
            }
        }
        else
        {
            nullList.Add(0);
            hasNull = true;
            for(int i = currentStep + 1; i < bricks_Pos.Length; i++)
            {
                int rewardType = brick_reward[i];
                if (rewardType == 0)
                {
                    nullList.Add(i);
                    hasNull = true;
                }
                else if (rewardType == 1)
                {
                    jackpotList.Add(i);
                    hasJackpot = true;
                }
                else if (rewardType == 2)
                {
                    cashList.Add(i);
                    hasCash = true;
                }
                else if (rewardType == 3)
                {
                    goldList.Add(i);
                    hasGold = true;
                }
            }
        }

        if (save.player.cash > 96)
            hasCash = false;

        if (hasNull)
        {
            minNull = maxWeight;
            maxWeight += 80;
            maxNull = maxWeight;
        }
        if (hasJackpot)
        {
            minJackpot = maxWeight;
            maxWeight += 300;
            maxJackpot = maxWeight;
        }
        if (hasCash)
        {
            minCash = maxWeight;
            maxWeight += 200;
            maxCash = maxWeight;
        }
        if (hasGold)
        {
            minGold = maxWeight;
            maxWeight += 200;
            maxGold = maxWeight;
        }
        int result = Random.Range(0, maxWeight);
        if (result >= minNull && result < maxNull)
        {
            if (nullList.Count > 0)
            {
                step = nullList[Random.Range(0, nullList.Count)];
                //Debug.Log("步数 : " + (step == 0 ? (Go_bricks.Count - currentStep) : (step - currentStep)) + " , 没有获得东西");
            }
        }
        else if (result >= minJackpot && result < maxJackpot)
        {
            if (jackpotList.Count > 0)
            {
                step = jackpotList[Random.Range(0, jackpotList.Count)];
                //Debug.Log("步数 : " + (step - currentStep) + " , 获得老虎机");
            }
        }
        else if (result >= minCash && result < maxCash)
        {
            if (cashList.Count > 0)
            {
                step = cashList[Random.Range(0, cashList.Count)];
                //Debug.Log("步数 : " + (step - currentStep) + " , 获得美元");
            }
        }
        else if (result >= minGold && result < maxGold)
        {
            if (goldList.Count > 0)
            {
                step = goldList[Random.Range(0, goldList.Count)];
                //Debug.Log("步数 : " + (step - currentStep) + " , 获得金币");
            }
        }
        if (step == 0)
            step = bricks_Pos.Length - currentStep;
        else
            step -= currentStep;
        //Debug.Log("步数 : " + step);
        return step;
    }
    static readonly Vector3 dice_StartPos = new Vector3(16.15f, 20.02f, 1.28f);
    static readonly Quaternion[] dice_startRotations = new Quaternion[6]
    {
        new Quaternion(-0.5f,1.32f,2.49f,0),
        new Quaternion(0.47f,0.6f,0.2f,-0.25f),
        new Quaternion(-0.5f,1.32f,0.63f,0),
        new Quaternion(-0.36f,-0.59f,-0.58f,-0.25f),
        new Quaternion(-0.7f,-0.6f,0.2f,-0.95f),
        new Quaternion(0.1f,0.89f,-0.49f,1)
    };
    static readonly Vector3[] dice_startAngleDir = new Vector3[6]
    {
        new Vector3(0.55f,0,1.87f),
        new Vector3(-0.28f,0,1.92f),
        new Vector3(0.55f,0,1.87f),
        new Vector3(-0.5f,0,1.87f),
        new Vector3(0.15f,0,2.99f),
        new Vector3(0.55f,0,1.87f),
    };
    static readonly Vector3[] dice_startMoveDir = new Vector3[6]
    {
        new Vector3(-14.03f,0,0),
        new Vector3(-12,0,0),
        new Vector3(-10.4f,0,0),
        new Vector3(-11.01f,0,0),
        new Vector3(-12.45f,0,0),
        new Vector3(-12.3f,0,0)
    };
    public void RollDice()
    {
        if (!canRollDice) return;
        MG_Manager.Instance.Add_Save_DiceTotalTimes();
        SendAdjustDiceEvent();
        canRollDice = false;
        int diceValue = RandomStep();
        StartCoroutine(RollDice(diceValue));
    }
    public bool canRollDice = true;
    IEnumerator RollDice(int diceValue)
    {
        dice_Rig.isKinematic = false;
        dice_Trans.position = dice_StartPos;
        dice_Trans.rotation = dice_startRotations[diceValue-1];
        dice_Trans.gameObject.SetActive(true);
        dice_Rig.angularVelocity = dice_startAngleDir[diceValue-1];
        dice_Rig.velocity = dice_startMoveDir[diceValue-1];
        while (true)
        {
            yield return null;
            if (dice_Rig.angularVelocity == Vector3.zero && dice_Rig.velocity == Vector3.zero)
                break;
        }
        yield return MoveByStep(diceValue);
        //canRollDice = true;
    }
    public Image img_SceneBg;
    public Image img_SceneMid;
    public Image img_SceneTop;
    const int maxSceneIndex = 3;
    void ChangeSceneBg()
    {
        save.player.sceneIndex++;
        MG_SaveManager.CurrentBgIndex++;
        if (save.player.sceneIndex > maxSceneIndex)
            save.player.sceneIndex = 0;
        img_SceneBg.sprite = Resources.Load<Sprite>("Scenes/Scene" + save.player.sceneIndex);
        img_SceneMid.sprite = Resources.Load<Sprite>("Scenes/SceneMid" + save.player.sceneIndex);
        img_SceneTop.sprite = Resources.Load<Sprite>("Scenes/SceneTop" + save.player.sceneIndex);
        car_material.SetVector("_LightDir", car_shadows[save.player.sceneIndex]);
        Resources.UnloadUnusedAssets();
    }
    public void GetExtraBonus()
    {
        nextRewardNum = GetExtraBonusRandom(out RewardType newRewardType);
        nextRewardType = newRewardType;
        nextRewardMutiple = 1;
        panelManager.ShowPanel(PanelType.Reward);
    }
    public int GetGold()
    {
        return save.player.gold;
    }
    public int GetCash()
    {
        return save.player.cash;
    }
    public int GetEnergy()
    {
        return save.player.energy;
    }
    public int GetStep()
    {
        return save.player.step;
    }
    public int GetTimeToBonus()
    {
        return save.player.stepToGetExtraBonus;
    }
    public int GetOfflineEnergyAndNextRevertTime(out int nextNeedseconds)
    {
        System.DateTime now = System.DateTime.Now;
        System.TimeSpan interval= now- save.player.lastRevertEnergyDate;
        int total = (int)interval.TotalSeconds;
       int  leftseconds = total % SaveManager.PLAYER_SECOND;
        int offlineAddEnergy= total / SaveManager.PLAYER_SECOND;
        save.player.energy += offlineAddEnergy;
        if (offlineAddEnergy > 0)
            save.player.lastRevertEnergyDate = now.AddSeconds(-leftseconds);
        if (save.player.energy > SaveManager.PLAYER_MAXENERGY)
            save.player.energy = SaveManager.PLAYER_MAXENERGY;
        nextNeedseconds = SaveManager.PLAYER_SECOND - leftseconds;
        return save.player.energy;
    }
    public int[] GetBrickReward()
    {
        return save.player.brickReward;
    }
    public bool[] GetBrickRewardGet()
    {
        return save.player.brickRewardGet;
    }
    private Panel_Game gamePanel;
    public bool AddGold(int num)
    {
        if (save.player.gold + num < 0)
        {
            notice = "You have not enough gold";
            panelManager.ShowPanel(PanelType.Notice);
            return false;
        }
        else
        {
            save.player.gold += num;
            if (gamePanel is null)
                gamePanel = panelManager.GetPanel(PanelType.Game).GetComponent<Panel_Game>();
            gamePanel.RefreshGold();
            if (num > 0)
            {
                coinCollectIsGold = true;
                coinCollect.SetActive(true);
                ShowGetRewardEffect();
            }
            return true;
        }
    }
    public bool AddCash(int num)
    {
        if (save.player.cash + num < 0)
        {
            notice = "You have not enough cash";
            panelManager.ShowPanel(PanelType.Notice);
            return false;
        }
        else
        {
            save.player.cash += num;
            if (gamePanel is null)
                gamePanel = panelManager.GetPanel(PanelType.Game).GetComponent<Panel_Game>();
            gamePanel.RefreshCash();
            if (num > 0)
            {
                coinCollectIsGold = false;
                coinCollect.SetActive(true);
                ShowGetRewardEffect();
            }
            return true;
        }
    }
    public bool ReduceEnergy(int value = 1)
    {
        if (save.player.energy - value < 0)
            return false;
        else
        {
            save.player.energy -= value;
            if (gamePanel is null)
                gamePanel = panelManager.GetPanel(PanelType.Game).GetComponent<Panel_Game>();
            gamePanel.RefreshEnergy();
            save.player.totalWasteEnergy += value;
            return true;
        }
    }
    public int AddEnergy(int value = 1)
    {
        save.player.energy += value;
        if (save.player.energy > SaveManager.PLAYER_MAXENERGY)
            save.player.energy = SaveManager.PLAYER_MAXENERGY;
        if (gamePanel is null)
            gamePanel = panelManager.GetPanel(PanelType.Game).GetComponent<Panel_Game>();
        gamePanel.RefreshEnergy();
        return save.player.energy;
    }
    public int AddEnergyNatural(int value = 1)
    {
        save.player.energy += value;
        if (save.player.energy > SaveManager.PLAYER_MAXENERGY)
            save.player.energy = SaveManager.PLAYER_MAXENERGY;
        save.player.lastRevertEnergyDate = System.DateTime.Now;
        return save.player.energy;
    }
    public void ReduceTimeToBonus(int value = 1)
    {
        save.player.stepToGetExtraBonus -= value;
        if (save.player.stepToGetExtraBonus < 1)
        {
            save.player.stepToGetExtraBonus = GetExtraBonusNeedStep();
            canGetExtraBonus = true;
        }
    }
    public int GetNextSigninDay()
    {
        return save.player.nextsigninDay;
    }
    public bool CheckCanSignin()
    {
        if (save.player.nextsigninDay > 7)
            return false;
        System.DateTime lastSigninTime = save.player.lastSigninDate;
        System.DateTime now = System.DateTime.Now;
        if (lastSigninTime.Year < now.Year)
            return true;
        else if (lastSigninTime.Year == now.Year)
        {
            if (lastSigninTime.Month < now.Month)
                return true;
            else if (lastSigninTime.Month == now.Month)
            {
                return lastSigninTime.Day < now.Day;
            }
            else
                return false;
        }
        else
            return false;
    }
    public void Signin(System.DateTime now,bool watchAd)
    {
        int today = save.player.nextsigninDay;
        today %= 7;
        save.player.signStatePerDay = save.player.signStatePerDay.Remove(today, 1).Insert(today, watchAd ? "1" : "0");
        save.player.lastSigninDate = now;
        save.player.nextsigninDay = today + 1;
    }
    public bool CheckFirstSignin(bool setValue)
    {
        bool result = save.player.firstLogin;
        if (result)
            save.player.firstLogin = setValue;
        return result;
    }
    public string GetSignStatePerDay()
    {
        if (string.IsNullOrEmpty(save.player.signStatePerDay))
            save.player.signStatePerDay = "0000000";
        return save.player.signStatePerDay;
    }
    public bool GetMusicOn()
    {
        return save.player.musicOn;
    }
    public bool GetSoundOn()
    {
        return save.player.soundOn;
    }
    public void SetMusicOn(bool value, bool isMG_Setting = false)
    {
        save.player.musicOn = value;
        audio.SetMusic(value);
        if (!isMG_Setting)
            MG_Manager.Instance.Set_Save_MusicOn(value, false);
    }
    public void SetSoundOn(bool value, bool isMG_Setting = false)
    {
        save.player.soundOn = value;
        audio.SetSound(value);
        if (!isMG_Setting)
            MG_Manager.Instance.Set_Save_SoundOn(value, false);

    }
    public bool GetShowExchange()
    {
#if UNITY_EDITOR
        return true;
#endif
        return save.player.showExchange;
    }
    public void SetShowExchange(bool value)
    {
        if (save.player.showExchange != value)
        {
            save.player.showExchange = value;
            if (value)
                SendAdjustPackBEvent();
        }
    }
    Config config;
    int GetConfigIndex()
    {
        float currentCash = save.player.cash;
        List<int> range = config.Range;
        int rangeCount = range.Count;
        for(int i = 0; i < rangeCount; i++)
        {
            int max = i == rangeCount - 1 ? 100000 : range[i + 1];
            if (i == 0)
            {
                if (currentCash >= range[0] && currentCash <= max)
                    return i;
                else
                    continue;
            }
            else
            {
                if (currentCash > range[i] && currentCash <= max)
                    return i;
                else
                    continue;
            }
        }
        return 0;
    }
    public int GetExtraBonusNeedStep()
    {
        int index = GetConfigIndex();
        ExtraBonusConfig tempConfig = config.ExtraBonusConfigs[index];
        return tempConfig.needTargetStep;
    }
    private int GetExtraBonusRandom(out RewardType rewardType)
    {
        int index = GetConfigIndex();
        ExtraBonusConfig tempConfig = config.ExtraBonusConfigs[index];
        float totalRate = tempConfig.cashBonusRate + tempConfig.goldBonusRate;
        float rateResult = Random.Range(0, totalRate);
        if (rateResult >= tempConfig.cashBonusRate)
            rewardType = RewardType.ExtraBonusGold;
        else
            rewardType = RewardType.ExtraBonusCash;
        int result;
        if (rewardType == RewardType.ExtraBonusGold)
        {
            result = Random.Range(tempConfig.minGoldBonus, tempConfig.maxGoldBonus + 1);
            result -= result % 10;
        }
        else
        {
            result = Random.Range(tempConfig.minCashBonus, tempConfig.maxCashBonus + 1);
        }
        return result;
    }
    public void GetSpecialPropsRandom(bool isGold)
    {
        float mutiple;
        int index = GetConfigIndex();
        SpecialPropsConfig tempConfig = config.SpecialPropsConfigs[index];
        int result;
        if (isGold)
        {
            result = Random.Range(tempConfig.minGoldReward, tempConfig.maxGoldReward + 1);
            result -= result % 10;
            int mutipleIndex = Random.Range(0, tempConfig.goldMutiple.Count);
            mutiple = tempConfig.goldMutiple[mutipleIndex];
        }
        else
        {
            result = Random.Range(tempConfig.minCashReward, tempConfig.maxCashReward + 1);
            int mutipleIndex = Random.Range(0, tempConfig.cashMutiple.Count);
            mutiple = tempConfig.cashMutiple[mutipleIndex];
        }
        nextRewardMutiple = mutiple;
        nextRewardNum = result;
        nextRewardType = isGold ? RewardType.Gold : RewardType.Cash;
    }
    public int GetJackpotRandom(out RewardType rewardType)
    {
        int index = GetConfigIndex();
        JackpotConfig tempJackpotConfig = config.JackpotConfigConfigs[index];
        SpecialPropsConfig tempSPConfig = config.SpecialPropsConfigs[index];
        float totalRate = tempJackpotConfig.noRewardRate + tempJackpotConfig.goldRewardRate + tempJackpotConfig.cashRewardRate;
        float rateResult = Random.Range(0, totalRate);
        if (rateResult < tempJackpotConfig.noRewardRate)
        {
            rewardType = RewardType.Null;
            return 0;
        }
        else if(rateResult>=tempJackpotConfig.noRewardRate&& rateResult < tempJackpotConfig.noRewardRate + tempJackpotConfig.goldRewardRate)
        {
            rewardType = RewardType.Gold;
            int baseNum = tempJackpotConfig.goldPool[Random.Range(0, tempJackpotConfig.goldPool.Count)];
            nextRewardMutiple = tempSPConfig.goldMutiple[Random.Range(0, tempSPConfig.goldMutiple.Count)];
            nextRewardType = RewardType.Gold;
            nextRewardNum = baseNum;
            return baseNum;
        }
        else
        {
            rewardType = RewardType.Cash;
            int baseNum = tempJackpotConfig.cashPool[Random.Range(0, tempJackpotConfig.cashPool.Count)];
            nextRewardMutiple = tempSPConfig.cashMutiple[Random.Range(0, tempSPConfig.cashMutiple.Count)];
            nextRewardType = RewardType.Cash;
            nextRewardNum = baseNum;
            return baseNum;
        }
    }
    [SerializeField]
    private ParticleSystem ps_effect1;
    [SerializeField]
    private ParticleSystem ps_effect2;
    void ShowGetRewardEffect()
    {
        ps_effect1.Play(true);
        ps_effect2.Play(true);
    }

    public void SendAdjustPackBEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEventNoParam(AdjustEventLogger.TOKEN_packb);
    }
    public void SendAdjustGameStartEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEventNoParam(AdjustEventLogger.TOKEN_open);
    }
    public void SendAdjustPlayAdEvent(bool hasAd,bool isRewardAd,string adByWay)
    {
#if UNITY_EDITOR
        return;
#endif
        MG_Manager.Instance.SendAdjustPlayAdEvent(hasAd, isRewardAd, adByWay);
    }
    public void SendAdjustDiceEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        MG_Manager.Instance.SendAdjustDiceEvent();
    }
    public void SendFBAttributeEvent(string uri)
    {
#if UNITY_EDITOR
        return;
#endif
        MG_Manager.Instance.SendFBAttributeEvent(uri);
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            save.player.step = currentStep;
            save.Save();
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            save.player.step = currentStep;
            save.Save();
        }
    }
    private void OnApplicationQuit()
    {
        save.player.step = currentStep;
        save.Save();
    }
}
