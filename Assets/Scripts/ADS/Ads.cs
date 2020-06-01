using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
	private const string APP_KEY = "c63c67d5";
	public static Ads _instance;
	public string adDes = string.Empty;
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

		IronSource.Agent.validateIntegration();

		// SDK init
		IronSource.Agent.init(APP_KEY);
		IronSource.Agent.loadInterstitial();

	}
	public bool ShowRewardVideo()
	{
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
			return true;
		}
		else
		{
			StartCoroutine(WaitLoadAD(true));
			return false;
		}
	}
	float interstialLasttime = 0;
	public void ShowInterstialAd()
	{
		if (Time.realtimeSinceStartup - interstialLasttime < 30)
			return;
		if (IronSource.Agent.isInterstitialReady())
		{
			interstialLasttime = Time.realtimeSinceStartup;
			IronSource.Agent.showInterstitial();
		}
		else
		{
			GameManager.Instance.SendAdjustPlayAdEvent(false, false, adDes);
		}
	}
	void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}
	public GameObject notice;
	public void ShowNotice(string text="AD Video is not ready!")
	{
		notice.SetActive(true);
		notice.GetComponentInChildren<Text>().text = text;
	}
	IEnumerator WaitLoadAD(bool isRewardedAd)
	{
		notice.SetActive(true);
		notice.GetComponentInChildren<Text>().text = "Loading...";
		yield return new WaitForSeconds(3);
		if (isRewardedAd && IronSource.Agent.isRewardedVideoAvailable())
			IronSource.Agent.showRewardedVideo();
		else
			GameManager.Instance.SendAdjustPlayAdEvent(false, true, adDes);
		notice.SetActive(false);
	}
	Action rewardCallback;
	public void SetRewardedCallBack(Action method)
	{
		rewardCallback = method;
	}
	public void GetReward()
	{
		rewardCallback();
	}
}
