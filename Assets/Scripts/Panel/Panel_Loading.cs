using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    IEnumerator LoadResource()
    {
        float time = 0.5f;
        while (true)
        {
            yield return null;
            diceIcon.Rotate(Vector3.forward * Time.deltaTime*500);
            if (loadSlider.value < 1)
            {
                loadSlider.value += Time.deltaTime;
                loadNum.text = Mathf.RoundToInt(loadSlider.value * 100) + "%";
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

        Close();
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
