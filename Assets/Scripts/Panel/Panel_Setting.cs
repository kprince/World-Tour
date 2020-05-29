using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Panel_Setting : PanelBase
{
    public Button btn_music;
    public Button btn_sound;
    public Image img_music;
    public Image img_sound;
    private SpriteAtlas settingAlats;
    protected override void Awake()
    {
        base.Awake();
        if (settingAlats is null)
            settingAlats = Resources.Load<SpriteAtlas>("SettingPanel");
        btn_music.onClick.AddListener(OnMusicClick);
        btn_sound.onClick.AddListener(OnSoundClick);
        img_music.sprite = settingAlats.GetSprite("music" + (GameManager.Instance.GetMusicOn() ? "on" : "off"));
        img_sound.sprite = settingAlats.GetSprite("sound" + (GameManager.Instance.GetSoundOn() ? "on" : "off"));
    }
    void OnMusicClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        bool musicOn = GameManager.Instance.GetMusicOn();
        GameManager.Instance.SetMusicOn(!musicOn);
        img_music.sprite = settingAlats.GetSprite("music" + (!musicOn ? "on" : "off"));
    }
    void OnSoundClick()
    {
        AudioManager.Instance.PlayerSound("Button");
        bool soundOn = GameManager.Instance.GetSoundOn();
        GameManager.Instance.SetSoundOn(!soundOn);
        img_sound.sprite = settingAlats.GetSprite("sound" + (!soundOn ? "on" : "off"));
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
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    protected override void Close()
    {
        base.Close();
        AudioManager.Instance.PlayerSound("Button");
        PanelManager.Instance.ClosePanel(PanelType.Setting);
    }
}
