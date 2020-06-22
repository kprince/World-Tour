using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Game : PanelBase
{
    public Button btn_Roll;
    public Button btn_Setting;
    public Button btn_Signin;
    public GameObject go_signRedpoint;
    public Button btn_Gold;
    public Text text_Gold;
    public Button btn_Cash;
    public Text text_Cash;
    public Text text_Bonus;
    public Slider sld_Energy;
    public Text text_Energy;
    public Text text_NextEnergy;
    public Image img_cashIcon;
    private int nextEnergyTime;
    private SpriteAtlas gameAltas;
    protected override void Awake()
    {
        btn_Roll.onClick.AddListener(OnRollClick);
        btn_Setting.onClick.AddListener(OnSettingClick);
        btn_Signin.onClick.AddListener(OnSignInClick);
        btn_Gold.onClick.AddListener(OnGoldClick);
        btn_Cash.onClick.AddListener(OnCashClick);
        if (gameAltas is null)
            gameAltas = Resources.Load<SpriteAtlas>("GamePanel");

        GameManager.Instance.goldTrans = text_Gold.transform;
        GameManager.Instance.cashTrans = text_Cash.transform;
        go_signRedpoint.SetActive(GameManager.Instance.CheckCanSignin());
        text_Gold.text = GameManager.Instance.GetGold().ToString();
        int currentCash = GameManager.Instance.GetCash();

        string cashString = currentCash.ToString();
        if (currentCash < 10)
        {
            cashString = cashString.Insert(0, "0.0");
        }
        else if (currentCash < 100)
        {
            cashString = cashString.Insert(0, "0.");
        }
        else
            cashString = cashString.Insert(cashString.Length - 2, ".");
        text_Cash.text = cashString;

        text_Bonus.text = "Bonus in <color=#ff4b4b>" + GameManager.Instance.GetTimeToBonus() + "</color> rolls";
        int offlineEnergy = GameManager.Instance.GetOfflineEnergyAndNextRevertTime(out nextEnergyTime);
        text_Energy.text = offlineEnergy + "/" + SaveManager.PLAYER_MAXENERGY;
        sld_Energy.value = (float)offlineEnergy / SaveManager.PLAYER_MAXENERGY;
        text_NextEnergy.text = "1 ROLL IN " + (nextEnergyTime / 60) + ":" + (nextEnergyTime % 60);
        StartCoroutine(TimeClock());
    }
    void OnRollClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (!GameManager.Instance.canRollDice) return;
        if (GameManager.Instance.ReduceEnergy(1))
        {
            GameManager.Instance.ReduceTimeToBonus(1);
            text_Bonus.text = "Bonus in <color=#ff4b4b>" + GameManager.Instance.GetTimeToBonus() + "</color> rolls";
            int currentEnergy = GameManager.Instance.GetEnergy();
            text_Energy.text = currentEnergy + "/" + SaveManager.PLAYER_MAXENERGY;
            sld_Energy.value = (float)currentEnergy / SaveManager.PLAYER_MAXENERGY;
            GameManager.Instance.RollDice();
            AudioManager.Instance.PlayerSound("Dice");
        }
        else
        {
            PanelManager.Instance.ShowPanel(PanelType.BuyEnergy);
        }
    }
    void OnSettingClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (!GameManager.Instance.canRollDice) return;
        PanelManager.Instance.ShowPanel(PanelType.Setting);
    }
    void OnSignInClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (!GameManager.Instance.canRollDice) return;
        PanelManager.Instance.ShowPanel(PanelType.Signin);
    }
    void OnGoldClick()
    {
        if (!GameManager.Instance.GetShowExchange()) return;
        if (!GameManager.Instance.canRollDice) return;
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ShowPanel(PanelType.Exchange);
    }
    void OnCashClick()
    {
        if (!GameManager.Instance.GetShowExchange()) return;
        if (!GameManager.Instance.canRollDice) return;
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ShowPanel(PanelType.Exchange);
    }
    IEnumerator TimeClock()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            if (nextEnergyTime == 1)
            {
                int currentEnergy = GameManager.Instance.AddEnergyNatural();
                nextEnergyTime = SaveManager.PLAYER_SECOND;
                text_Energy.text = currentEnergy + "/" + SaveManager.PLAYER_MAXENERGY;
            }
            else
                nextEnergyTime--;
            text_NextEnergy.text = "1 ROLL IN " + (nextEnergyTime / 60) + ":" + (nextEnergyTime % 60);
        }
    }
    public void RefreshGold()
    {
        text_Gold.text = GameManager.Instance.GetGold().ToString();
    }
    public void RefreshCash()
    {
        int currentCash = GameManager.Instance.GetCash();

        string cashString = currentCash.ToString();
        if (currentCash < 10)
        {
            cashString = cashString.Insert(0, "0.0");
        }
        else if (currentCash < 100)
        {
            cashString = cashString.Insert(0, "0.");
        }
        else
            cashString = cashString.Insert(cashString.Length - 2, ".");
        text_Cash.text = cashString;
    }
    public void RefreshEnergy()
    {
        int currentEnergy = GameManager.Instance.GetEnergy();
        text_Energy.text = currentEnergy + "/" + SaveManager.PLAYER_MAXENERGY;
        sld_Energy.value = (float)currentEnergy / SaveManager.PLAYER_MAXENERGY;
    }
    public void RefreshSigninState()
    {
        go_signRedpoint.SetActive(GameManager.Instance.CheckCanSignin());
    }
    public override void OnResume()
    {
        base.OnResume();
        go_signRedpoint.SetActive(GameManager.Instance.CheckCanSignin());
        if (GameManager.Instance.GetNextSigninDay() > 6)
        {
            btn_Signin.gameObject.SetActive(false);
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (GameManager.Instance.CheckFirstSignin(true))
        {
            GameManager.Instance.GetExtraBonus();
        }
        img_cashIcon.sprite = GameManager.Instance.GetShowExchange() ?
            gameAltas.GetSprite("cashB") : gameAltas.GetSprite("cashA");
        GameManager.Instance.SetCashBrickTex();
        if (GameManager.Instance.GetNextSigninDay() > 6)
        {
            btn_Signin.gameObject.SetActive(false);
        }
    }
}
