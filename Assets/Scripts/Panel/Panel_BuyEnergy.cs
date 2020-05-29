using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_BuyEnergy : PanelBase
{
    public Button btn_ad;
    const int adEnergy = 5;
    protected override void Awake()
    {
        base.Awake();
        btn_ad.onClick.AddListener(OnAdClick);
    }
    void OnAdClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        GameManager.Instance.AddEnergy(adEnergy);
    }
    protected override void Close()
    {
        base.Close();
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ClosePanel(PanelType.BuyEnergy);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
    public override void OnExit()
    {
        base.OnExit();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
