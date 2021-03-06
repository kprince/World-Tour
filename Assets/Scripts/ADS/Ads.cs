﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
#if UNITY_ANDROID
	private const string APP_KEY = "c9e51615";
#elif UNITY_IOS
	private const string APP_KEY = "c9e4dc8d";
#endif
	public static Ads _instance;
	public string adDes = string.Empty;
	public const string AppName = "A018_DiceCar";
	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		//Dynamic config example
		IronSourceConfig.Instance.setClientSideCallbacks(true);

		string id = IronSource.Agent.getAdvertiserId();

		//IronSource.Agent.validateIntegration();

		// SDK init
		IronSource.Agent.init(APP_KEY);
		IronSource.Agent.loadInterstitial();

	}
	public bool ShowRewardVideo(Action callback,int clickTime,string des)
    {
		rewardCallback = callback;
		adDes = des;
#if UNITY_EDITOR
		canGetReward = true;
		InvokeGetRewardMethod();
		Debug.Log("Show " + des + " of rewarded AD.");
		return true;
#elif UNITY_IOS
		if (!GameManager.Instance.GetShowExchange())
		{
			canGetReward = true;
			InvokeGetRewardMethod();
			return true;
		}
#endif
		return ShowRewardVideo(clickTime);
    }
	bool ShowRewardVideo(int clickAdTime)
	{
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
			return true;
		}
		else
		{
			StartCoroutine(WaitLoadAD(true,clickAdTime));
			return false;
		}
	}
	Action interstitialCallback = null;
	public void ShowInterstialAd(Action callback,string des)
    {
		adDes = des;
		interstitialCallback = callback;
#if UNITY_EDITOR
		InvokeInterstialCallback();
		Debug.Log("Show " + des + " of intersititial ad.");
		return;
#endif
		ShowInterstialAd();
	}
	float interstialLasttime = 0;
	void ShowInterstialAd()
	{
#if UNITY_IOS
		if (!GameManager.Instance.GetShowExchange()) 
		{
			InvokeInterstialCallback();
			return;
		}
#endif
		if (Time.realtimeSinceStartup - interstialLasttime < 30)
		{
			InvokeInterstialCallback();
			return;
		}
		if (IronSource.Agent.isInterstitialReady())
		{
			interstialLasttime = Time.realtimeSinceStartup;
			IronSource.Agent.showInterstitial();
		}
		else
		{
			InvokeInterstialCallback();
			GameManager.Instance.SendAdjustPlayAdEvent(false, false, adDes);
		}
	}
	void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}
	public GameObject notice;
	const string text = "No Video is ready , please try again later.";
	public void ShowNotice()
	{
		notice.SetActive(true);
		notice.GetComponentInChildren<Text>().text = text;
		StartCoroutine(AutoHideNotice(2));
	}
	IEnumerator WaitLoadAD(bool isRewardedAd,int clickAdTime)
	{
		notice.SetActive(true);
		StringBuilder content = new StringBuilder("Loading.");
		Text noticeText = notice.GetComponentInChildren<Text>();
		noticeText.text = content.ToString();
		int timeOut = 6;
		while (timeOut > 0)
		{
			yield return new WaitForSeconds(Time.timeScale);
			timeOut--;
			content.Append('.');
			noticeText.text = content.ToString();
			if (isRewardedAd && IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
				notice.SetActive(false);
				yield break;
			}
		}
		GameManager.Instance.SendAdjustPlayAdEvent(false, true, adDes);
		if (clickAdTime >= 2)
		{
			PanelManager.Instance.CloseTopPanel();
			noticeText.text = text;
			yield return new WaitForSeconds(2 * Time.timeScale);
		}
		notice.SetActive(false);
	}
	IEnumerator AutoHideNotice(float time)
	{
		yield return new WaitForSeconds(time * Time.timeScale);
		notice.SetActive(false);
	}
	Action rewardCallback;
	public void SetRewardedCallBack(Action method)
	{
		rewardCallback = method;
	}
	private bool canGetReward = false;
	public void GetReward()
	{
		canGetReward = true;
	}
	public void InvokeGetRewardMethod()
	{
		if (canGetReward)
		{
			if (rewardCallback is object)
			{
				rewardCallback();
				rewardCallback = null;
			}
			canGetReward = false;
		}
	}
	public void InvokeInterstialCallback()
    {
		if(interstitialCallback is object)
        {
			interstitialCallback();
			interstitialCallback = null;
        }
    }
}
//IOS FB ID：859639054525170
//安卓 FB ID：650086569195157
