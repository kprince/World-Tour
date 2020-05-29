using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Signin : PanelBase
{
    public Image[] img_allDailyBg = new Image[7];
    public GameObject[] go_allDailyTog = new GameObject[7];
    public Text[] text_allDailyNum = new Text[7];
    float[] rewards = new float[7] { 2000, 1, 2000, 2000, 1, 1, 5 };
    bool[] isGold = new bool[7] { true, false, true, true, false, false, false };
    float[] rewardmutiples = new float[7] { 3, 1.5f, 1.5f, 5, 1.5f, 1.5f, 1 };
    public Transform todayEffect;
    public Transform handle;
    public Button btn_get;
    Sprite hasgetBg = null;
    Sprite ungetBg = null;
    SpriteAtlas signinAltas;
    protected override void Awake()
    {
        base.Awake();
        if (signinAltas is null)
            signinAltas = Resources.Load<SpriteAtlas>("SigninPanel");
        if (hasgetBg is null)
            hasgetBg = signinAltas.GetSprite("hasgetbg");
        if (ungetBg is null)
            ungetBg = signinAltas.GetSprite("ungetbg");
        for(int i = 0; i < 7; i++)
        {
            text_allDailyNum[i].text = rewards[i].ToString();
        }
        btn_get.onClick.AddListener(OnGetClick);
    }
    private void OnEnable()
    {
        int nextSigninDay = GameManager.Instance.GetNextSigninDay();
        for(int i = 0; i < 7; i++)
        {
            if (i < nextSigninDay)
            {
                img_allDailyBg[i].sprite = hasgetBg;
                go_allDailyTog[i].SetActive(true);
            }
            else if (i == nextSigninDay)
            {
                todayEffect.SetParent(img_allDailyBg[i].transform);
                todayEffect.localPosition = Vector3.zero;
                todayEffect.SetAsFirstSibling();
                img_allDailyBg[i].sprite = ungetBg;
                go_allDailyTog[i].SetActive(false);
            }
            else
            {
                img_allDailyBg[i].sprite = ungetBg;
                go_allDailyTog[i].SetActive(false);
            }
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        isRandom = false;
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
        AudioManager.Instance.PlayerSound("Button");
        if (isRandom)
            return;
        PanelManager.Instance.ClosePanel(PanelType.Signin);
    }
    bool isRandom = false;
    void OnGetClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        if (isRandom) return;
        if (GameManager.Instance.CheckCanSignin())
        {
            StartCoroutine(StartRandom());
            isRandom = true;
        }
        else
        {
            GameManager.Instance.notice = "You have signed!";
            PanelManager.Instance.ShowPanel(PanelType.Notice);
        }
    }
    float[] posX = new float[5] { -407, -201.7f, 0, 201, 408 };
    float[] mutiples = new float[5] { 1, 1.5f, 2, 3, 5 };
    float minX = -499;
    float maxX = 495;
    IEnumerator StartRandom()
    {
        AudioSource spinAS = AudioManager.Instance.PlayerSoundLoop("Spin");
        int result = 0;
        int nextDay = GameManager.Instance.GetNextSigninDay();
        float nextDayMutiply = rewardmutiples[nextDay];
        for(int i = 0; i < 5; i++)
        {
            if(nextDayMutiply == mutiples[i])
            {
                result = i;
                break;
            }
        }
        int turns = 0;
        float speed = 2000;
        while (true)
        {
            yield return null;
            handle.localPosition += Vector3.left * Time.deltaTime * speed;
            if (handle.transform.localPosition.x <= minX)
            {
                turns++;
                handle.localPosition = new Vector3(maxX, handle.localPosition.y, 0);
                if (turns == 3)
                    speed = 1000;
            }
            if (turns >= 3 && Mathf.Abs(handle.localPosition.x - posX[result]) <= 10)
            {
                break;
            }
        }
        spinAS.Stop();
        int addResult = (int)(rewards[nextDay] * nextDayMutiply);
        if (isGold[nextDay])
            GameManager.Instance.AddGold(addResult);
        else
            GameManager.Instance.AddCash(addResult);
        GameManager.Instance.Signin(DateTime.Now);
        img_allDailyBg[nextDay].sprite = hasgetBg;
        go_allDailyTog[nextDay].SetActive(true);
        todayEffect.SetParent(img_allDailyBg[nextDay].transform);
        todayEffect.localPosition = Vector3.zero;
        todayEffect.SetAsFirstSibling();

        isRandom = false;
    }
}
