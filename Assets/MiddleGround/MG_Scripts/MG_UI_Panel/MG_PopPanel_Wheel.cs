﻿using MiddleGround.GameConfig;
using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Wheel : MG_UIBase
    {
        public List<Image> img_AllIcons = new List<Image>();
        public List<Text> text_AllTexts = new List<Text>();

        readonly Dictionary<int, Sprite> dic_type_sprite = new Dictionary<int, Sprite>();

        public RectTransform rect_wheel;
        public GameObject go_lock;
        public GameObject go_adicon;
        public Image img_midhandle;
        public Text text_locktime;
        public Text text_wheelticket;
        public Text text_speedup;
        RectTransform rect_speedup;

        public Button btn_Speedup;
        public Button btn_Close;
        MG_Wheel_RewardType[] _Wheel_RewardTypes;
        int[] _Wheel_RewardNums;
        protected override void Awake()
        {
            base.Awake();

            btn_Close.onClick.AddListener(OnCloseClick);
            btn_Speedup.onClick.AddListener(OnSpinClick);
            MG_UIManager.Instance.MenuPanel.dic_flytarget_transform.Add((int)MG_MenuFlyTarget.WheelTicket, text_wheelticket.transform);
            rect_speedup = text_speedup.GetComponent<RectTransform>();

            SpriteAtlas wheelSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.WheelPanel);
            bool packB = MG_Manager.Instance.Get_Save_PackB();
            int typeCount = (int)MG_Wheel_RewardType.TypeNum;
            for(int i = 0; i < typeCount; i++)
            {
                string name = ((MG_Wheel_RewardType)i).ToString();
                if (i == (int)MG_Wheel_RewardType.Cash)
                {
                    name += packB ? "B" : "A";
                }
                dic_type_sprite.Add(i, wheelSA.GetSprite("MG_Sprite_Wheel_" + name));
            }

            _Wheel_RewardTypes = MG_Manager.Instance.Get_Config_WheelReward(out _Wheel_RewardNums);
            if (_Wheel_RewardTypes.Length != img_AllIcons.Count || _Wheel_RewardTypes.Length != text_AllTexts.Count)
            {
                Debug.LogError("Config MG_Wheel Reward Error : config file is not matched in wheel.");
            }
            else
            {
                int totalWheelReward = _Wheel_RewardTypes.Length;
                for(int i = 0; i < totalWheelReward; i++)
                {
                    img_AllIcons[i].sprite = dic_type_sprite[(int)_Wheel_RewardTypes[i]];
                    text_AllTexts[i].text = "x" + GetShowNumString(_Wheel_RewardNums[i]);
                }
            }
        }
        bool isRotating = false;
        int rewardIndex = -1;
        int clickTime = 0;
        void OnSpinClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isRotating) return;
            if (isAdLock)
            {
                clickTime++;
                MG_Manager.ShowRV(AdCallback, clickTime, "wheel unlock");
                return;
            }
            else
            {
                if (MG_Manager.Instance.Get_Save_WheelTickets() <= 0)
                {
                    MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.WheelPanel);
                }
                else
                {
                    isRotating = true;
                    MG_Manager.Instance.SendAdjustWheelEvent();
                    rewardIndex = MG_Manager.Instance.Random_WheelReward();
                    MG_Manager.Instance.Add_Save_WheelTickets(-1);
                    StartCoroutine("WaitRoateEnd");
                }
            }
        }
        void AdCallback()
        {
            clickTime = 0;
            MG_SaveManager.WheelLastLockDate = System.DateTime.Now.AddSeconds(-3601);
            StopCoroutine("WaitForUnlock");
            CheckLock();
        }
        public void UpdateWheelTicketShow()
        {
            text_wheelticket.text = "Ticket x" + MG_Manager.Instance.Get_Save_WheelTickets();
        }
        IEnumerator WaitRoateEnd()
        {
            img_midhandle.color = Color.clear;
            float endAngleZ = (-90 - 30 * rewardIndex) % 360;
            if (endAngleZ < 0)
                endAngleZ += 360;
            float rotateSpeed = 800;
            float rotateBackSpeed = 300;
            float time = 0.6f;
            bool isStop = false;
            bool isBack = false;
            bool isRight = false;
            AudioSource spinAS = MG_Manager.Play_SpinSlots();
            while (!isStop)
            {
                yield return null;
                time -= Time.unscaledDeltaTime;
                if (time <= 0)
                {
                    if (!isRight)
                    {
                        rect_wheel.localEulerAngles = new Vector3(0, 0, endAngleZ);
                        isRight = true;
                    }
                    if (!isBack)
                    {
                        rotateBackSpeed -= 40;
                        rect_wheel.Rotate(new Vector3(0, 0, -Time.unscaledDeltaTime * rotateBackSpeed));
                        if (rotateBackSpeed <= 0)
                            isBack = true;
                    }
                    else
                    {
                        rotateBackSpeed += 20;
                        rect_wheel.Rotate(new Vector3(0, 0, Time.unscaledDeltaTime * rotateBackSpeed));
                        float angleZ = rect_wheel.localEulerAngles.z % 360;
                        if (angleZ < 0)
                            angleZ += 360;
                        if (Mathf.Abs(angleZ - endAngleZ) < 3)
                        {
                            rect_wheel.localEulerAngles = new Vector3(0, 0, endAngleZ);
                            isStop = true;
                        }
                    }
                }
                else
                    rect_wheel.Rotate(new Vector3(0, 0, -Time.unscaledDeltaTime * rotateSpeed));
            }
            spinAS.Stop();
            yield return null;
            img_midhandle.color = Color.white;
            yield return new WaitForSeconds(0.3f * Time.timeScale);
            img_midhandle.color = Color.clear;
            yield return new WaitForSeconds(0.3f * Time.timeScale);
            img_midhandle.color = Color.white;
            yield return new WaitForSeconds(0.3f * Time.timeScale);
            if (MG_Manager.Instance.Get_Save_WheelTotalTimes() % 2 == 0)
                MG_Manager.ShowIV(OnIntersititialCallback, "wheel Interstitial video");
            else
                OnIntersititialCallback();
        }
        void OnIntersititialCallback()
        {
            switch (_Wheel_RewardTypes[rewardIndex])
            {
                case MG_Wheel_RewardType.Gold:
                    MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.Gold, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Cash:
                    MG_Manager.Instance.Show_PopCashPanel_Reward(_Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Scratch:
                    MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.Scratch, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.Gift:
                    MG_Manager.Instance.Random_DiceOrExtraReward(MG_PopRewardPanel_RewardType.Extra);
                    break;
                case MG_Wheel_RewardType.Amazon:
                    MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.Amazon, _Wheel_RewardNums[rewardIndex]);
                    break;
                case MG_Wheel_RewardType.WheelTicket:
                    MG_Manager.Instance.Show_PopDoublePanel_Reward(MG_PopDoublePanel_RewardType.WheelTicket, _Wheel_RewardNums[rewardIndex]);
                    break;
            }
            CheckLock();
            isRotating = false;
        }
        void OnCloseClick()
        {
            MG_Manager.Play_ButtonClick();
            if (isRotating) return;
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.WheelPanel);
        }


        public override IEnumerator OnEnter()
        {
            img_midhandle.color = Color.clear;
            UpdateWheelTicketShow();
            CheckLock();

            while (canvasGroup.alpha < 1)
            {
                yield return null;
                canvasGroup.alpha += Time.unscaledDeltaTime * 4;
            }
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

        public override IEnumerator OnExit()
        {
            while (canvasGroup.alpha > 0)
            {
                yield return null;
                canvasGroup.alpha -= Time.unscaledDeltaTime * 4;
            }
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            MG_UIManager.Instance.UpdateWheelRP();
        }

        public override void OnPause()
        {
            //btn_Close.gameObject.SetActive(false);
        }

        public override void OnResume()
        {
            //btn_Close.gameObject.SetActive(true);
        }
        string GetShowNumString(int num)
        {
            string str = num.ToString();
            if (num < 1000)
                return str;
            else if (num < 1000000)
                str = str.Insert(str.Length - 3, ",");
            return str;
        }
        bool isAdLock = false;
        void CheckLock()
        {
            if (MG_Manager.Instance.Get_Save_WheelTickets() <= 0)
            {
                btn_Speedup.gameObject.SetActive(false);
                if (!go_lock.activeSelf)
                    go_lock.SetActive(true);
                System.DateTime now = System.DateTime.Now;
                int seconds = (23 - now.Hour) * 3600 + (59 - now.Minute) * 60 + 59 - now.Second;
                StopCoroutine("WaitForUnlock");
                StartCoroutine("WaitForUnlock", seconds);
                return;
            }
            else
            {
                btn_Speedup.gameObject.SetActive(true);
            }
            int totalPlayTimes = MG_Manager.Instance.Get_Save_WheelTotalTimes();
            if (totalPlayTimes > 0 && totalPlayTimes % 3 == 0)
            {
                if (totalPlayTimes == MG_SaveManager.WheelLockTimeIndex)
                {
                    System.DateTime lastLockDate = MG_SaveManager.WheelLastLockDate;
                    System.TimeSpan interval = System.DateTime.Now - lastLockDate;
                    if (interval.TotalSeconds >= 3600)
                        isAdLock = false;
                    else
                    {
                        StopCoroutine("WaitForUnlock");
                        StartCoroutine("WaitForUnlock", 3600 - interval.TotalSeconds);
                        isAdLock = true;
                    }
                }
                else
                {
                    isAdLock = true;
                    MG_SaveManager.WheelLockTimeIndex = totalPlayTimes;
                    MG_SaveManager.WheelLastLockDate = System.DateTime.Now;
                    StopCoroutine("WaitForUnlock");
                    StartCoroutine("WaitForUnlock", 3599);
                }
            }
            else
                isAdLock = false;
            if (go_lock.activeSelf != isAdLock)
                go_lock.SetActive(isAdLock);
            if (go_adicon.activeSelf != isAdLock)
                go_adicon.SetActive(isAdLock);
            if (isAdLock)
            {
                text_speedup.text = "SPEED UP";
                rect_speedup.localPosition = new Vector2(55, 8.2782f);
            }
            else
            {
                text_speedup.text = "SPIN";
                rect_speedup.localPosition = new Vector2(0, 8.2782f);
            }
        }
        IEnumerator WaitForUnlock(int seconds)
        {
            if (seconds == 0)
            {
                CheckLock();
                yield break;
            }
            int showHours = seconds / 3600;
            int showMinutes = (seconds % 3600) / 60;
            int showSeconds = seconds % 60;
            text_locktime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            while (seconds > 0)
            {
                yield return new WaitForSeconds(Time.timeScale);
                seconds--;
                if (seconds == 0)
                {
                    CheckLock();
                    yield break;
                }
                showHours = seconds / 3600;
                showMinutes = (seconds % 3600) / 60;
                showSeconds = seconds % 60;
                text_locktime.text = "Unlock after " + (showHours > 9 ? showHours.ToString() : "0" + showHours) + ":" + (showMinutes > 9 ? showMinutes.ToString() : "0" + showMinutes) + ":" + (showSeconds > 9 ? showSeconds.ToString() : "0" + showSeconds);
            }
        }
    }
}
