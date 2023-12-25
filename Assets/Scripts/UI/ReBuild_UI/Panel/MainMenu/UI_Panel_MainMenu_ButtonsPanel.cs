using System;
using ToufFrame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public partial class UI_Panel_MainMenu_ButtonsPanel : BasePanel
{
    public override string ResourceName { get=>GameStrings.ADDRESSABLES_UI_PANEL_Buttons;  }
    private UI_Canvas_MainMenu menuCanvas;
    
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    
    
    private void Start()
    {
        menuCanvas=rootCanvas as UI_Canvas_MainMenu;        
        if(menuCanvas==null)Debug.LogWarning(" menuCanvas==null");
        
        BUC_Button_Press2Start.button.onClick.AddListener(PressToStart);
        BUC_Button_NewGame.button.onClick.AddListener(menuCanvas.NewtGame);
        BUC_Button_LoadGame.button.onClick.AddListener(menuCanvas.EnterLoadMenu);
        
        BUC_Button_LoadGame.gameObject.SetActive(false);
        BUC_Button_NewGame.gameObject.SetActive(false);
    }

    public void PressToStart()
    {
        NetworkManager.Singleton.StartHost();
        
        
        BUC_Button_Press2Start.gameObject.SetActive(false);
        
        BUC_Button_NewGame.gameObject.SetActive(true);
        BUC_Button_LoadGame.gameObject.SetActive(true);
        
        
    }
    
    
    
        
}