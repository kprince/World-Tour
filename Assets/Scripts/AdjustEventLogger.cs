﻿using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdjustEventLogger : MonoBehaviour
{
    public const string APP_TOKEN = "stg63h4jumtc";
    public const string TOKEN_open = "outopv";
    public const string TOKEN_ad = "9jhkm5";
    public const string TOKEN_noads = "12bgiw";
    public const string TOKEN_stage_end = "g53a9y";
    public const string TOKEN_deeplink = "95sha9";
    public static AdjustEventLogger Instance;
    bool sandbox = true;
    private void Awake()
    {
        Instance = this;
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Sandbox);
        adjustConfig.sendInBackground = true;
        //adjustConfig.setAttributionChangedDelegate(OnAttributionChangedCallback);
        Adjust.start(adjustConfig);
    }
    private void Start()
    {
        GetAdID();
        StartCoroutine(CheckAttributeTo());
    }
    public void AdjustEventNoParam(string token)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        Adjust.trackEvent(adjustEvent);
    }
    public void AdjustEvent(string token, params (string key, string value)[] list)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        foreach (var (key, value) in list)
        {
            adjustEvent.addCallbackParameter(key, value);
        }
        Adjust.trackEvent(adjustEvent);
    }
    void OnAttributionChangedCallback(AdjustAttribution attribution)
    {
        if (attribution.network.Equals("Organic"))
        {

        }
        else
        {
            //GameManager.Instance.SendFBAttributeEvent();
        }
    }
    private string AppName = "A013_dice";
    private string ifa;
    private IEnumerator CheckAttributeTo()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(ifa));
        string url = string.Format("http://a.mobile-mafia.com:3838/adjust/is_unity_clicked?ifa={0}&app_name={1}", ifa, AppName);

        var web = new UnityWebRequest(url);
        web.downloadHandler = new DownloadHandlerBuffer();
        yield return web.SendWebRequest();
        if (web.responseCode == 200)
        {
            if (web.downloadHandler.text.Equals("1"))
            {
#if UNITY_ANDROID
                //SetToB();
#endif
                GameManager.Instance.SendFBAttributeEvent();
            }
            PlayerPrefs.SetInt("GetUnityAttributeTo", 1);
        }
    }
    private void GetAdID()
    {
#if UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               ifa = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
        //ifa = Getidfa();
#endif
    }
}
