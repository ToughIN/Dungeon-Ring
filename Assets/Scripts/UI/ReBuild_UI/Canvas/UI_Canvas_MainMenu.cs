using System;
using ToufFrame;
using UnityEngine.AddressableAssets;

public  class UI_Canvas_MainMenu : BaseCanvas
{
    public override string ResourceName
    {
        get => GameStrings.ADDRESSABLES_UI_CANVAS_MainMenu;
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        uiMgr.canvasDIC.TryAdd(ResourceName, this);
        uiMgr.ShowPanel<UI_Panel_MainMenu_ButtonsPanel>(ResourceName,GameStrings.ADDRESSABLES_UI_PANEL_Buttons,EUIlayer.Mid);
        uiMgr.ShowPanel<UI_Panel_MainMenu_Background>(ResourceName,GameStrings.ADDRESSABLES_UI_PANEL_Background,EUIlayer.Bot);


    }
    
    public void EnterLoadMenu()
    {
        uiMgr.ClosePanel(GameStrings.ADDRESSABLES_UI_PANEL_Buttons);
        uiMgr.ShowPanel<UI_Panel_LoadMenu>(ResourceName,GameStrings.ADDRESSABLES_UI_PANEL_LoadMenu,EUIlayer.Mid);
    }

    public void ReturnToMainMenu()
    {
        uiMgr.ClosePanel( GameStrings.ADDRESSABLES_UI_PANEL_LoadMenu);
        uiMgr.ShowPanel<UI_Panel_MainMenu_ButtonsPanel>(ResourceName,GameStrings.ADDRESSABLES_UI_PANEL_Buttons,EUIlayer.Mid);
    }
    
    public void NewtGame()
    {
        WorldSaveGameManager.Instance.AttemptToCreateNewGame();
    }

}