﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Panel_Loading : PanelBase
{
    public Slider loadSlider;
    public Text loadNum;
    public Transform diceIcon;
    public Transform bg;
    public GameObject title;
    protected override void Awake()
    {
        loadSlider.value = 0;
        StartCoroutine(LoadResource());
    }
    IEnumerator AutoRotateDice()
    {
        while (true)
        {
            yield return null;
            diceIcon.Rotate(Vector3.forward * Time.deltaTime * 500);
        }
    }
    IEnumerator LoadResource()
    {
        float time = 0.5f;
#if UNITY_IOS
        Coroutine getCor = StartCoroutine(WaitFor());
#endif
        Coroutine dicCor = StartCoroutine(AutoRotateDice());
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
                maxWaitTime -= Time.deltaTime;
                if (maxWaitTime <= 0)
                    speed = 50;
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    if (title.activeSelf)
                        title.SetActive(false);
                    if (loadSlider.gameObject.activeSelf)
                        loadSlider.gameObject.SetActive(false);
                    if (diceIcon.gameObject.activeSelf)
                        diceIcon.gameObject.SetActive(false);
                    bg.localScale -= Vector3.one * Time.deltaTime;
                    if (bg.localScale.x < 0.775f)
                        break;
                }
            }
        }
        GameManager.Instance.loadEnd = true;
        StopCoroutine(dicCor);
#if UNITY_IOS
        StopCoroutine(getCor);
#endif
        Close();
    }
    IEnumerator WaitFor()
    {
        if (GameManager.Instance.GetShowExchange()) yield break;
        UnityWebRequest webRequest = new UnityWebRequest("dice1.fengwan8.com");
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode == 200)
        {
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
