using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Config",menuName = "CreateConfigFile",order = 1001)]
public class Config:ScriptableObject
{
    public List<int> Range = new List<int>();
    public List<ExtraBonusConfig> ExtraBonusConfigs = new List<ExtraBonusConfig>();
    public List<SpecialPropsConfig> SpecialPropsConfigs = new List<SpecialPropsConfig>();
    public List<JackpotConfig> JackpotConfigConfigs = new List<JackpotConfig>();
}
[System.Serializable]
public class ExtraBonusConfig
{
    public int rangeIndex;
    public int needTargetStep;
    public float goldBonusRate;
    public float cashBonusRate;
    public int maxGoldBonus;
    public int minGoldBonus;
    public int maxCashBonus;
    public int minCashBonus;
}
[System.Serializable]
public class SpecialPropsConfig
{
    public int rangeIndex;
    public int maxGoldReward;
    public int minGoldReward;
    public int maxCashReward;
    public int minCashReward;
    public List<float> goldMutiple;
    public List<float> cashMutiple;
}
[System.Serializable]
public class JackpotConfig
{
    public float noRewardRate;
    public float goldRewardRate;
    public float cashRewardRate;
    public List<int> goldPool;
    public List<int> cashPool;
}
