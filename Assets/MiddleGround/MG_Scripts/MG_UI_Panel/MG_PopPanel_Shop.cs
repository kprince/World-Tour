using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Shop : MG_UIBase
    {
        public RectTransform rect_Top;

        public Button btn_back;
        public Button btn_get1;
        public Button btn_get2;
        public Button btn_get3;
        public Button btn_get4;
        public Button btn_get5;

        public Image img_icon1;
        public Image img_icon2;
        public Image img_icon3;
        public Image img_icon4;
        public Image img_icon5;
        public Image img_rewardIcon1;
        public Image img_rewardIcon2;
        public Image img_rewardIcon3;
        public Image img_rewardIcon4;
        public Image img_rewardIcon5;
        public Text text_des1;
        public Text text_des2;
        public Text text_des3;
        public Text text_des4;
        public Text text_des5;
        public Text text_num1;
        public Text text_num2;
        public Text text_num3;
        public Text text_num4;
        public Text text_num5;
        SpriteAtlas shopAtlas;
        Sprite sp_cash;
        Sprite sp_diamond;
        Sprite sp_amazon;
        Sprite sp_sss;
        Sprite sp_fruit;
        Sprite sp_amazon100;
        Sprite sp_amazon200;
        Sprite sp_amazon500;
        Sprite sp_amazon1000;
        Sprite sp_amazon10000;
        protected override void Awake()
        {
            base.Awake();
            float lwr = Screen.height / Screen.width;
            if (lwr > 4 / 3f)
                rect_Top.anchoredPosition = new Vector2(0, 0);
            btn_back.onClick.AddListener(OnBackButtonClick);

            shopAtlas = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.ShopPanel);
            sp_cash = shopAtlas.GetSprite("MG_Sprite_Shop_CashB");
            sp_diamond = shopAtlas.GetSprite("MG_Sprite_Shop_Diamond");
            sp_amazon = shopAtlas.GetSprite("MG_Sprite_Shop_Amazon");
            sp_sss = shopAtlas.GetSprite("MG_Sprite_Shop_SSS");
            sp_fruit = shopAtlas.GetSprite("MG_Sprite_Shop_Fruits");
            sp_amazon100 = shopAtlas.GetSprite("MG_Sprite_Shop_Paypal50");
            sp_amazon200 = shopAtlas.GetSprite("MG_Sprite_Shop_Paypal100");
            sp_amazon500 = shopAtlas.GetSprite("MG_Sprite_Shop_AmazonCard500");
            sp_amazon1000 = shopAtlas.GetSprite("MG_Sprite_Shop_AmazonCard1000");
            sp_amazon10000 = shopAtlas.GetSprite("MG_Sprite_Shop_Gfit");

            img_icon1.sprite = sp_diamond;
            img_icon2.sprite = sp_cash;
            img_icon3.sprite = sp_amazon;
            img_icon4.sprite = sp_sss;
            img_icon5.sprite = sp_fruit;
            img_rewardIcon1.sprite = sp_amazon100;
            img_rewardIcon2.sprite = sp_amazon200;
            img_rewardIcon3.sprite = sp_amazon500;
            img_rewardIcon4.sprite = sp_amazon1000;
            img_rewardIcon5.sprite = sp_amazon10000;

            text_des1.text = "5M Gems=$50";
            text_des2.text = "100 Dollar=$100";
            text_des3.text = "100 Cards=$500";
            text_des4.text = "150 Lukcy=$1000";
            text_des5.text = "200 Fruits\n=$10,000";

            btn_get1.onClick.AddListener(OnGetButtonClick);
            btn_get2.onClick.AddListener(OnGetButtonClick);
            btn_get3.onClick.AddListener(OnGetButtonClick);
            btn_get4.onClick.AddListener(OnGetButtonClick);
            btn_get5.onClick.AddListener(OnGetButtonClick);
        }
        void OnBackButtonClick()
        {
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.ShopPanel);
        }
        void OnGetButtonClick()
        {
            MG_Manager.Instance.Show_PopTipsPanel("Not enough money to exchange.");
        }
        public override IEnumerator OnEnter()
        {
            yield return null;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            text_num1.text = "<color=#FF780E>" + MG_Manager.Instance.Get_Save_Diamond() + "</color>/5000000";
            text_num2.text = "<color=#FF780E>" + MG_Manager.Get_CashShowText(MG_Manager.Instance.Get_Save_Cash())  + "</color>/100";
            text_num3.text = "<color=#FF780E>" + MG_Manager.Instance.Get_Save_Amazon() + "</color>/100";
            text_num4.text = "<color=#FF780E>" + MG_Manager.Instance.Get_Save_777() + "</color>/150";
            text_num5.text = "<color=#FF780E>" + MG_Manager.Instance.Get_Save_Fruits() + "</color>/200";
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            yield return null;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
