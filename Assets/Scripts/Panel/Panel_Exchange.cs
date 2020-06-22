using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Exchange : PanelBase
{
    public Text num;
    public Text notice;
    public Toggle tog_cash;
    public Toggle tog_gold;
    public GameObject go_tip;
    public Image img_icon;
    public List<Button> allRedeemBtns = new List<Button>();
    public List<Image> allRedeemImg = new List<Image>();
    SpriteAtlas exchangeAltas;
    Sprite goldSprite = null;
    Sprite cashSprite = null;
    bool isGold = false;
    protected override void Awake()
    {
        base.Awake();
        foreach(Button btn in allRedeemBtns)
        {
            btn.onClick.AddListener(OnRedeemClick);
        }
        if (exchangeAltas is null)
            exchangeAltas = Resources.Load<SpriteAtlas>("ExchangePanel");
        if (goldSprite is null)
            goldSprite = exchangeAltas.GetSprite("gold");
        if (cashSprite is null)
            cashSprite = exchangeAltas.GetSprite("cash");
        int count = allRedeemImg.Count;
        for(int i = 0; i < count; i++)
        {
            allRedeemImg[i].sprite = exchangeAltas.GetSprite((count - i).ToString());
        }
        tog_gold.onValueChanged.AddListener((value) => 
        { 
            isGold = value;
            string showNum = string.Empty;
            if (isGold)
            {
                notice.text = "Reach 5b Gold to redeem the gift card!";
                img_icon.sprite = goldSprite;
                showNum = GameManager.Instance.GetGold().ToString();
            }
            else
            {
                notice.text = "Reach $100 USD to redeem the gift card!";
                img_icon.sprite = cashSprite;
                int cash = GameManager.Instance.GetCash();
                showNum = cash.ToString();
                if (cash < 10)
                    showNum = showNum.Insert(0, "0.0");
                else if (cash < 100)
                    showNum = showNum.Insert(0, "0.");
                else
                    showNum = showNum.Insert(showNum.Length - 2, ".");
            }
            num.text = showNum;
        });
    }
    private void OnEnable()
    {
        go_tip.SetActive(false);
        string showNum;
        if (isGold)
            showNum = GameManager.Instance.GetGold().ToString();
        else
        {
            int cash = GameManager.Instance.GetCash();
            showNum = cash.ToString();
            if (cash < 10)
                showNum = showNum.Insert(0, "0.0");
            else if (cash < 100)
                showNum = showNum.Insert(0, "0.");
            else
                showNum = showNum.Insert(showNum.Length - 2, ".");
        }
        num.text = showNum;
        img_icon.sprite = isGold ? goldSprite : cashSprite;
    }
    private void OnDisable()
    {
        StopCoroutine("delayHideTip");
    }
    void OnRedeemClick()
    {
        go_tip.SetActive(true);
        StopCoroutine("delayHideTip");
        StartCoroutine("delayHideTip");
    }
    IEnumerator delayHideTip()
    {
        yield return new WaitForSeconds(2);
        go_tip.SetActive(false);
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
    protected override void Close()
    {
        base.Close();
        PanelManager.Instance.ClosePanel(PanelType.Exchange);
    }
}
