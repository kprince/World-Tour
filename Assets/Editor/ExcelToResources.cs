using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Data;
using Excel;
using System.IO;
using System;
using System.Reflection;
using UnityEngine.AI;

public class ExcelToResources : Editor
{
    private static string xlsxPath;
    [MenuItem("Excel/ReadConfigExcel")]
    static void ReadExcel()
    {
        Config Config = Resources.Load<Config>("Config");
        Config.Range.Clear();
        Config.ExtraBonusConfigs.Clear();
        Config.SpecialPropsConfigs.Clear();
        Config.JackpotConfigConfigs.Clear();

        //在此添加新列表
        xlsxPath = Application.dataPath + "/Config.xlsx";
        if (!File.Exists(xlsxPath))
        {
            Debug.LogError("文件不存在");
            return;
        }
        FileStream fs = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
        DataSet dataSet = reader.AsDataSet();
        reader.Dispose();
        if (dataSet == null)
        {
            Debug.LogError("文件为空!");
            return;
        }
        DataTable firstTable = dataSet.Tables[0];
        int rowsCount = firstTable.Rows.Count;
        for (int i = 2; i < rowsCount; i++)
        {
            //范围
            Config.Range.Add((int)(float.Parse(firstTable.Rows[i][1].ToString()) * 100));
            int configIndex = i - 2;
            ExtraBonusConfig extraBonusConfig = new ExtraBonusConfig
            {
                rangeIndex = configIndex,
                needTargetStep = int.Parse(firstTable.Rows[i][2].ToString()),
                cashBonusRate = float.Parse(firstTable.Rows[i][3].ToString()),
                minCashBonus = int.Parse(firstTable.Rows[i][4].ToString()),
                maxCashBonus =int.Parse(firstTable.Rows[i][5].ToString()),
                goldBonusRate = float.Parse(firstTable.Rows[i][6].ToString()),
                minGoldBonus = int.Parse(firstTable.Rows[i][7].ToString()),
                maxGoldBonus = int.Parse(firstTable.Rows[i][8].ToString()),
            };
            Config.ExtraBonusConfigs.Add(extraBonusConfig);
            SpecialPropsConfig specialPropsConfig = new SpecialPropsConfig
            {
                rangeIndex = configIndex,
                minCashReward = int.Parse(firstTable.Rows[i][9].ToString()),
                maxCashReward = int.Parse(firstTable.Rows[i][10].ToString()),
                minGoldReward = int.Parse(firstTable.Rows[i][12].ToString()),
                maxGoldReward = int.Parse(firstTable.Rows[i][13].ToString())
            };
            specialPropsConfig.cashMutiple = new List<float>();
            string[] cashmutiples = firstTable.Rows[i][11].ToString().Split(';');
            for(int j = 0; j < cashmutiples.Length; j++)
            {
                specialPropsConfig.cashMutiple.Add(float.Parse(cashmutiples[j]));
            }
            specialPropsConfig.goldMutiple = new List<float>();
            string[] goldmutiples = firstTable.Rows[i][14].ToString().Split(';');
            for(int j = 0; j < goldmutiples.Length; j++)
            {
                specialPropsConfig.goldMutiple.Add(float.Parse(goldmutiples[j]));
            }
            Config.SpecialPropsConfigs.Add(specialPropsConfig);
            JackpotConfig jackpotConfig = new JackpotConfig()
            {
                noRewardRate = float.Parse(firstTable.Rows[i][15].ToString()),
                cashRewardRate = float.Parse(firstTable.Rows[i][16].ToString()),
                goldRewardRate = float.Parse(firstTable.Rows[i][18].ToString())
            };
            jackpotConfig.cashPool = new List<int>();
            string[] cashNums = firstTable.Rows[i][17].ToString().Split(';');
            for(int j = 0; j < cashNums.Length; j++)
            {
                jackpotConfig.cashPool.Add(int.Parse(cashNums[j]));
            }
            jackpotConfig.goldPool = new List<int>();
            string[] goldNums = firstTable.Rows[i][19].ToString().Split(';');
            for(int j = 0; j < goldNums.Length; j++)
            {
                jackpotConfig.goldPool.Add(int.Parse(goldNums[j]));
            }
            Config.JackpotConfigConfigs.Add(jackpotConfig);
        }
        Debug.Log("Excel文件读取完成！");
        EditorUtility.SetDirty(Config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
