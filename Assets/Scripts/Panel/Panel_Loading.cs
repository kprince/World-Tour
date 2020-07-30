using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Panel_Loading : PanelBase
{
    public Slider loadSlider;
    public Text loadNum;
    public Transform bg;
    protected override void Awake()
    {
        loadSlider.value = 0;
        StartCoroutine(LoadResource());
    }
    IEnumerator LoadResource()
    {
        Coroutine getCor = null;
        if(!GameManager.Instance.GetShowExchange())
            getCor= StartCoroutine(WaitFor());
        int progress = 0;
        int speed = 1;
        float maxWaitTime = 5;
        while (true)
        {
            yield return null;
            if (loadSlider.value < 1)
            {
                progress += 1*speed;
                progress = Mathf.Clamp(progress, 0, 1000);
                loadSlider.value = progress * 0.001f;
                loadNum.text = progress/10 + "%";
                if (GameManager.Instance.GetShowExchange())
                    speed = 10;
                maxWaitTime -= Time.deltaTime/ Time.timeScale;
                if (maxWaitTime <= 0)
                    speed = 50;
            }
            else
            {
                break;
            }
        }
        GameManager.Instance.loadEnd = true;
        if(getCor is object)
            StopCoroutine(getCor);
        Close();
    }
    IEnumerator WaitFor()
    {
#if UNITY_EDITOR
        yield break;
#endif
#if UNITY_ANDROID
        UnityWebRequest webRequest = new UnityWebRequest("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package=com.LuckyDice.CashTycoon.IdleGame.LowPolyCausalGame.WorldTour&version=6&os=android");
#elif UNITY_IOS
            UnityWebRequest webRequest = new UnityWebRequest("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package=com.LuckyDice.CashTycoon.IdleGame.LowPolyCausalGame.WorldTour&version=6&os=ios");
#endif
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode == 200)
        {
            if (webRequest.downloadHandler.text.Equals("{\"store_review\": true}"))
                if (!GameManager.Instance.loadEnd)
                    GameManager.Instance.SetShowExchange(true);
        }
    }
    protected override void Close()
    {
        base.Close();
        PanelManager.Instance.ClosePanel(PanelType.Loading);
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
        PanelManager.Instance.ShowPanel(PanelType.Game);
        Destroy(gameObject);
    }
}
