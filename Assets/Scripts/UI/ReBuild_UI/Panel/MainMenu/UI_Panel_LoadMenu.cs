using System;
using ToufFrame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class UI_Panel_LoadMenu : BasePanel
{
    public override string ResourceName { get=>GameStrings.ADDRESSABLES_UI_PANEL_LoadMenu;  }
    UI_Canvas_MainMenu menuCanvas;
    SaveFileDataWriter saveFileDataWriter;
    UI_Character_Save_Slot newSaveSlot;

    

    private void Start()
    {
        menuCanvas=rootCanvas as UI_Canvas_MainMenu;
        if(menuCanvas==null)Debug.LogWarning(" menuCanvas==null");
        
        BUC_Button_Return.button.onClick.AddListener(menuCanvas.ReturnToMainMenu);
        
        AddCreateNewSaveSlot();
    }

    public void AddCreateNewSaveSlot()
    {
        int i = 1;
        for (; i <= 10; i++)
        {
           if(GetType().GetField($"BUC_Save_Slot_{i}")==null) break;
        }

        if (i <= 10)
        {
            newSaveSlot=new GameObject("Save_Slot_New").AddComponent<UI_Character_Save_Slot>();
            
            newSaveSlot.transform.SetParent(BUC_Content_SaveSlots.transform);
            
            newSaveSlot.characterSlot = ECharacterSlots.NoSlot;
            
            newSaveSlot.characterPortrait.color= Color.clear;
            
            newSaveSlot.characterName.text = "New Save Slot";
            
            newSaveSlot.timePlayed.text = " ";

        }
    }
    
    //todo: delete save
}