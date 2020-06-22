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
    public Transform player_Trans;
    public Transform dice_Trans;
    private Rigidbody dice_Rig;
    private Animator player_Animator;
    public List<GameObject> Go_bricks = new List<GameObject>();
    public List<int> cornor_index = new List<int>();
    /// <summary>
    /// 0灰黄色1蓝色2绿色3蛋黄色
    /// </summary>
    public List<Material> Mat_bricks = new List<Material>();
    private readonly Dictionary<int, int> brick_reward_Dic = new Dictionary<int, int>();
    private readonly Dictionary<int, GameObject> brick_rewardGo_Dic = new Dictionary<int, GameObject>();
    public Transform specialBrickParent;
    private int[] brick_reward;
    private bool[] brick_reward_get;
    private GameObject prefab_gold;
    private GameObject[] go_golds = new GameObject[8];//5-8
    private int goldgoIndex = 0;
    private GameObject prefab_cash;
    private GameObject[] go_cashs = new GameObject[2];//1-2
    private int cashgoIndex = 0;
    private GameObject prefab_jackpot;
    private GameObject[] go_jackpots = new GameObject[2];//1-2
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
        Time.timeScale = 2;
        panelManager = GetComponent<PanelManager>();
        config = Resources.Load<Config>("Config");
        save = GetComponent<SaveManager>();
        save.Init();
        //save.player.showExchange = true;
        save.player.gameTimes++;
        panelManager.ShowPanel(PanelType.Loading);
        Application.targetFrameRate = 60;
        audio = GetComponent<AudioManager>();
        dice_Rig = dice_Trans.GetComponent<Rigidbody>();
        player_Animator = player_Trans.GetComponent<Animator>();
        audio.Init(save.player.musicOn);
        panelManager.PreLoadPanel(PanelType.Game);
    }
    private void Start()
    {
        player_Trans.localScale = player_OriginScale;
        currentStep = save.player.step;
        Vector3 endPos = player_StartPos;
        int rot = 0;
        for (int i = 1; i <= currentStep; i++)
        {
            if (i <= cornor_index[0])
                endPos.z += Z_ps;
            else if (i > cornor_index[0] && i <= cornor_index[1])
            {
                rot = 1;
                endPos.x -= X_ps;
            }
            else if (i > cornor_index[1] && i <= cornor_index[2])
            {
                rot = 2;
                endPos.z -= Z_ps;
            }
            else if (i > cornor_index[2] && i <= cornor_index[3])
            {
                rot = 3;
                endPos.x += X_ps;
            }
            else
            {
                rot = 4;
                endPos.z += Z_ps;
            }
        }
        switch (rot)
        {
            case 0:
            case 4:
                player_Trans.rotation = player_RightRotation;
                break;
            case 1:
                player_Trans.rotation = player_UpRotation;
                break;
            case 2:
                player_Trans.rotation = player_LeftRotation;
                break;
            case 3:
                player_Trans.rotation = player_DownRotation;
                break;
            default:
                player_Trans.rotation = player_RightRotation;
                break;
        }
        player_Trans.position = endPos;
        dice_Trans.position = dice_StartPos;
        dice_Trans.rotation = dice_StartRotation;
        brick_reward = save.player.brickReward;
        brick_reward_get = save.player.brickRewardGet;
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
                Go_bricks[i].GetComponent<MeshRenderer>().material = Mat_bricks[rewardType];
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
                        tempJackpot.transform.position = Go_bricks[i].transform.position;
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
                        tempCash.transform.position = Go_bricks[i].transform.position;
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
                        tempGold.transform.position = Go_bricks[i].transform.position;
                        tempGold.SetActive(true);
                        brick_reward_Dic.Add(i, 3);
                        brick_rewardGo_Dic.Add(i, tempGold);
                        goldgoIndex++;
                        break;
                    default:
                        break;
                }
            }

        img_Scene.sprite = Resources.Load<Sprite>("Scenes/Scene" + save.player.sceneIndex);
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
    static readonly Vector3 player_StartPos = new Vector3(3.42f, -0.131f, -2.14f);
    static readonly Quaternion player_LeftRotation = new Quaternion(0, 1, 0.2f, 0);
    static readonly Quaternion player_RightRotation = new Quaternion(0.2f, 0, 0, 1);
    static readonly Quaternion player_UpRotation = new Quaternion(0.2f, -0.7f, -0.2f, 0.7f);
    static readonly Quaternion player_DownRotation = new Quaternion(0.2f, 0.7f, 0.2f, 0.7f);
    static readonly Vector3 player_OriginScale = new Vector3(102.954f, 102.954f, 102.954f);
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
        int totalBrick = Go_bricks.Count;

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
                    goldGo.transform.position = Go_bricks[rewardPos].transform.position;
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
                    cashGo.transform.position = Go_bricks[rewardPos].transform.position;
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
                    jackpotGo.transform.position = Go_bricks[rewardPos].transform.position;
                    jackpotGo.SetActive(true);
                    jackpotsgoIndex++;
                    break;
                }
            }
        }

        for (int i = 1; i < brick_reward.Length; i++)
        {
            Go_bricks[i].GetComponent<MeshRenderer>().material = Mat_bricks[brick_reward[i]];
        }
    }
    int currentStep = 0;
    const float X_ps = 0.95646f;
    const float Z_ps = 1;
    const float Y_Max = 0.45f;//0.45f
    IEnumerator JumpForStep(int step)
    {
        player_Animator.SetBool("Jump", true);
        int cornorIndex1 = cornor_index[0];
        int cornorIndex2 = cornor_index[1];
        int cornorIndex3 = cornor_index[2];
        int cornorIndex4 = cornor_index[3];
        for(int i = 0; i < step; i++)
        {
            audio.PlayerSound("Jump");
            Vector3 oldPos = player_Trans.position;
            //Z轴跳跃
            if (currentStep < cornorIndex1 || (currentStep >= cornorIndex2 && currentStep < cornorIndex3) || currentStep >= cornorIndex4)
            {
                if (currentStep == cornorIndex2)
                    player_Trans.rotation = player_LeftRotation;
                if (currentStep == cornorIndex4)
                    player_Trans.rotation = player_RightRotation;
                float offsetZ;
                if (currentStep >= cornorIndex2 && currentStep < cornorIndex3)
                    offsetZ = -Z_ps;
                else
                    offsetZ = Z_ps;
                Vector3 midPos = new Vector3(oldPos.x, player_StartPos.y + Y_Max, oldPos.z + offsetZ / 2);
                while (true)
                {
                    player_Trans.position = Vector3.Lerp(player_Trans.position, midPos, 0.5f);
                    yield return null;
                    if (Vector3.Distance(player_Trans.position, midPos) < 0.02f)
                        break;
                }

                if (i == step - 1)
                {
                    if (brick_rewardGo_Dic.ContainsKey(currentStep + 1))
                    {
                        brick_rewardGo_Dic[currentStep + 1].SetActive(false);
                        brick_reward_get[currentStep + 1] = true;
                    }
                }

                Vector3 endPos = new Vector3(oldPos.x, player_StartPos.y, oldPos.z + offsetZ);
                while (true)
                {
                    player_Trans.position = Vector3.Lerp(player_Trans.position, endPos, 0.5f);
                    yield return null;
                    if (Vector3.Distance(player_Trans.position, endPos) < 0.02f)
                        break;
                }
            }
            //X轴跳跃
            else
            {
                if (currentStep == cornorIndex1)
                    player_Trans.rotation = player_UpRotation;
                if (currentStep == cornorIndex3)
                    player_Trans.rotation = player_DownRotation;
                float offsetX;
                if (currentStep >= cornorIndex1 && currentStep < cornorIndex2)
                    offsetX = -X_ps;
                else
                    offsetX = X_ps;
                Vector3 midPos = new Vector3(oldPos.x + offsetX / 2, player_StartPos.y + Y_Max, oldPos.z);
                while (true)
                {
                    player_Trans.position = Vector3.Lerp(player_Trans.position, midPos, 0.5f);
                    yield return null;
                    if (Vector3.Distance(player_Trans.position, midPos) < 0.02f)
                        break;
                }


                if (i == step - 1)
                {
                    if (brick_rewardGo_Dic.ContainsKey(currentStep + 1))
                    {
                        brick_rewardGo_Dic[currentStep + 1].SetActive(false);
                        brick_reward_get[currentStep + 1] = true;
                    }
                }

                Vector3 endPos = new Vector3(oldPos.x + offsetX, player_StartPos.y, oldPos.z);
                while (true)
                {
                    player_Trans.position = Vector3.Lerp(player_Trans.position, endPos, 0.5f);
                    yield return null;
                    if (Vector3.Distance(player_Trans.position, endPos) < 0.02f)
                        break;
                }

            }
            currentStep++;
            if (currentStep > Go_bricks.Count - 1)
            {
                player_Trans.position = player_StartPos;
                player_Trans.rotation = player_RightRotation;
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
            yield return new WaitForSeconds(0.3f);
            if (i == step - 1)
            {
                //获得奖励
                int rewardType = brick_reward[currentStep];
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
            }
        }
        player_Animator.SetBool("Jump", false);
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
        if (currentStep < Go_bricks.Count - 6)
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
            for(int i = currentStep + 1; i < Go_bricks.Count; i++)
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
            step = Go_bricks.Count - currentStep;
        else
            step -= currentStep;
        //Debug.Log("步数 : " + step);
        return step;
    }
    static readonly Vector3 dice_StartPos = new Vector3(0.18f, 3.04f, -11.79f);
    static readonly Quaternion dice_StartRotation = new Quaternion(0.5f, -0.5f, 0.5f, 0f);
    public Vector3 dice_anglarV;
    public Vector3 dice_moveV;
    static readonly Vector3[][,] dice_angular_move_V = new Vector3[][,]
    {
        //骰子1
        new Vector3[,]
        {
            { new Vector3(10, 0 , 0) , new Vector3(0,0,8) },
            { new Vector3(8, 2,2),new Vector3(0,0,8) },
            { new Vector3(10,5,-5),new Vector3(0,0,7) },
        },
        //骰子2
        new Vector3[,]
        {
            { new Vector3(5, 5, 10),new Vector3(0, 0, 8)},
            { new Vector3(10, 10, 10),new Vector3(0, 0, 8) },
            { new Vector3(0,0,10),new Vector3(0,0,6) },
            { new Vector3(10,10,10), new Vector3(0,0,6)},
            { new Vector3(5,5,-5),new Vector3(0,0,8) },
        },
        //骰子3
        new Vector3[,]
        {
            { new Vector3(10,0,0), new Vector3(0,0,7) },
            { new Vector3(10,0,-5),new Vector3(0,0,7) },
            { new Vector3(10,-5,0),new Vector3(0,0,9) },
            { new Vector3(5,5,-1),new Vector3(-2,0,8) },
        },
        //骰子4
        new Vector3[,]
        {
            { new Vector3(0, 0, 10),new Vector3(0, 0, 8) },
            { new Vector3(0, 0, 10),new Vector3(0, 0, 7) },
            { new Vector3(10,5,0),new Vector3(0,0,7) },
            { new Vector3(5,-5,5), new Vector3(0,0,6)},
        },
        //骰子5
        new Vector3[,]
        {
            { new Vector3(10,0,0), new Vector3(0,0,9)},
            {new Vector3(10,0,-10),  new Vector3(0,0,7)},
            { new Vector3(2,-5,2),new Vector3(0,0,7)},
            {new Vector3(5,5,0), new Vector3(0,0,11)},
        },
        //骰子6
        new Vector3[,]
        {
            { new Vector3(5,0,-10), new Vector3(0, 0, 7) },
            {new Vector3(10,-5,5),new Vector3(0,0,8) },
            { new Vector3(3,0,0),new Vector3(-1,0,7)},
        }
    };
    public void RollDice()
    {
        if (!canRollDice) return;
        SendAdjustDiceEvent();
        canRollDice = false;
        dice_Trans.position = dice_StartPos;
        dice_Trans.rotation = dice_StartRotation;
        int diceValue = RandomStep();
        StartCoroutine(RollDice(diceValue));
    }
    public bool canRollDice = true;
    IEnumerator RollDice(int diceValue)
    {
        Vector3[,] tempDiceData = dice_angular_move_V[diceValue - 1];
        int randomIndex = Random.Range(0, tempDiceData.Rank);
        dice_Rig.angularVelocity = tempDiceData[randomIndex, 0];
        dice_Rig.velocity = tempDiceData[randomIndex, 1];
        while (true)
        {
            yield return null;
            if (dice_Rig.angularVelocity == Vector3.zero && dice_Rig.velocity == Vector3.zero)
                break;
        }
        yield return JumpForStep(diceValue);
        //canRollDice = true;
    }
    public Image img_Scene;
    const int maxSceneIndex = 4;
    void ChangeSceneBg()
    {
        save.player.sceneIndex++;
        if (save.player.sceneIndex > maxSceneIndex)
            save.player.sceneIndex = 0;
        img_Scene.sprite = Resources.Load<Sprite>("Scenes/Scene" + save.player.sceneIndex);
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
    public void Signin(System.DateTime now)
    {
        save.player.lastSigninDate = now;
        save.player.nextsigninDay++;
    }
    public bool CheckFirstSignin(bool setValue)
    {
        bool result = save.player.firstLogin;
        if (result)
            save.player.firstLogin = setValue;
        return result;
    }
    public bool GetMusicOn()
    {
        return save.player.musicOn;
    }
    public bool GetSoundOn()
    {
        return save.player.soundOn;
    }
    public void SetMusicOn(bool value)
    {
        save.player.musicOn = value;
        audio.SetMusic(value);
    }
    public void SetSoundOn(bool value)
    {
        save.player.soundOn = value;
        audio.SetSound(value);
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
        save.player.showExchange = value;
        if (value)
            SendAdjustPackBEvent();
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
        AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
            //累计美元
            ("value", save.player.cash.ToString()),
            //累计金币
            ("new_value", save.player.gold.ToString()),
            //累计体力
            ("stage_id", save.player.totalWasteEnergy.ToString()),
            //广告id
            ("id", adByWay),
            //广告类型，0插屏1奖励视频
            ("type", isRewardAd ? "1" : "0")
            );
    }
    public void SendAdjustDiceEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
            //累计美元
            ("value", save.player.cash.ToString()),
            //累计金币
            ("new_value", save.player.gold.ToString()),
            //累计体力
            ("stage_id", save.player.totalWasteEnergy.ToString()),
            //游戏次数
            ("id", save.player.totalWasteEnergy.ToString())
            );
    }
    public void SendFBAttributeEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_deeplink,
            //累计美元
            ("value", save.player.cash.ToString()),
            //累计金币
            ("new_value", save.player.gold.ToString()),
            //累计体力
            ("stage_id", save.player.totalWasteEnergy.ToString())
            );
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
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
