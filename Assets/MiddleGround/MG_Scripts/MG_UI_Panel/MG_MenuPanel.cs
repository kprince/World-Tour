using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_MenuPanel : MG_UIBase
    {
        public Button btn_Setting;
        public Button btn_Wheel;
        public Button btn_Sign;
        public Button btn_Scratch;
        public Button btn_Dice;
        public Button btn_Slots;
        public Button btn_Gold;
        public Button btn_Cash;
        public Button btn_SpecialToken;
        public Button btn_Back;

        public Text text_Gold;
        public Text text_Cash;
        public Text text_Scratch;
        public Text text_ScratchTicketNum;
        public Text text_Dice;
        public Text text_Slots;
        public Text text_SpecialToken;

        public Image img_SpecialToken;
        public Image img_CashIcon;
        public GameObject go_SpecialToken;

        public Transform trans_SelectGame;
        float selectGameY = 0;
        float diceX = 0;
        float scratchX = 0;
        float slotsX = 0;
        public RectTransform rect_Top;
        public Transform trans_guidMask;
        public Transform trans_guidBase;
        public Image img_guidBG;
        public Image img_guidIcon;
        public Text text_guidDes;

        public GameObject go_wheelRP;
        public GameObject go_signRP;
        public GameObject go_scratchRP;
        public GameObject go_cashoutTips_cash;
        public GameObject go_cashoutTips_special;

        SpriteAtlas MenuAtlas;
        protected override void Awake()
        {
            base.Awake();
            btn_Setting.onClick.AddListener(OnSettingButtonClick);
            btn_Wheel.onClick.AddListener(OnWheelButtonClick);
            btn_Sign.onClick.AddListener(OnSignButtonClick);
            btn_Scratch.onClick.AddListener(OnScratchButtonClick);
            btn_Dice.onClick.AddListener(OnDiceButtonClick);
            btn_Slots.onClick.AddListener(OnSlotsButtonClick);
            btn_Gold.onClick.AddListener(OnGoldButtonClick);
            btn_Cash.onClick.AddListener(OnCashButtonClick);
            btn_SpecialToken.onClick.AddListener(OnSpecialButtonClick);
            btn_Back.onClick.AddListener(MG_Manager.Instance.CloseTopPopPanel);
            trans_guidMask.GetComponent<Button>().onClick.AddListener(OnMaskButtonClick);

            text_Scratch.text = "Scratch";
            text_Dice.text = "Dice";
            text_Slots.text = "Slots";

            float lwr = Screen.height / Screen.width;
            if (lwr > 4 / 3f)
            {
                rect_Top.anchoredPosition = new Vector2(0, -87);
                f_guidY = 600;
            }
            else
                f_guidY = 513;
            trans_guidBase.localPosition = new Vector2(0, f_guidY);

            selectGameY = trans_SelectGame.localPosition.y;
            diceX = btn_Dice.transform.localPosition.x;
            scratchX = btn_Scratch.transform.localPosition.x;
            slotsX = btn_Slots.transform.localPosition.x;


            MenuAtlas = MG_UIManager.Instance.GetMenuSpriteAtlas();
            sp_ScratchToken = MenuAtlas.GetSprite("MG_Sprite_Menu_ScratchToken");
            sp_SlotsToken = MenuAtlas.GetSprite("MG_Sprite_Menu_SlotsToken");
            sp_DiceToken = MenuAtlas.GetSprite("MG_Sprite_Menu_DiceToken");
            packB = MG_Manager.Instance.Get_Save_PackB();
            if (packB)
                img_CashIcon.sprite = MenuAtlas.GetSprite("MG_Sprite_Menu_CashB");
            else
                img_CashIcon.sprite = MenuAtlas.GetSprite("MG_Sprite_Menu_CashA");
            go_cashoutTips_cash.SetActive(packB);
            go_cashoutTips_special.SetActive(packB);

            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.OneGold, btn_Gold.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Cash, btn_Cash.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Amazon, btn_SpecialToken.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.ScratchTicket, btn_Scratch.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.SlotsSpecialToken, btn_SpecialToken.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Scratch, btn_Scratch.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Slots, btn_Slots.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.SSS, btn_SpecialToken.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Orange, btn_Cash.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Cherry, btn_Cash.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Watermalen, btn_Cash.transform);
            dic_flytarget_transform.Add((int)MG_MenuFlyTarget.Diamond, btn_SpecialToken.transform);
            go_scratchRP.SetActive(false);
        }
        bool packB = false;
        void OnSettingButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.SettingPanel);
        }
        void OnWheelButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            go_SpecialToken.SetActive(false);
            MG_SaveManager.Current_GamePanel = (int)MG_PopPanelType.DicePanel;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.WheelPanel);
            UpdateAllContent();
        }
        void OnSignButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.SignPanel);
        }
        public void OnScratchButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            if (go_scratchRP.activeSelf)
                go_scratchRP.SetActive(false);
            trans_SelectGame.localPosition = new Vector3(scratchX, selectGameY);
            MG_SaveManager.Current_GamePanel = (int)MG_PopPanelType.ScratchPanel;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ScratchPanel);
            UpdateAllContent();
        }
        public void OnDiceButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            trans_SelectGame.localPosition = new Vector3(diceX, selectGameY);
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.DicePanel);
            SetSpecialToken(MG_SpecialTokenType.DiceToken);
        }
        public void OnSlotsButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            if (!MG_Manager.Instance.canChangeGame) return;
            trans_SelectGame.localPosition = new Vector3(slotsX, selectGameY);
            MG_SaveManager.Current_GamePanel = (int)MG_PopPanelType.SlotsPanel;
            MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.SlotsPanel);
            UpdateAllContent();
        }
        void OnGoldButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnCashButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnSpecialButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            OnMaskButtonClick();
            if (MG_Manager.Instance.isGuid) return;
            if (!MG_Manager.Instance.canChangeGame) return;
            if (packB)
                MG_UIManager.Instance.ShowPopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void UpdateAllContent()
        {
            UpdateGoldText();
            UpdateCashText();
            UpdateScratchTicketText();
            UpdateSpecialTokenText();
            UpdateWheelRP();
            UpdateSingRP();
        }
        Sprite sp_ScratchToken = null;
        Sprite sp_SlotsToken = null;
        Sprite sp_DiceToken = null;
        void SetSpecialToken(MG_SpecialTokenType _SpecialTokenType)
        {
            switch (_SpecialTokenType)
            {
                case MG_SpecialTokenType.ScratchToken:
                    if(!packB)
                        go_SpecialToken.SetActive(false);
                    else
                    {
                        go_SpecialToken.SetActive(true);
                        img_SpecialToken.sprite = sp_ScratchToken;
                        text_SpecialToken.text = MG_Manager.Instance.Get_Save_777().ToString();
                    }
                    break;
                case MG_SpecialTokenType.SlotsToken:
                    img_SpecialToken.sprite = sp_SlotsToken;
                    text_SpecialToken.text = MG_Manager.Instance.Get_Save_Diamond().ToString();
                    go_SpecialToken.SetActive(true);
                    break;
                case MG_SpecialTokenType.DiceToken:
                    if (!packB)
                        go_SpecialToken.SetActive(false);
                    else
                    {
                        go_SpecialToken.SetActive(true);
                        img_SpecialToken.sprite = sp_DiceToken;
                        text_SpecialToken.text = MG_Manager.Instance.Get_Save_Amazon().ToString();
                    }
                    break;
                case MG_SpecialTokenType.Null:
                    break;
            }
        }
        public void FlyToTarget(Vector3 startPos,MG_MenuFlyTarget flyTarget,int num)
        {
            MG_Manager.Instance.MG_Fly.FlyToTarget(startPos, GetFlyTargetPos(flyTarget), num, flyTarget, FlyToTargetCallback);
        }
        public void UpdateCashText()
        {
            text_Cash.text = MG_Manager.Get_CashShowText(MG_Manager.Instance.Get_Save_Cash());
        }
        public void UpdateGoldText()
        {
            text_Gold.text = MG_Manager.Instance.Get_Save_Gold().ToString();
        }
        public void UpdateScratchTicketText()
        {
            text_ScratchTicketNum.text = MG_Manager.Instance.Get_Save_ScratchTicket().ToString();
        }
        public void UpdateWheelRP()
        {
            go_wheelRP.SetActive(MG_Manager.Instance.Get_Save_WheelTickets() > 0);
        }
        public void UpdateSingRP()
        {
            go_signRP.SetActive(MG_Manager.Instance.Get_Save_WetherSign());
        }
        public void UpdateSpecialTokenText()
        {
            int panelIndex = MG_SaveManager.Current_GamePanel;
            if (panelIndex == (int)MG_GamePanelType.ScratchPanel)
            {
                trans_SelectGame.localPosition = new Vector3(scratchX, selectGameY);
                SetSpecialToken(MG_SpecialTokenType.ScratchToken);
            }
            else if (panelIndex == (int)MG_GamePanelType.DicePanel)
            {
                trans_SelectGame.localPosition = new Vector3(diceX, selectGameY);
                SetSpecialToken(MG_SpecialTokenType.DiceToken);
            }
            else if (panelIndex == (int)MG_GamePanelType.SlotsPanel)
            {
                trans_SelectGame.localPosition = new Vector3(slotsX, selectGameY);
                SetSpecialToken(MG_SpecialTokenType.SlotsToken);
            }
        }
        public readonly Dictionary<int, Transform> dic_flytarget_transform = new Dictionary<int, Transform>();
        Vector3 GetFlyTargetPos(MG_MenuFlyTarget _flyTarget)
        {
            if(dic_flytarget_transform.TryGetValue((int)_flyTarget,out Transform trans_Target))
            {
                return trans_Target.position;
            }
            return Vector3.zero;
        }
        void FlyToTargetCallback(MG_MenuFlyTarget _flyTarget)
        {
            if (_flyTarget == MG_MenuFlyTarget.WheelTicket)
            {
                MG_UIManager.Instance.UpdateWheelTicketText();
                return;
            }
            StopCoroutine("ExpandTarget");
            StartCoroutine("ExpandTarget", _flyTarget);
        }
        IEnumerator ExpandTarget(MG_MenuFlyTarget _flyTarget)
        {
            if (!dic_flytarget_transform.TryGetValue((int)_flyTarget, out Transform tempTrans))
                yield break;
            bool toBiger = true;
            while (true)
            {
                yield return null;
                if (toBiger)
                {
                    tempTrans.localScale += Vector3.one * Time.unscaledDeltaTime * 3;
                    if (tempTrans.localScale.x >= 1.3f)
                    {
                        toBiger = false;
                        switch (_flyTarget)
                        {
                            case MG_MenuFlyTarget.OneGold:
                                UpdateGoldText();
                                break;
                            case MG_MenuFlyTarget.Cash:
                                UpdateCashText();
                                break;
                            case MG_MenuFlyTarget.Diamond:
                                UpdateSpecialTokenText();
                                break;
                            case MG_MenuFlyTarget.Scratch:
                            case MG_MenuFlyTarget.ScratchTicket:
                                UpdateScratchTicketText();
                                break;
                            default:
                                UpdateSpecialTokenText();
                                break;
                        }
                    }
                }
                else
                {
                    tempTrans.localScale -= Vector3.one * Time.unscaledDeltaTime * 3;
                    if (tempTrans.localScale.x <= 1f)
                        break;
                }
            }
            if (_flyTarget == MG_MenuFlyTarget.Scratch)
            {
                if (!go_scratchRP.activeSelf)
                    go_scratchRP.SetActive(true);
            }
            yield return null;
            tempTrans.localScale = Vector3.one;
        }
        public override IEnumerator OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            UpdateAllContent();
            btn_Back.gameObject.SetActive(false);
            rect_Top.gameObject.SetActive(false);
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            yield return null;
        }

        public override void OnPause()
        {
            rect_Top.gameObject.SetActive(true);
            btn_Scratch.gameObject.SetActive(false);
            btn_Slots.gameObject.SetActive(false);
            btn_Wheel.gameObject.SetActive(false);
            btn_Back.gameObject.SetActive(true);
        }

        public override void OnResume()
        {
            rect_Top.gameObject.SetActive(false);
            btn_Scratch.gameObject.SetActive(true);
            btn_Slots.gameObject.SetActive(true);
            btn_Wheel.gameObject.SetActive(true);
            btn_Back.gameObject.SetActive(false);
        }
        public void HideButton()
        {
            btn_Slots.gameObject.SetActive(false);
            btn_Scratch.gameObject.SetActive(false);
            btn_Wheel.gameObject.SetActive(false);
        }
        public void ShowButton()
        {
            btn_Slots.gameObject.SetActive(true);
            btn_Scratch.gameObject.SetActive(true);
            btn_Wheel.gameObject.SetActive(true);
        }
        public void CheckGuid()
        {
            if (MG_Manager.Instance.isGuid)
            {
                if (!MG_Manager.Instance.Get_Save_PackB())
                {
                    MG_Manager.Instance.isGuid = false;
                    return;
                }
                switch (MG_Manager.Instance.next_GuidType)
                {
                    case MG_Guid_Type.DiceGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_Cash.transform.SetAsLastSibling();
                        trans_guidBase.localScale = new Vector3(-1, 1, 1);
                        text_guidDes.transform.localScale = new Vector3(-1, 1, 1);
                        img_guidIcon.transform.localScale = new Vector3(-1, 1, 1);
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_CashoutSP_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_BlueBgSP_name);
                        text_guidDes.text = str_guidCashout;
                        MG_SaveManager.GuidDice = true;
                        StartCoroutine(WaitForClickDiceGuid());
                        break;
                    case MG_Guid_Type.ScratchGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_SpecialToken.transform.SetAsLastSibling();
                        trans_guidBase.localScale = Vector3.one;
                        text_guidDes.transform.localScale = Vector3.one;
                        img_guidIcon.transform.localScale = Vector3.one;
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_GiftSP_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_BlueBgSP_name);
                        text_guidDes.text = str_guid7;
                        MG_SaveManager.GuidScratch = true;
                        StartCoroutine(WaitForClickScratchGuid());
                        break;
                    case MG_Guid_Type.SlotsGuid:
                        trans_guidMask.gameObject.SetActive(true);
                        trans_guidMask.SetParent(rect_Top);
                        trans_guidMask.SetAsLastSibling();
                        btn_SpecialToken.transform.SetAsLastSibling();
                        trans_guidBase.localScale = Vector3.one;
                        text_guidDes.transform.localScale = Vector3.one;
                        img_guidIcon.transform.localScale = Vector3.one;
                        img_guidIcon.sprite = MenuAtlas.GetSprite(str_GiftSP_name);
                        img_guidBG.sprite = MenuAtlas.GetSprite(str_GreenBgSP_name);
                        text_guidDes.text = str_guidDimond;
                        MG_SaveManager.GuidSlots = true;
                        StartCoroutine(WaitForClickScratchGuid());
                        break;
                }
            }
        }
        const string str_guidCashout = "<size=70><color=#FFF408>You'have won cash!</color></size>\nOnce you get up to specified amount\nyou can cash out with PayPal.";
        float f_guidY = 0;
        const string str_guidAmazon = "<size=70><color=#FFF408>Win the big prize</color></size>\nOnce you meet the requirements,\nyou can get a huge bonus";
        const string str_guid7 = "<size=70><color=#FFF408>Lucky Seven Day</color></size>\nCollect lucky 7 to redeem cash\nrewards";
        const string str_guidDimond = "<size=70><color=#FFF408>Redeem gifts</color></size>\nYou can use coins to redeem\nGift Cards and more!";

        const string str_CashoutSP_name = "MG_Sprite_Menu_Guid_Paypal";
        const string str_AmazonSP_name = "MG_Sprite_Menu_Guid_Amazon";
        const string str_GiftSP_name = "MG_Sprite_Menu_Guid_Cashout";
        const string str_BlueBgSP_name = "MG_Sprite_Menu_Guid_B";
        const string str_GreenBgSP_name = "MG_Sprite_Menu_Guid_G";
        const string str_OrangeBgSP_name = "MG_Sprite_Menu_Guid_O";
        bool hasClickGuid = false;
        bool canClickButton = false;
        IEnumerator WaitForClickDiceGuid()
        {
            hasClickGuid = false;
            canClickButton = false;
            yield return new WaitForSeconds(Time.timeScale);
            canClickButton = true;
            while (true)
            {
                if (hasClickGuid)
                    break;
                yield return null;
            }
            trans_guidMask.SetAsLastSibling();
            btn_SpecialToken.transform.SetAsLastSibling();
            trans_guidBase.localScale = Vector3.one;
            text_guidDes.transform.localScale = Vector3.one;
            img_guidIcon.transform.localScale = Vector3.one;
            img_guidIcon.sprite = MenuAtlas.GetSprite(str_AmazonSP_name);
            img_guidBG.sprite = MenuAtlas.GetSprite(str_OrangeBgSP_name);
            text_guidDes.text = str_guidAmazon;
            hasClickGuid = false;
            canClickButton = false;
            yield return new WaitForSeconds(Time.timeScale);
            canClickButton = true;
            while (true)
            {
                yield return null;
                if (hasClickGuid)
                    break;
            }
            trans_guidMask.SetParent(transform);
            trans_guidMask.gameObject.SetActive(false);
            MG_Manager.Instance.isGuid = false;
            MG_Manager.Instance.next_GuidType = MG_Guid_Type.Null;
        }
        IEnumerator WaitForClickScratchGuid()
        {
            hasClickGuid = false;
            canClickButton = false;
            yield return new WaitForSeconds(Time.timeScale);
            canClickButton = true;
            while (true)
            {
                if (hasClickGuid)
                    break;
                yield return null;
            }
            trans_guidMask.SetParent(transform);
            trans_guidMask.gameObject.SetActive(false);
            MG_Manager.Instance.isGuid = false;
            MG_Manager.Instance.next_GuidType = MG_Guid_Type.Null;
        }
        void OnMaskButtonClick()
        {
            if (canClickButton)
                hasClickGuid = true;
        }
    }
}
