using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    Jackpot,
    Notice,
    Loading,
    Game,
    Reward,
    BuyEnergy,
    Setting,
    Exchange,
    Signin,
    RateUs,
}
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    [SerializeField]
    private Transform panelRoot;
    private readonly Stack<PanelBase> PanelStack = new Stack<PanelBase>();
    private readonly Dictionary<int, string> PanelPathDic = new Dictionary<int, string>()
    {
        {(int)PanelType.Jackpot,"Panel/Panel_Jackpot" },
        {(int)PanelType.Notice,"Panel/Panel_Notice" },
        {(int)PanelType.Loading,"Panel/Panel_Loading" },
        {(int)PanelType.Game,"Panel/Panel_Game" },
        {(int)PanelType.Reward,"Panel/Panel_Reward" },
        {(int)PanelType.BuyEnergy,"Panel/Panel_BuyEnergy" },
        {(int)PanelType.Setting,"Panel/Panel_Setting" },
        {(int)PanelType.Exchange,"Panel/Panel_Exchange" },
        {(int)PanelType.Signin,"Panel/Panel_Signin" },
        {(int)PanelType.RateUs,"Panel/Panel_RateUs" },
    };
    private readonly Dictionary<int, GameObject> PanelDic = new Dictionary<int, GameObject>();


    private void Awake()
    {
        Instance = this;
    }
    public void PreLoadPanel(PanelType panel)
    {
        int panelIndex = (int)panel;
        if (PanelDic.ContainsKey(panelIndex))
            return;
        string loadPath = string.Empty;
        if(PanelPathDic.TryGetValue(panelIndex,out loadPath) && !string.IsNullOrEmpty(loadPath))
        {
            GameObject tempPanelGo = Instantiate(Resources.Load<GameObject>(loadPath), panelRoot);
            tempPanelGo.GetComponent<CanvasGroup>().alpha = 0;
            tempPanelGo.GetComponent<CanvasGroup>().blocksRaycasts = false;
            PanelDic.Add(panelIndex, tempPanelGo);
        }
        else
        {
            Debug.LogError("尚未配置加载路径，预加载失败，面板：" + panel.ToString());
        }
    }
    public void ShowPanel(PanelType panel)
    {
        int index = (int)panel;
        GameObject panelResource;
        if (PanelDic.TryGetValue(index,out panelResource) && panelResource is object)
        {
            PanelBase newPanel = panelResource.GetComponentInChildren<PanelBase>();
            if (newPanel is null)
            {
                Debug.LogError(panelResource.name + "面板没有挂载panelBase脚本");
                return;
            }
            else
            {
                if (PanelStack.Contains(newPanel))
                {
                    Debug.LogError("已经显示" + panelResource.name + "面板了");
                    return;
                }
                else
                {
                    if (PanelStack.Count > 0)
                    {
                        PanelBase topPanel = PanelStack.Peek();
                        topPanel.OnPause();
                    }
                    PanelStack.Push(newPanel);
                    newPanel.transform.SetAsLastSibling();
                    newPanel.OnEnter();
                }
            }
        }
        else
        {
            string loadPath;
            if(PanelPathDic.TryGetValue(index,out loadPath) && !string.IsNullOrEmpty(loadPath))
            {
                panelResource = Resources.Load<GameObject>(loadPath);
                if(panelResource is object)
                {
                    GameObject newPanelGo = Instantiate(panelResource, panelRoot);
                    PanelDic.Add(index, newPanelGo);
                    PanelBase newPanel = newPanelGo.GetComponentInChildren<PanelBase>();
                    if (PanelStack.Count > 0)
                    {
                        PanelBase topPanel = PanelStack.Peek();
                        topPanel.OnPause();
                    }
                    PanelStack.Push(newPanel);
                    newPanel.OnEnter();
                    newPanel.transform.SetAsLastSibling();
                }
                else
                {
                    Debug.LogError("加载" + panel.ToString() + "面板路径错误，路径 : " + loadPath);
                    return;
                }
            }
            else
            {
                Debug.LogError("加载" + panel.ToString() + "面板错误，没有配置该面板的路径");
                return;
            }
        }
    }
    public void ShowPanel(PanelType panel,float delayTime)
    {
        StartCoroutine(ShowPanelDelay(panel, delayTime));
    }
    public void ClosePanel(PanelType panel)
    {
        int index = (int)panel;
        GameObject panelGo;
        if (PanelDic.TryGetValue(index, out panelGo) && panelGo is object)
        {
            PanelBase panelScript = panelGo.GetComponentInChildren<PanelBase>();
            if (panelScript is object)
            {
                if (PanelStack.Contains(panelScript))
                {
                    while (PanelStack.Count > 0)
                    {
                        PanelBase tempPanel = PanelStack.Pop();
                        tempPanel.OnExit();
                        PanelStack.Peek().OnResume();
                        if (tempPanel == panelScript)
                            break;
                        if (PanelStack.Count <= 0)
                            Debug.LogError("这是不可能出现的错误，如果出现了那就是代码的问题了");
                    }
                }
                else
                {
                    Debug.LogError(panel.ToString() + "面板没有显示，关闭失败");
                    return;
                }
            }
            else
            {
                Debug.LogError(panelGo.name + "面板没有挂载panelBase脚本");
            }
        }
        else
        {
            Debug.LogError("从未实例化过该面板，也没有显示过");
            return;
        }
    }
    public void CloseTopPanel()
    {
        if (PanelStack.Count > 0)
        {
            PanelBase tempPanel = PanelStack.Pop();
            tempPanel.OnExit();
        }
    }
    public void ClearAllPanel()
    {
        foreach (GameObject panel in PanelDic.Values)
            Destroy(panel);
        PanelStack.Clear();
        PanelDic.Clear();
        Resources.UnloadUnusedAssets();
    }
    public void ReleasePanel(PanelType panel)
    {
        int index = (int)panel;
        if (PanelDic.TryGetValue(index, out GameObject panelResource) && panelResource is object)
        {
            PanelBase temp = panelResource.GetComponentInChildren<PanelBase>();
            if (PanelStack.Contains(temp))
            {
                Debug.LogError(panel.ToString() + "面板正在被使用");
                return;
            }
            else
            {
                temp = null;
                Destroy(panelResource);
                panelResource = null;
                PanelDic.Remove(index);
                Resources.UnloadUnusedAssets();
            }
        }
        else
        {
            Debug.LogError(panel.ToString() + "面板没有添加到字典中或者没有加载过");
        }
    }
    IEnumerator ShowPanelDelay(PanelType panel,float delayTime)
    {
        yield return new WaitForSeconds(delayTime*2);
        ShowPanel(panel);
    }
    public GameObject GetPanel(PanelType panel)
    {
        int index = (int)panel;
        if (PanelDic.TryGetValue(index, out GameObject panelGo) && panelGo is object)
        {
            return panelGo;
        }
        else
            return null;
    }
}
