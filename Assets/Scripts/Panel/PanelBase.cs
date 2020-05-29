using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBase : MonoBehaviour
{
    [SerializeField]
    protected Button btn_close;
    protected CanvasGroup canvasGroup;
    protected virtual void Awake()
    {
        if (!(btn_close is null))
            btn_close.onClick.AddListener(Close);
    }
    public virtual void OnEnter() 
    {
        if (canvasGroup is null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void OnPause() { }
    public virtual void OnResume() { }
    public virtual void OnExit() { }
    protected virtual void Close() { }
}
