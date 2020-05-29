using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Notice : PanelBase
{
    public Text text_msg;
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        text_msg.text = GameManager.Instance.notice;
    }
    public override void OnExit()
    {
        base.OnExit();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        if (GameManager.Instance.canGetExtraBonus)
        {
            GameManager.Instance.GetExtraBonus();
            GameManager.Instance.canGetExtraBonus = false;
        }
    }
    protected override void Close()
    {
        base.Close();
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ClosePanel(PanelType.Notice);
    }
}
