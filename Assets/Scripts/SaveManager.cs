using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public const int PLAYER_MAXENERGY = 50;
    public const int PLAYER_SECOND = 900;
    string dataPath;
    public PlayerInfo player = null;
    public void Init()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "data");
        Read();
    }
    public void Save()
    {
        FileStream fileStream = new FileStream(dataPath, FileMode.OpenOrCreate, FileAccess.Write);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, player);
        fileStream.Close();
        fileStream.Dispose();
    }
    private void Read()
    {
        if (File.Exists(dataPath))
        {
            FileStream fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            player = binaryFormatter.Deserialize(fileStream) as PlayerInfo;
            fileStream.Close();
            fileStream.Dispose();
        }
        if (player.brickReward.Length < 28)
        {
            DateTime now = DateTime.Now;
            player = new PlayerInfo()
            {
                firstLogin = true,
                cash = 0,
                gold = 0,
                energy = 50,
                totalWasteEnergy = 0,
                gameTimes = 0,
                step = 0,
                sceneIndex = 0,
                stepToGetExtraBonus = GameManager.Instance.GetExtraBonusNeedStep(),
                brickReward = new int[28],
                brickRewardGet = new bool[28],
                nextsigninDay = 0,
                lastRevertEnergyDate = now.AddSeconds(-PLAYER_SECOND + 1),
                lastSigninDate = now.AddDays(-1),
                soundOn = true,
                musicOn = true,
                showExchange = false,
                hasRateUs = false,
            };
        }
    }
}
[System.Serializable]
public class PlayerInfo
{
    public bool firstLogin;
    public int gold;
    public int cash;
    public int energy;
    public int totalWasteEnergy;
    public int gameTimes;
    public int step;
    public int sceneIndex;
    public int stepToGetExtraBonus;
    public int nextsigninDay;
    public DateTime lastSigninDate;
    public DateTime lastRevertEnergyDate;
    public int[] brickReward;
    public bool[] brickRewardGet;
    public bool soundOn;
    public bool musicOn;
    public bool showExchange;
    public bool hasRateUs;
}
