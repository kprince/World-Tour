using MiddleGround.Save;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using System;

namespace MiddleGround.UI
{
    public class MG_UIManager : MonoBehaviour
    {
        public static MG_UIManager Instance;
        readonly Stack<MG_UIBase> Panel_Stack = new Stack<MG_UIBase>();
        readonly Dictionary<int, string> Type_Path_Dic = new Dictionary<int, string>()
        {
            {(int)MG_GamePanelType.DicePanel,"MG_Prefabs/MG_GamePanels/MG_GamePanel_Dice" },
            {(int)MG_GamePanelType.ScratchPanel,"MG_Prefabs/MG_GamePanels/MG_GamePanel_Scratch" },
            {(int)MG_GamePanelType.SlotsPanel,"MG_Prefabs/MG_GamePanels/MG_GamePanel_Slots" },
            {(int)MG_PopPanelType.DiceRewardPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_DiceReward" },
            {(int)MG_PopPanelType.ExtraRewardPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_ExtraReward" },
            {(int)MG_PopPanelType.SettingPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Setting" },
            {(int)MG_PopPanelType.BuyDiceEnergy,"MG_Prefabs/MG_PopPanels/MG_PopPanel_DiceBuyEnergy" },
            {(int)MG_PopPanelType.DiceSlotsPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_DiceSlots" },
            {(int) MG_PopPanelType.DoublePanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Double"},
            {(int)MG_PopPanelType.CashoutPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Cashout" },
            {(int)MG_PopPanelType.Tips,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Tips" },
            {(int)MG_PopPanelType.ShopPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Shop" },
            {(int)MG_PopPanelType.Rateus,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Rateus" },
            {(int)MG_PopPanelType.WheelPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Wheel" },
            {(int)MG_PopPanelType.SignPanel,"MG_Prefabs/MG_PopPanels/MG_PopPanel_Sign" },
        };
        readonly Dictionary<int, MG_UIBase> LoadedPanel_Dic = new Dictionary<int, MG_UIBase>();

        readonly Dictionary<int, string> Type_SAPath_Dic = new Dictionary<int, string>()
        {
            {(int)MG_GamePanelType.DicePanel,"MG_SpriteAltas/MG_GamePanel_Dice" },
            {(int)MG_PopPanelType.ShopPanel,"MG_SpriteAltas/MG_PopPanel_Shop" },
            {(int)MG_PopPanelType.Random,"MG_SpriteAltas/MG_PopPanel_Random" },
            {(int)MG_PopPanelType.SettingPanel,"MG_SpriteAltas/MG_PopPanel_Setting" },
            {(int)MG_GamePanelType.ScratchPanel,"MG_SpriteAltas/MG_GamePanel_Scratch" },
            {(int)MG_PopPanelType.DiceSlotsPanel,"MG_SpriteAltas/MG_PopPanel_DiceSlots" },
            {(int)MG_PopPanelType.DiceRewardPanel,"MG_SpriteAltas/MG_PopPanel_Reward" },
            {(int)MG_PopPanelType.WheelPanel,"MG_SpriteAltas/MG_PopPanel_Wheel" },
            {(int)MG_PopPanelType.SignPanel ,"MG_SpriteAltas/MG_PopPanel_Sign"},
        };
        readonly Dictionary<int, SpriteAtlas> LoadedSpriteAtlas_Dic = new Dictionary<int, SpriteAtlas>();
        const string MenuPanelPath = "MG_Prefabs/MG_MenuPanel";
        MG_UIBase Current_GamePanel = null;
        public MG_MenuPanel MenuPanel = null;
        SpriteAtlas MenuAtlas = null;
        Transform PopPanelRoot;
        Transform GamePanelRoot;
        Transform MenuPanelRoot;
        public void Init(Transform gamePanelRoot, Transform menuPanelRoot,Transform popPanelRoot)
        {
            PopPanelRoot = popPanelRoot;
            GamePanelRoot = gamePanelRoot;
            MenuPanelRoot = menuPanelRoot;
            Instance = this;
        }
        readonly Queue<PanelTask> Queue_PopPanel = new Queue<PanelTask>();
        Coroutine Cor_PopPanelTask = null;
        public void ShowPopPanelAsync(MG_PopPanelType _PopPanelType)
        {
            PanelTask newTask = new PanelTask()
            {
                t_panelType = _PopPanelType,
                t_open = true
            };
            Queue_PopPanel.Enqueue(newTask);
            if (Cor_PopPanelTask is null)
                Cor_PopPanelTask = StartCoroutine(ExcuteTask());
        }
        public void ClosePopPanelAsync(MG_PopPanelType _PopPanelType)
        {
            PanelTask newTask = new PanelTask()
            {
                t_panelType = _PopPanelType,
                t_open = false
            };
            Queue_PopPanel.Enqueue(newTask);
            if (Cor_PopPanelTask is null)
                Cor_PopPanelTask = StartCoroutine(ExcuteTask());
        }
        IEnumerator ExcuteTask()
        {
            while (Queue_PopPanel.Count > 0)
            {
                PanelTask nextTask = Queue_PopPanel.Dequeue();
                int panelIndex = (int)nextTask.t_panelType;
                bool open = nextTask.t_open;
                if(LoadedPanel_Dic.TryGetValue(panelIndex,out MG_UIBase loadedPopPanel))
                {
                    if(loadedPopPanel is null)
                    {
                        Debug.LogWarning((open ? "Show" : "Close") + " MG_PopPanel-" + panelIndex + " Error : loadedDic has key , but content is null.");
                        continue;
                    }
                    else
                    {
                        if (Panel_Stack.Contains(loadedPopPanel))
                        {
                            if (open)
                            {
                                Debug.LogWarning("Show MG_PopPanel-" + panelIndex + " Error : panel has showed.");
                                continue;
                            }
                            else
                            {
                                while (true)
                                {
                                    if (Panel_Stack.Count > 0)
                                    {
                                        MG_UIBase outPanel = Panel_Stack.Pop();
                                        yield return outPanel.OnExit();
                                        if (MG_Manager.Instance.next_GuidType != MG_Guid_Type.Null)
                                            MG_Manager.Instance.isGuid = true;
                                        if (Panel_Stack.Count > 0)
                                            Panel_Stack.Peek().OnResume();
                                        else
                                            MenuPanel.OnResume();
                                        if (outPanel == loadedPopPanel)
                                            break;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (open)
                            {
                                while (true)
                                {
                                    if (MG_Manager.Instance.isGuid == false)
                                        break;
                                    yield return null;
                                }
                                if (Panel_Stack.Count > 0)
                                    Panel_Stack.Peek().OnPause();
                                else
                                    MenuPanel.OnPause();
                                loadedPopPanel.transform.SetAsLastSibling();
                                Panel_Stack.Push(loadedPopPanel);
                                yield return loadedPopPanel.OnEnter();
                            }
                            else
                            {
                                Debug.LogWarning("Close MG_PopPanel-" + panelIndex + " Error : panel has not show.");
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (open)
                    {
                        if (Type_Path_Dic.TryGetValue(panelIndex, out string panelPath))
                        {
                            if (string.IsNullOrEmpty(panelPath))
                            {
                                Debug.LogWarning("Show MG_PopPanel-" + panelIndex + " Error : panelPathDic content is null or empty.");
                                continue;
                            }
                            else
                            {
                                while (true)
                                {
                                    if (MG_Manager.Instance.isGuid == false)
                                        break;
                                    yield return null;
                                }
                                if (Panel_Stack.Count > 0)
                                    Panel_Stack.Peek().OnPause();
                                else
                                    MenuPanel.OnPause();
                                MG_UIBase nextShowPanel = Instantiate(Resources.Load<GameObject>(panelPath), PopPanelRoot).GetComponent<MG_UIBase>();
                                nextShowPanel.transform.SetAsLastSibling();
                                Panel_Stack.Push(nextShowPanel);
                                LoadedPanel_Dic.Add(panelIndex, nextShowPanel);
                                yield return nextShowPanel.OnEnter();
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Show MG_PopPanel-" + panelIndex + " Error : panelPathDic content is null or empty.");
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Close MG_PopPanel-" + panelIndex + " Error : panel has not loaded or show.");
                        continue;
                    }
                }
            }
            Cor_PopPanelTask = null;
        }
        public bool CloseTopPopPanelAsync()
        {
            if (Panel_Stack.Count > 0)
            {
                MG_UIBase _UIBase = Panel_Stack.Peek();
                if (LoadedPanel_Dic.ContainsValue(_UIBase))
                {
                    foreach(var keyValue in LoadedPanel_Dic)
                    {
                        if (keyValue.Value == _UIBase)
                        {
                            ClosePopPanelAsync((MG_PopPanelType)keyValue.Key);
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        public bool ShowGamePanel(MG_GamePanelType _PanelType)
        {
            int panelIndex = (int)_PanelType;
            MG_SaveManager.Current_GamePanel = panelIndex;
            if(LoadedPanel_Dic.TryGetValue(panelIndex,out MG_UIBase loadedGamePanel))
            {
                if(loadedGamePanel is null)
                {
                    Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : loadedDic has key , but content is null.");
                    return false;
                }
                if (Current_GamePanel == loadedGamePanel)
                {
                    //Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panel has show.");
                    return false;
                }
                if (Current_GamePanel is object)
                {
                    StartCoroutine(Current_GamePanel.OnExit());
                }
                StartCoroutine(loadedGamePanel.OnEnter());
                Current_GamePanel = loadedGamePanel;
            }
            else
            {
                if (Type_Path_Dic.TryGetValue(panelIndex, out string panelPath))
                {
                    if (string.IsNullOrEmpty(panelPath))
                    {
                        Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panelPathDic content is null or empty.");
                        return false;
                    }
                    MG_UIBase nextShowPanel = Instantiate(Resources.Load<GameObject>(panelPath), GamePanelRoot).GetComponent<MG_UIBase>();
                    if (Current_GamePanel is object)
                    {
                        StartCoroutine(Current_GamePanel.OnExit());
                    }
                    nextShowPanel.transform.SetAsLastSibling();
                    StartCoroutine(nextShowPanel.OnEnter());
                    LoadedPanel_Dic.Add(panelIndex, nextShowPanel);
                    Current_GamePanel = nextShowPanel;
                }
                else
                {
                    Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panelPathDic content is null or empty.");
                    return false;
                }
            }
            return true;
        }
        public void ShowMenuPanel(MG_GamePanelType startGamePanel = MG_GamePanelType.DicePanel)
        {
            if(MenuPanel is null)
            {
                MG_SaveManager.Current_GamePanel = (int)startGamePanel;
                MenuPanel = Instantiate(Resources.Load<GameObject>(MenuPanelPath), MenuPanelRoot).GetComponent<MG_MenuPanel>();
                StartCoroutine(MenuPanel.OnEnter());
            }
            else
            {
                StartCoroutine(MenuPanel.OnEnter());
            }
            switch (startGamePanel)
            {
                case MG_GamePanelType.DicePanel:
                    MenuPanel.OnDiceButtonClick();
                    break;
                case MG_GamePanelType.ScratchPanel:
                    MenuPanel.OnScratchButtonClick();
                    break;
                case MG_GamePanelType.SlotsPanel:
                    MenuPanel.OnSlotsButtonClick();
                    break;
            }
        }
        public void CloseMenuPanel()
        {
            if(MenuPanel is object)
            {
                StartCoroutine(MenuPanel.OnExit());
                if (Current_GamePanel is object)
                {
                    StartCoroutine(Current_GamePanel.OnExit());
                    Current_GamePanel = null;
                }
            }
        }
        public void FlyEffectTo_MenuTarget(Vector3 startPos,MG_MenuFlyTarget flyTarget,int num)
        {
            MenuPanel.FlyToTarget(startPos, flyTarget, num);
        }
        public void UpdateMenuPanel_CashText()
        {
            MenuPanel.UpdateCashText();
        }
        public void UpdateMenuPanel_GoldText()
        {
            MenuPanel.UpdateGoldText();
        }
        public void UpdateMenuPanel_ScratchTicketText()
        {
            MenuPanel.UpdateScratchTicketText();
        }
        public void UpdateMenuPanel_SpecialTokenText()
        {
            MenuPanel.UpdateSpecialTokenText();
        }
        public void UpdateSlotsSpinButton(int gold)
        {
            if (Current_GamePanel != null && Current_GamePanel is MG_GamePanel_Slots)
            {
                MG_GamePanel_Slots SlotsPanel = Current_GamePanel as MG_GamePanel_Slots;
                SlotsPanel.UpdateSpinButtonState(gold);
            }
        }
        public void UpdateDicePanel_DiceLifeText()
        {
            MG_GamePanel_Dice _GamePanel_Dice = LoadedPanel_Dic[(int)MG_GamePanelType.DicePanel] as MG_GamePanel_Dice;
            _GamePanel_Dice.UpdateDiceLifeAndGiftStepText();
        }
        public void UpdateWheelTicketText()
        {
            MG_PopPanel_Wheel _PopPanel_Wheel = Get_UIPanel((int)MG_PopPanelType.WheelPanel) as MG_PopPanel_Wheel;
            if (_PopPanel_Wheel is null)
                return;
            _PopPanel_Wheel.UpdateWheelTicketShow();
        }
        public void UpdateWheelRP()
        {
            MenuPanel.UpdateWheelRP();
        }
        public void UpdateSignRP()
        {
            MenuPanel.UpdateSingRP();
        }
        public SpriteAtlas GetSpriteAtlas(int index)
        {
            if(LoadedSpriteAtlas_Dic.TryGetValue(index,out SpriteAtlas loadedSA))
            {
                if(loadedSA is null)
                {
                    Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : loadedDic has key , but content is null.");
                    LoadedSpriteAtlas_Dic.Remove(index);
                    return null;
                }
                return loadedSA;
            }
            else
            {
                if(Type_SAPath_Dic.TryGetValue(index,out string path))
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : SAPathDic content is null or empty.");
                        return null;
                    }
                    SpriteAtlas tempSA = Resources.Load<SpriteAtlas>(path);
                    LoadedSpriteAtlas_Dic.Add(index, tempSA);
                    return tempSA;
                }
                else
                {
                    Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : SAPathDic content is null or empty.");
                    return null;
                }
            }
        }
        public SpriteAtlas GetMenuSpriteAtlas()
        {
            if(MenuAtlas is null)
            {
                MenuAtlas = Resources.Load<SpriteAtlas>("MG_SpriteAltas/MG_MenuPanel");
            }
            return MenuAtlas;
        }
        public MG_UIBase Get_UIPanel(int index)
        {
            if(LoadedPanel_Dic.TryGetValue(index,out MG_UIBase temp))
            {
                return temp;
            }
            else
            {
                Debug.LogWarning("Get MG_UIPanel Error : panel has not show.");
                return null;
            }
        }
        struct PanelTask
        {
            public MG_PopPanelType t_panelType;
            public bool t_open;
        }
    }
    public enum MG_PopPanelType
    {
        WheelPanel = 3,
        SignPanel = 4,
        SettingPanel = 5,
        ExchangePanel = 6,
        DiceRewardPanel = 7,
        DiceSlotsPanel = 8,
        ExtraRewardPanel = 9,
        ShopPanel = 10,
        Random = 11,
        BuyDiceEnergy = 12,
        DoublePanel = 13,
        CashoutPanel = 14,
        Tips = 15,
        Rateus = 16,
    }
    public enum MG_GamePanelType
    {
        ScratchPanel = 0,
        DicePanel = 1,
        SlotsPanel = 2
    }
    public enum MG_MenuFlyTarget
    {
        OneGold,
        Cash,
        Amazon,
        ScratchTicket,
        SlotsSpecialToken,
        Scratch,
        Slots,
        SSS,
        Orange,
        Cherry,
        Watermalen,
        Diamond,
        WheelTicket,
    }
}
