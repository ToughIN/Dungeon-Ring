using System;
using ToufFrame;
using UnityEngine;
using UnityEngine.AddressableAssets;

public  class UI_Canvas_PlayerHUD : BaseCanvas
{
    public override string ResourceName
    {
        get => GameStrings.ADDRESSABLES_UI_CANVAS_PlayerHUD;
    }
    
    [Header("NET JOIN")]
    [SerializeField] private bool startGameAsClient;

    [HideInInspector] public UI_Panel_HUD_Basic uiPanelHudBasic;
    [HideInInspector] public PlayerUIPopupManager playerUIPopupManager;
    private void OnEnable()
    {
        Init();
        DontDestroyOnLoad(this);
    }
    public override void Init()
    {
        base.Init();
        uiMgr.canvasDIC.TryAdd(ResourceName, this);
        uiMgr.ShowPanel<UI_Panel_HUD_Basic>(ResourceName, GameStrings.ADDRESSABLES_UI_PANEL_HUD_Basic,EUIlayer.Top, (panel) =>
        {
            uiPanelHudBasic = panel;
        });
    }
    private void OnDestroy()
    {
        uiMgr.canvasDIC.Remove(ResourceName);
    }

    private void Start()
    {
        
    }
}
