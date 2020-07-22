using MiddleGround.Audio;
using MiddleGround.GameConfig;
using MiddleGround.Save;
using MiddleGround.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiddleGround
{
    public class MG_Manager : MonoBehaviour
    {
        public static MG_Manager Instance;
        public bool canChangeGame = true;

        public MG_PopRewardPanel_RewardType MG_PopDiceReward_Type;
        public int MG_PopDiceReward_Num;
        public float MG_PopDiceReward_Mutiple;

        public MG_PopDoublePanel_RewardType MG_PopDoublePanel_Type;
        public int MG_PopDoublePanel_Num;

        public int MG_PopCashPanel_Num;
        public bool hasGift = false;

        public MG_Pop_FlyReward MG_Fly;

        public string str_Tips = "";
        public float time_Tips = 2;

        readonly Sprite[] MG_GamePanel_BG = new Sprite[4];
        public bool loadEnd = false;
        public ParticleSystem ps_effectL;
        public ParticleSystem ps_effectR;
        public bool willRateus = false;
        public bool isGuid = false;
        public MG_Guid_Type next_GuidType = MG_Guid_Type.Null;

        public bool isInMG = false;

        MG_Config MG_Config;
        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
            gameObject.AddComponent<MG_UIManager>().Init(
                transform.GetChild(1),
                transform.GetChild(0),
                transform.GetChild(2)
                );
            MG_Config = Resources.Load<ScriptableObject>("MG_ConfigAssets/MG_Dice_Config") as MG_Config;
            gameObject.AddComponent<MG_AudioManager>().Init(transform.Find("MG_AudioRoot").gameObject);
        }
        public void Init()
        {
            MG_Fly.Init();
            MG_UIManager.Instance.ShowMenuPanel();
        }
        public void ShowPopPanel(MG_PopPanelType _PopPanelType)
        {
            MG_UIManager.Instance.ShowPopPanelAsync(_PopPanelType);
        }
        public void CloseTopPopPanel()
        {
            if (canChangeGame)
                MG_UIManager.Instance.CloseTopPopPanelAsync();
        }
        public bool Get_Save_SoundOn()
        {
            return GameManager.Instance.GetSoundOn();
        }
        public void Set_Save_SoundOn(bool value, bool isMG_Setting = true)
        {
            if (isMG_Setting)
                GameManager.Instance.SetSoundOn(value, true);
            MG_AudioManager.Instance.SetSoundState(value);
        }
        public bool Get_Save_MusicOn()
        {
            return GameManager.Instance.GetMusicOn();
        }
        public void Set_Save_MusicOn(bool value, bool isMG_Setting = true)
        {
            if (isMG_Setting)
                GameManager.Instance.SetMusicOn(value, true);
            MG_AudioManager.Instance.SetMusicState(value);
        }
        public bool Get_Save_PackB()
        {
            return GameManager.Instance.GetShowExchange();
        }
        public void Set_Save_isPackB()
        {
            if (MG_SaveManager.PackB) return;
            MG_SaveManager.PackB = true;
            SendAdjustPackBEvent();
        }
        public int Get_Save_Gold()
        {
            return GameManager.Instance.GetGold();
        }
        public void Add_Save_Gold(int value)
        {
            GameManager.Instance.AddGold(value);
            if (value < 0)
                MG_UIManager.Instance.UpdateMenuPanel_GoldText();
            MG_UIManager.Instance.UpdateSlotsSpinButton(Get_Save_Gold());
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_Cash()
        {
            return GameManager.Instance.GetCash();
        }
        public void Add_Save_Cash(int value)
        {
            GameManager.Instance.AddCash(value);
            if (value < 0)
                MG_UIManager.Instance.UpdateMenuPanel_CashText();
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_DiceLife()
        {
            return MG_SaveManager.DiceLife;
        }
        public void Add_Save_DiceLife(int value)
        {
            MG_SaveManager.DiceLife += value;
            if (value < 0)
                Add_Save_DiceTotalTimes(-value);
            MG_UIManager.Instance.UpdateDicePanel_DiceLifeText();
        }
        public int Get_Save_ScratchTicket()
        {
            return MG_SaveManager.ScratchTickets;
        }
        public void Add_Save_ScratchTicket(int value)
        {
            MG_SaveManager.ScratchTickets += value;
            if (value < 0)
            {
                MG_UIManager.Instance.UpdateMenuPanel_ScratchTicketText();
                Add_Save_ScratchTotalTimes(-value);
            }
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_WheelTickets()
        {
            return MG_SaveManager.WheelTickets;
        }
        public void Add_Save_WheelTickets(int value)
        {
            MG_SaveManager.WheelTickets += value;
            if (value < 0)
            {
                Add_Save_WheelTotalTimes(-value);
                MG_UIManager.Instance.UpdateWheelTicketText();
            }
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_Amazon()
        {
            return MG_SaveManager.DiceToken;
        }
        public void Add_Save_Amazon(int value)
        {
            MG_SaveManager.DiceToken += value;
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_777()
        {
            return MG_SaveManager.SpecialToken_777;
        }
        public void Add_Save_777(int value)
        {
            MG_SaveManager.SpecialToken_777 += value;
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_Fruits()
        {
            return MG_SaveManager.SpecialToken_Fruits;
        }
        public void Add_Save_Fruits(int value)
        {
            MG_SaveManager.SpecialToken_Fruits += value;
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_Diamond()
        {
            return MG_SaveManager.Diamond;
        }
        public void Add_Save_Diamond(int value)
        {
            MG_SaveManager.Diamond += value;
            if (value <= 0)
                MG_UIManager.Instance.UpdateMenuPanel_SpecialTokenText();
            if (value > 0)
                Play_Effect();
        }
        public int Get_Save_DiceTotalTimes()
        {
            return MG_SaveManager.DiceTotalPlayTimes;
        }
        public void Add_Save_DiceTotalTimes(int value = 1)
        {
            MG_SaveManager.DiceTotalPlayTimes += value;
            if (MG_SaveManager.DiceTotalPlayTimes == 3)
                next_GuidType = MG_Guid_Type.DiceGuid;
        }
        public int Get_Save_ScratchTotalTimes()
        {
            return MG_SaveManager.ScratchTotalPlayTimes;
        }
        public void Add_Save_ScratchTotalTimes(int value = 1)
        {
            MG_SaveManager.ScratchTotalPlayTimes += value;
        }
        public int Get_Save_SlotsTotalTimes()
        {
            return MG_SaveManager.SlotsTotalPlayTimes;
        }
        public void Add_Save_SlotsTotalTimes(int value=1)
        {
            MG_SaveManager.SlotsTotalPlayTimes += value;
        }
        public int Get_Save_WheelTotalTimes()
        {
            return MG_SaveManager.WheelTotalPlayTimes;
        }
        public void Add_Save_WheelTotalTimes(int value = 1)
        {
            MG_SaveManager.WheelTotalPlayTimes += value;
        }
        public int Get_Save_TotalTimes()
        {
            return MG_SaveManager.TotalPlayTimes;
        }
        public int Get_Save_NextSignDay()
        {
            return GameManager.Instance.GetNextSigninDay();
        }
        public bool Get_Save_WetherSign()
        {
            return GameManager.Instance.CheckCanSignin();
        }
        public string Get_Save_SignStatePerDay()
        {
            return GameManager.Instance.GetSignStatePerDay();
        }
        public void Signin(bool watchAd)
        {
            GameManager.Instance.Signin(DateTime.Now,watchAd);
        }
        public int Get_Config_NextGiftStep()
        {
            int rewardRangeIndex = Get_Config_DiceRewardRangeIndex();
            return MG_Config.MG_Dice_ExtraBonusConfigs[rewardRangeIndex].needTargetStep;
        }
        public Dictionary<int, MG_Dice_BrickConfig> Get_Config_DiceBrick()
        {
            Dictionary<int, MG_Dice_BrickConfig> result = new Dictionary<int, MG_Dice_BrickConfig>();
            foreach (MG_Dice_BrickConfig perConfig in MG_Config.MG_Dice_BricksConfigs)
            {
                int index = (int)perConfig.brickType;
                if (result.ContainsKey(index))
                {
                    Debug.LogError("Get MG_DicBrickConfig Error : config repeat bricktype.");
                    continue;
                }
                result.Add(index, perConfig);
            }
            return result;
        }
        int Get_Config_DiceRewardRangeIndex()
        {
            int _Cash = Get_Save_Cash();
            List<int> tempRange = MG_Config.MG_Dice_CashRange;
            int rangeCount = tempRange.Count;
            for (int i = 1; i < rangeCount; i++)
            {
                if (_Cash <= tempRange[i])
                    return i - 1;
            }
            return rangeCount - 1;
        }
        public Sprite Get_GamePanelBg()
        {
            int bgindex = MG_SaveManager.CurrentBgIndex;
            if(MG_GamePanel_BG[bgindex] is null)
            {
                MG_GamePanel_BG[bgindex] = Resources.Load<Sprite>("MG_GamePanel_BG/MG_GamePanel_BG" + bgindex);
            }
            return MG_GamePanel_BG[bgindex];
        }
        public MG_Wheel_RewardType[] Get_Config_WheelReward(out int[] rewardNum)
        {
            List<MG_Wheel_Config> _Wheel_Configs = MG_Config.MG_Wheel_Configs;
            int length = _Wheel_Configs.Count;
            MG_Wheel_RewardType[] _Wheel_ConfigTypes = new MG_Wheel_RewardType[length];
            rewardNum = new int[length];
            bool packB = Get_Save_PackB();
            for(int i = 0; i < length; i++)
            {
                if(!packB&& _Wheel_Configs[i].rewardType == MG_Wheel_RewardType.Amazon)
                {
                    _Wheel_ConfigTypes[i] = MG_Wheel_RewardType.Gold;
                    rewardNum[i] = 500;
                    continue;
                }
               _Wheel_ConfigTypes[i] = _Wheel_Configs[i].rewardType;
                rewardNum[i] = _Wheel_Configs[i].rewardNum;
            }
            return _Wheel_ConfigTypes;
        }
        public void Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType _Dice_RewardType)
        {
            int rewardRangeIndex = Get_Config_DiceRewardRangeIndex();
            switch (_Dice_RewardType)
            {
                case MG_PopRewardPanel_RewardType.Cash:
                    MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.Cash;
                    MG_Dice_SpecialPropsConfig _SpecialPropsCashConfig = MG_Config.MG_Dice_SpecialPropsConfigs[rewardRangeIndex];
                    MG_PopDiceReward_Num = UnityEngine.Random.Range(_SpecialPropsCashConfig.minCashReward, _SpecialPropsCashConfig.maxCashReward);
                    MG_PopDiceReward_Mutiple = _SpecialPropsCashConfig.cashMutiple[UnityEngine.Random.Range(0, _SpecialPropsCashConfig.cashMutiple.Count)];
                    MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.DiceRewardPanel);
                    break;
                case MG_PopRewardPanel_RewardType.Gold:
                    MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.Gold;
                    MG_Dice_SpecialPropsConfig _SpecialPropsGoldConfig = MG_Config.MG_Dice_SpecialPropsConfigs[rewardRangeIndex];
                    MG_PopDiceReward_Num = UnityEngine.Random.Range(_SpecialPropsGoldConfig.minGoldReward, _SpecialPropsGoldConfig.maxGoldReward);
                    MG_PopDiceReward_Mutiple = _SpecialPropsGoldConfig.goldMutiple[UnityEngine.Random.Range(0, _SpecialPropsGoldConfig.goldMutiple.Count)];
                    MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.DiceRewardPanel);
                    break;
                case MG_PopRewardPanel_RewardType.Extra:
                    MG_Dice_ExtraBonusConfig _ExtraBonusConfig = MG_Config.MG_Dice_ExtraBonusConfigs[rewardRangeIndex];
                    float result = UnityEngine.Random.Range(0, _ExtraBonusConfig.goldBonusRate + _ExtraBonusConfig.cashBonusRate);
                    if (result < _ExtraBonusConfig.goldBonusRate)
                    {
                        MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.ExtraGold;
                        MG_PopDiceReward_Num = UnityEngine.Random.Range(_ExtraBonusConfig.minGoldBonus, _ExtraBonusConfig.maxGoldBonus);
                    }
                    else
                    {
                        MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.ExtraCash;
                        MG_PopDiceReward_Num = UnityEngine.Random.Range(_ExtraBonusConfig.minCashBonus, _ExtraBonusConfig.maxCashBonus);
                    }
                    MG_PopDiceReward_Mutiple = 1;
                    MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ExtraRewardPanel);
                    break;
            }
        }
        public int Random_DiceSlotsReward(out bool isGold)
        {
            int rewardRangeIndex = Get_Config_DiceRewardRangeIndex();
            MG_Dice_JackpotConfig _Dice_JackpotConfig = MG_Config.MG_Dice_JackpotConfigs[rewardRangeIndex];
            float result = UnityEngine.Random.Range(0, _Dice_JackpotConfig.noRewardRate + _Dice_JackpotConfig.goldRewardRate + _Dice_JackpotConfig.cashRewardRate);
            if (result < _Dice_JackpotConfig.noRewardRate)
            {
                isGold = true;
                return 0;
            }
            else if (result < _Dice_JackpotConfig.noRewardRate + _Dice_JackpotConfig.goldRewardRate)
            {
                isGold = true;
                MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.Gold;
                MG_PopDiceReward_Num = _Dice_JackpotConfig.goldPool[UnityEngine.Random.Range(0, _Dice_JackpotConfig.goldPool.Count)];
                MG_PopDiceReward_Mutiple = _Dice_JackpotConfig.mutiplePool[UnityEngine.Random.Range(0, _Dice_JackpotConfig.mutiplePool.Count)];
                return MG_PopDiceReward_Num;
            }
            else
            {
                isGold = false;
                MG_PopDiceReward_Type = MG_PopRewardPanel_RewardType.Cash;
                MG_PopDiceReward_Num = _Dice_JackpotConfig.cashPool[UnityEngine.Random.Range(0, _Dice_JackpotConfig.cashPool.Count)];
                MG_PopDiceReward_Mutiple = _Dice_JackpotConfig.mutiplePool[UnityEngine.Random.Range(0, _Dice_JackpotConfig.mutiplePool.Count)];
                return MG_PopDiceReward_Num;
            }
        }
        public int Random_ScratchCardReward(out int rewardType)
        {
            List<MG_Scratch_Config> _Scratch_Configs = MG_Config.MG_Scratch_Configs;
            int configCount = _Scratch_Configs.Count;
            int cash = Get_Save_Cash();
            int sss = Get_Save_777();
            int scratchedTimes = MG_SaveManager.ScratchRewardCashAsTimeIndex - 1;
            if (MG_SaveManager.ScratchedTimes == MG_SaveManager.ScratchRewardCashAsTimeIndex - 1 && MG_SaveManager.TodayExtraRewardTimes > 0)
            {
                for (int i = 0; i < configCount; i++)
                {
                    if (i < configCount - 1)
                    {
                        if (cash >= _Scratch_Configs[i].RnageCashStart && cash < _Scratch_Configs[i + 1].RnageCashStart)
                        {
                            int rewardNum = UnityEngine.Random.Range(_Scratch_Configs[i].minCash, _Scratch_Configs[i].maxCash);
                            MG_SaveManager.ScratchCardRewardNum = rewardNum;
                            MG_SaveManager.ScratchCardRewardType = -2;
                            MG_SaveManager.ScratchRewardCashAsTimeIndex = UnityEngine.Random.Range(_Scratch_Configs[i].minScratchTimes, _Scratch_Configs[i].maxScratchTimes);
                            MG_SaveManager.ScratchedTimes = 0;
                            rewardType = -2;
                            return rewardNum;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        int rewardNum = UnityEngine.Random.Range(_Scratch_Configs[i].minCash, _Scratch_Configs[i].maxCash);
                        MG_SaveManager.ScratchCardRewardNum = rewardNum;
                        MG_SaveManager.ScratchCardRewardType = -2;
                        MG_SaveManager.ScratchRewardCashAsTimeIndex = UnityEngine.Random.Range(_Scratch_Configs[i].minScratchTimes, _Scratch_Configs[i].maxScratchTimes);
                        MG_SaveManager.ScratchedTimes = 0;
                        rewardType = -2;
                        return rewardNum;
                    }
                }
            }

            MG_SaveManager.ScratchedTimes++;
            for(int i = 0; i < configCount; i++)
            {
                if (i < configCount - 1)
                {
                    if (sss >= _Scratch_Configs[i].RnageSssStart && sss < _Scratch_Configs[i + 1].RnageSssStart)
                    {
                        int sssWeight = Get_Save_PackB() ? _Scratch_Configs[i].sssWeight : 0;
                        int goldWeight = _Scratch_Configs[i].goldWeight;
                        int result = UnityEngine.Random.Range(0, sssWeight + goldWeight);
                        if (result < sssWeight)
                        {
                            MG_SaveManager.ScratchCardRewardNum = 1;
                            MG_SaveManager.ScratchCardRewardType = -3;
                            rewardType = -3;
                            return 1;
                        }
                        else
                        {
                            int rewardNum = UnityEngine.Random.Range(_Scratch_Configs[i].minGold, _Scratch_Configs[i].maxGold);
                            MG_SaveManager.ScratchCardRewardNum = rewardNum;
                            MG_SaveManager.ScratchCardRewardType = -1;
                            rewardType = -1;
                            return rewardNum;
                        }
                    }
                }
                else
                {
                    int sssWeight = Get_Save_PackB() ? _Scratch_Configs[i].sssWeight : 0;
                    int goldWeight = _Scratch_Configs[i].goldWeight;
                    int result = UnityEngine.Random.Range(0, sssWeight + goldWeight);
                    if (result < sssWeight)
                    {
                        MG_SaveManager.ScratchCardRewardNum = 1;
                        MG_SaveManager.ScratchCardRewardType = -3;
                        rewardType = -3;
                        return 1;
                    }
                    else
                    {
                        int rewardNum = UnityEngine.Random.Range(_Scratch_Configs[i].minGold, _Scratch_Configs[i].maxGold);
                        MG_SaveManager.ScratchCardRewardNum = rewardNum;
                        MG_SaveManager.ScratchCardRewardType = -1;
                        rewardType = -1;
                        return rewardNum;
                    }
                }
            }

            Debug.LogError("Random ScratchReward Error : cash is out of range.");
            rewardType = 0;
            return 1;
        }
        //以位置为索引，
        List<MG_Wheel_RandomInfo> list_index_info = new List<MG_Wheel_RandomInfo>();
        public int Random_WheelReward()
        {
            list_index_info.Clear();
            int count = MG_Config.MG_Wheel_Configs.Count;
            int total = 0;
            int currentTime = Get_Save_WheelTotalTimes() + 1;
            for(int i = 0; i < count; i++)
            {
                MG_Wheel_Config _Wheel_Config = MG_Config.MG_Wheel_Configs[i];
                if (currentTime == _Wheel_Config.rewardThisWhereIndex)
                    return i;
                MG_Wheel_RandomInfo _RandomInfo = new MG_Wheel_RandomInfo() { hasThis = true, startIndex = 0, endIndex = 0 };
                if (_Wheel_Config.rewardType == MG_Wheel_RewardType.Cash)
                {
                    if (Get_Save_Cash() >= _Wheel_Config.maxCanGetValue || MG_SaveManager.TodayExtraRewardTimes <= 0)
                        _RandomInfo.hasThis = false;
                }
                else if (_Wheel_Config.rewardType == MG_Wheel_RewardType.Amazon)
                {
                    if (Get_Save_Amazon() >= _Wheel_Config.maxCanGetValue)
                        _RandomInfo.hasThis = false;
                }
                if(_RandomInfo.hasThis)
                {
                    _RandomInfo.startIndex = total;
                    total += _Wheel_Config.weight;
                    _RandomInfo.endIndex = total;
                }
                list_index_info.Add(_RandomInfo);
            }
            int resultNum = UnityEngine.Random.Range(0, total);
            for(int i = 0; i < count; i++)
            {
                MG_Wheel_RandomInfo _RandomInfo = list_index_info[i];
                if (!_RandomInfo.hasThis) continue;
                if (resultNum >= _RandomInfo.startIndex && resultNum <= _RandomInfo.endIndex)
                {
                    return i;
                }
            }
            Debug.LogError("Random MG_Wheel Reward Error : no reward random.");
            return -1;
        }
        struct MG_Wheel_RandomInfo
        {
            public bool hasThis;
            public int startIndex;
            public int endIndex;
        }
        Dictionary<int, MG_Slots_Config> dic_type_slotsConfigs = null;
        public MG_Slots_RewardType Random_SlotsReward(int useNum, out int num)
        {
            if(dic_type_slotsConfigs is null)
            {
                dic_type_slotsConfigs = new Dictionary<int, MG_Slots_Config>();
                int count = MG_Config.MG_Slots_Configs.Count;
                for(int i = 0; i < count; i++)
                {
                    dic_type_slotsConfigs.Add((int)MG_Config.MG_Slots_Configs[i].rewardType, MG_Config.MG_Slots_Configs[i]);
                }
            }
            MG_Slots_Config _SlotsGold = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Gold];
            MG_Slots_Config _SlotsCash = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Cash];
            MG_Slots_Config _SlotsDimaond = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Diamond];
            MG_Slots_Config _SlotsGift = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Gift];
            MG_Slots_Config _SlotsCherry = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Cherry];
            MG_Slots_Config _SlotsOrange = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Orange];
            MG_Slots_Config _SlotsWatermalen = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Watermalen];
            MG_Slots_Config _SlotsSSS = dic_type_slotsConfigs[(int)MG_Slots_RewardType.SSS];
            MG_Slots_Config _SlotsNull = dic_type_slotsConfigs[(int)MG_Slots_RewardType.Null];
            MG_Slots_Config _SlotsSS_Other = dic_type_slotsConfigs[(int)MG_Slots_RewardType.SS_Other];


            MG_Slots_RewardType RewardGold(out int needNum)
            {
                needNum = (int)(useNum * UnityEngine.Random.Range(_SlotsGold.rewardPercentRangeMin, _SlotsGold.rewardPercentRangeMax));
                return MG_Slots_RewardType.Gold;
            };
            MG_Slots_RewardType RewardCash(out int needNum)
            {
                //needNum = (int)(useNum * UnityEngine.Random.Range(_SlotsCash.rewardPercentRangeMin, _SlotsCash.rewardPercentRangeMax));
                needNum = UnityEngine.Random.Range(5, 11);
                return MG_Slots_RewardType.Cash;
            };
            MG_Slots_RewardType RewardDiamond(out int needNum)
            {
                needNum = (int)(useNum * UnityEngine.Random.Range(_SlotsDimaond.rewardPercentRangeMin, _SlotsDimaond.rewardPercentRangeMax));
                return MG_Slots_RewardType.Diamond;
            };
            MG_Slots_RewardType RewardGift(out int needNum)
            {
                needNum = 1;
                return MG_Slots_RewardType.Gift;
            };
            MG_Slots_RewardType RewardCherry(out int needNum)
            {
                needNum = 1;
                return MG_Slots_RewardType.Cherry;
            };
            MG_Slots_RewardType RewardOrange(out int needNum)
            {
                needNum = 1;
                return MG_Slots_RewardType.Orange;
            };
            MG_Slots_RewardType RewardWatermalen(out int needNum)
            {
                needNum = 1;
                return MG_Slots_RewardType.Watermalen;
            };
            MG_Slots_RewardType RewardSSS(out int needNum)
            {
                needNum = 0;
                return MG_Slots_RewardType.Null;
            };
            MG_Slots_RewardType RewardSS_Other(out int needNum)
            {
                needNum = 0;
                return MG_Slots_RewardType.SS_Other;
            };
            MG_Slots_RewardType RewardNull(out int needNum)
            {
                needNum = 0;
                return MG_Slots_RewardType.Null;
            };

            bool packB = Get_Save_PackB();
            if (packB)
            {
                int currentSlotsNum = MG_SaveManager.SlotsTotalPlayTimes;
                foreach (int index in _SlotsGold.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardGold(out num);
                    }
                }
                foreach (int index in _SlotsCash.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardCash(out num);
                    }
                }
                foreach (int index in _SlotsDimaond.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardDiamond(out num);
                    }
                }
                foreach (int index in _SlotsGift.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardGift(out num);
                    }
                }
                foreach (int index in _SlotsCherry.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardCherry(out num);
                    }
                }
                foreach (int index in _SlotsOrange.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardOrange(out num);
                    }
                }
                foreach (int index in _SlotsWatermalen.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardWatermalen(out num);
                    }
                }
                foreach (int index in _SlotsSSS.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardSSS(out num);
                    }
                }
                foreach (int index in _SlotsSS_Other.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardSS_Other(out num);
                    }
                }
                foreach (int index in _SlotsNull.rewardThisWhereIndex)
                {
                    if (currentSlotsNum == index)
                    {
                        return RewardNull(out num);
                    }
                }
            }




            int total = _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight + _SlotsOrange.weight +
                _SlotsWatermalen.weight + _SlotsSSS.weight + _SlotsNull.weight + _SlotsSS_Other.weight;

            int result = UnityEngine.Random.Range(0, total);
            if (result < _SlotsGold.weight)
            {
                return RewardGold(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight && Get_Save_Cash() < _SlotsCash.maxCanGetValue)
            {
                return RewardCash(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight && Get_Save_Diamond() < _SlotsDimaond.maxCanGetValue)
            {
                return RewardDiamond(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight+ _SlotsDimaond.weight + _SlotsGift.weight && Get_Save_Cash() < _SlotsGift.maxCanGetValue && MG_SaveManager.TodayExtraRewardTimes > 0)
            {
                return RewardGift(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight && Get_Save_Fruits() < _SlotsCherry.maxCanGetValue && packB)
            {
                return RewardCherry(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight + _SlotsOrange.weight && Get_Save_Fruits() < _SlotsOrange.maxCanGetValue && packB)
            {
                return RewardOrange(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight + _SlotsOrange.weight +
                _SlotsWatermalen.weight && Get_Save_Fruits() < _SlotsWatermalen.maxCanGetValue && packB)
            {
                return RewardWatermalen(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight + _SlotsOrange.weight +
                _SlotsWatermalen.weight + _SlotsSSS.weight)
            {
                return RewardSSS(out num);
            }
            if (result < _SlotsGold.weight + _SlotsCash.weight + _SlotsDimaond.weight + _SlotsGift.weight + _SlotsCherry.weight + _SlotsOrange.weight +
                _SlotsWatermalen.weight + _SlotsSSS.weight + _SlotsNull.weight)
            {
                return RewardNull(out num);
            }
            return RewardSS_Other(out num);
        }
        public void Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType rewardType,int rewardNum)
        {
            MG_PopDoublePanel_Type = rewardType;
            MG_PopDoublePanel_Num = rewardNum;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.DoublePanel);
        }
        public void Show_PopCashPanel_Reward(int rewardNum)
        {
            MG_PopCashPanel_Num = rewardNum;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.CashoutPanel);
        }
        public int MG_SignRewardNum = 0;
        public float MG_SignRewardMutiple = 1;
        public void Show_SignRewardPanel_Reward(MG_PopRewardPanel_RewardType _rewardType,int _rewardNum,float _rewardMutiple)
        {
            MG_PopDiceReward_Type = _rewardType;
            MG_SignRewardNum = _rewardNum;
            MG_SignRewardMutiple = _rewardMutiple;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ExtraRewardPanel);
        }
        public void Show_PopTipsPanel(string content, float time = 1)
        {
            str_Tips = content;
            time_Tips = time;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.Tips);
        }

        public void Play_Effect()
        {
            ps_effectL.Play(true);
            ps_effectR.Play(true);
        }
        public int Get_OfflineDiceLifeAndNextRevertTime(out int nextNeedseconds)
        {
            System.DateTime now = System.DateTime.Now;
            System.TimeSpan interval = now - MG_SaveManager.LastRevertEnergyDate;
            int total = (int)interval.TotalSeconds;
            int leftseconds = total % MG_SaveManager.RevertDiceLifeTimePer;
            int offlineAddEnergy = total / MG_SaveManager.RevertDiceLifeTimePer;
            MG_SaveManager.DiceLife += offlineAddEnergy;
            if (offlineAddEnergy > 0)
                MG_SaveManager.LastRevertEnergyDate = now.AddSeconds(-leftseconds);
            nextNeedseconds = MG_SaveManager.RevertDiceLifeTimePer - leftseconds;
            return MG_SaveManager.DiceLife;
        }
        public int AddEnergyNatural(int value = 1)
        {
           MG_SaveManager.DiceLife += value;
            MG_SaveManager.LastRevertEnergyDate = DateTime.Now;
            return MG_SaveManager.DiceLife;
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
        public void SendAdjustPlayAdEvent(bool hasAd, bool isRewardAd, string adByWay)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
                //累计美元
                ("value", Get_Save_Cash().ToString()),
                //累计金币
                ("new_value", Get_Save_Gold().ToString()),
                //累计体力
                ("stage_id", Get_Save_TotalTimes().ToString()),
                //广告描述
                ("id", adByWay),
                //广告类型，0插屏1奖励视频
                ("type", isRewardAd ? "1" : "0"),
                //累计钻石
                ("other_int1", Get_Save_Diamond().ToString()),
                //累计777
                ("other_int2",Get_Save_777().ToString()),
                //累计亚马逊
                ("other_str1", Get_Save_Amazon().ToString()),
                //累计水果
                ("other_str2",Get_Save_Fruits().ToString())
                );
        }
        public void SendAdjustDiceEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
                //累计美元
                ("value", Get_Save_Cash().ToString()),
                //累计金币
                ("new_value", Get_Save_Gold().ToString()),
                //总游戏次数
                ("stage_id",Get_Save_TotalTimes().ToString() ),
                //第几次骰子
                ("id", Get_Save_DiceTotalTimes().ToString()),
                //累计钻石
                ("other_int1", Get_Save_Diamond().ToString()),
                //累计777
                ("other_int2", Get_Save_777().ToString()),
                //累计亚马逊
                ("other_str1", Get_Save_Amazon().ToString()),
                //累计水果
                ("other_str2", Get_Save_Fruits().ToString())
                );
        }
        public void SendAdjustWheelEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_wheel,
                //累计美元
                ("value", Get_Save_Cash().ToString()),
                //累计金币
                ("new_value", Get_Save_Gold().ToString()),
                //总游戏次数
                ("stage_id",Get_Save_TotalTimes().ToString() ),
                //第几次转盘
                ("id", Get_Save_WheelTotalTimes().ToString()),
                //累计钻石
                ("other_int1", Get_Save_Diamond().ToString()),
                //累计777
                ("other_int2", Get_Save_777().ToString()),
                //累计亚马逊
                ("other_str1", Get_Save_Amazon().ToString()),
                //累计水果
                ("other_str2", Get_Save_Fruits().ToString())
                );
        }
        public void SendAdjustSlotsEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_slots,
                //累计美元
                ("value", Get_Save_Cash().ToString()),
                //累计金币
                ("new_value", Get_Save_Gold().ToString()),
                //总游戏次数
                ("stage_id",Get_Save_TotalTimes().ToString() ),
                //第几次老虎机
                ("id", Get_Save_SlotsTotalTimes().ToString()),
                //累计钻石
                ("other_int1", Get_Save_Diamond().ToString()),
                //累计777
                ("other_int2", Get_Save_777().ToString()),
                //累计亚马逊
                ("other_str1", Get_Save_Amazon().ToString()),
                //累计水果
                ("other_str2", Get_Save_Fruits().ToString())
                );
        }
        public void SendAdjustScratchEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_ggl,
                //累计美元
                ("value", Get_Save_Cash().ToString()),
                //累计金币
                ("new_value", Get_Save_Gold().ToString()),
                //总游戏次数
                ("stage_id",Get_Save_TotalTimes().ToString() ),
                //第几次刮刮乐
                ("id", Get_Save_ScratchTotalTimes().ToString()),
                //累计钻石
                ("other_int1", Get_Save_Diamond().ToString()),
                //累计777
                ("other_int2", Get_Save_777().ToString()),
                //累计亚马逊
                ("other_str1", Get_Save_Amazon().ToString()),
                //累计水果
                ("other_str2", Get_Save_Fruits().ToString())
                );
        }
        public void SendFBAttributeEvent(string uri)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_deeplink,
                ("link", uri),
                ("order_id", uri)
                );
        }


        public static string Get_CashShowText(int cashValue)
        {
            cashValue = Mathf.Abs(cashValue);
            if (cashValue < 10)
                return "0.0" + cashValue;
            else if (cashValue < 100)
                return "0." + cashValue;
            else
            {
                string cashStr = cashValue.ToString();
                return cashStr.Insert(cashStr.Length - 2, ".");
            }
        }
        public static AudioSource Play_ButtonClick()
        {
            return MG_AudioManager.Instance.PlayOneShot(MG_PlayAudioType.Button);
        }
        public static AudioSource Play_SpinDice()
        {
            return MG_AudioManager.Instance.PlayOneShot(MG_PlayAudioType.SpinDice);
        }
        public static AudioSource Play_SpinSlots()
        {
            return MG_AudioManager.Instance.PlayLoop(MG_PlayAudioType.SpinSlots);
        }
        public static AudioSource Play_FlyOver()
        {
            return MG_AudioManager.Instance.PlayOneShot(MG_PlayAudioType.Fly);
        }
        public static bool ShowRV(Action callback,int clickTime,string des)
        {
            return Ads._instance.ShowRewardVideo(callback, clickTime,des);
        }
        public static void ShowIV(Action callback, string des)
        {
            Ads._instance.ShowInterstialAd(callback, des);
        }
    }
    public enum MG_SpecialTokenType
    {
        ScratchToken,
        SlotsToken,
        DiceToken,
        Null
    }
    public enum MG_PopRewardPanel_RewardType
    {
        Cash,
        Gold,
        Extra,
        ExtraCash,
        ExtraGold,
        SignGold,
        SignCash,
    }
    public enum MG_PopDoublePanel_RewardType
    {
        Gold,
        Diamond,
        Cherry,
        Orange,
        Watermalen,
        SSS,
        Scratch,
        ScratchTicket,
        Amazon,
        WheelTicket,
        SignScratchTicket,
        TypeNum,
    }
    public enum MG_Guid_Type
    {
        Null,
        DiceGuid,
        ScratchGuid,
        SlotsGuid
    }
}
