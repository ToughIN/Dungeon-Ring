using System.Collections;
using System.Collections.Generic;
using ToufFrame;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoSingletonBaseDestroyOnLoad<TitleScreenManager>
{
    [SerializeField] private GameObject titleScreenMainMenu;
    [SerializeField] private GameObject titleScreenLoadMenu;

    [Header("Buttons")] 
    [SerializeField] private Button mainMenuNewGameButton;
    [SerializeField] private Button loadMenuReturnButton;
    [SerializeField] private Button deleteCharacterPopUpConfirmButton;
    [SerializeField] private Button mainMenuLoadGameButton;

    [Header("Pop Ups")] 
    [SerializeField] private GameObject noCharacterSlotsPopUp;
    [SerializeField] private GameObject deleteCharacterSlotPopUp;
    [SerializeField] private Button noCharacterSlotsOkayButton;

    [Header("Character Slots")] 
    public ECharacterSlots currentSelectedSlot=ECharacterSlots.NoSlot;
    
    
    
    
    
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.Instance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        
        titleScreenLoadMenu.SetActive(true);
        
        loadMenuReturnButton.Select();
        
    }
    
    public void CloseLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(true);
        
        titleScreenLoadMenu.SetActive(false);
        
        mainMenuLoadGameButton.Select();
    }
    
    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(true);
    }
    
    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuLoadGameButton.Select();
    }
    
    
    
    public void SelectCharacterSlot(ECharacterSlots characterSlot)
    {
        currentSelectedSlot = characterSlot;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = ECharacterSlots.NoSlot;
    }
    
    public void AttemptToDeleteCharacterSlot()
    {
        if (currentSelectedSlot != ECharacterSlots.NoSlot)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmButton.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.Instance.DeleteGame(currentSelectedSlot);
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }
    
    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();
    }
    
    
}
