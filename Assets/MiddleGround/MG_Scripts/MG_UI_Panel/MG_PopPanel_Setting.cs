using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Setting : MG_UIBase
    {
        public Button btn_Close;
        public Button btn_Sound;
        public Button btn_Music;
        public Image img_SoundSwitch;
        public Image img_MusicSwitch;
        SpriteAtlas settingSA;
        Sprite sp_SwitchOn;
        Sprite sp_SwitchOff;
        protected override void Awake()
        {
            base.Awake();
            btn_Close.onClick.AddListener(OnCloseButtonClick);
            btn_Sound.onClick.AddListener(OnSoundButtonClick);
            btn_Music.onClick.AddListener(OnMusicButtonClick);
            settingSA = MG_UIManager.Instance.GetSpriteAtlas((int)MG_PopPanelType.SettingPanel);
            sp_SwitchOn = settingSA.GetSprite("MG_Sprite_Setting_SwitchOn");
            sp_SwitchOff = settingSA.GetSprite("MG_Sprite_Setting_SwitchOff");
        }
        void OnCloseButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.SettingPanel);
        }
        void OnSoundButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            bool oldstate = MG_Manager.Instance.Get_Save_SoundOn();
            MG_Manager.Instance.Set_Save_SoundOn(!oldstate);
            img_SoundSwitch.sprite = oldstate ? sp_SwitchOff : sp_SwitchOn;
        }
        void OnMusicButtonClick()
        {
            MG_Manager.Play_ButtonClick();
            bool oldstate = MG_Manager.Instance.Get_Save_MusicOn();
            MG_Manager.Instance.Set_Save_MusicOn(!oldstate);
            img_MusicSwitch.sprite = oldstate ? sp_SwitchOff : sp_SwitchOn;
        }
        public override IEnumerator OnEnter()
        {
            img_MusicSwitch.sprite = MG_Manager.Instance.Get_Save_MusicOn() ? sp_SwitchOn : sp_SwitchOff;
            img_SoundSwitch.sprite = MG_Manager.Instance.Get_Save_SoundOn() ? sp_SwitchOn : sp_SwitchOff;

            Transform transAll = transform.GetChild(1);
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = true;
            while (transAll.localScale.x < 1)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale += new Vector3(addValue, addValue);
                canvasGroup.alpha += addValue;
            }
            transAll.localScale = Vector3.one;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;

        }

        public override IEnumerator OnExit()
        {
            Transform transAll = transform.GetChild(1);
            canvasGroup.interactable = false;
            while (transAll.localScale.x > 0.8f)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale -= new Vector3(addValue, addValue);
                canvasGroup.alpha -= addValue;
            }
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
