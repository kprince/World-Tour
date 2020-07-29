using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_RateUs : PanelBase
{
    public Button btn_yes;
    protected override void Awake()
    {
        base.Awake();
        btn_yes.onClick.AddListener(RateUs);
    }
    void RateUs()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.LuckyDice.CashTycoon.IdleGame.LowPolyCausalGame.WorldTour");
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
           "1520171235");
        Application.OpenURL(url);
#endif
        PanelManager.Instance.ClosePanel(PanelType.RateUs);
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
        GameManager.Instance.needRateUs = false;
        if (GameManager.Instance.needShowMGGuid)
        {
            GameManager.Instance.needShowMGGuid = false;
            MiddleGround.MG_Manager.ShowMGGuid();
        }
    }
    protected override void Close()
    {
        base.Close();
        PanelManager.Instance.ClosePanel(PanelType.RateUs);
    }
}
