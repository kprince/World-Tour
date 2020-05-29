using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRewardedAds : MonoBehaviour
{
    private Ads adM;
    // Start is called before the first frame update
    void Start()
    {
        adM = GetComponent<Ads>();
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
    }

    private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
    {
        GameManager.Instance.SendAdjustPlayAdEvent(false, true, IronSource.Agent.getAdvertiserId());
    }

    private void RewardedVideoAdClickedEvent(IronSourcePlacement obj)
    {

    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        GameManager.Instance.SendAdjustPlayAdEvent(true, true, IronSource.Agent.getAdvertiserId());
        Ads._instance.GetReward();
    }

    private void RewardedVideoAdEndedEvent()
    {

    }

    private void RewardedVideoAdStartedEvent()
    {

    }

    private void RewardedVideoAvailabilityChangedEvent(bool obj)
    {
    }

    private void RewardedVideoAdClosedEvent()
    {

    }

    private void RewardedVideoAdOpenedEvent()
    {

    }
}
